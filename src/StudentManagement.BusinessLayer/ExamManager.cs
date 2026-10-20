using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using StudentManagement.BusinessLayer.Offline;
using StudentManagement.DataAccessLayer;
using StudentManagement.DataAccessLayer.Data_Access_Methods;
using StudentManagement.UserObjects.Enums;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.BusinessLayer
{
    public class ExamManager
    {
        public List<ExamTerm> GetActiveTerms()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.Terms.ToList();

            var dt = new tblExam().GetActiveTerms();
            return dt.AsEnumerable().Select(r => new ExamTerm
            {
                ExamTermId = Convert.ToInt32(r["ExamTermId"]),
                TermName = Convert.ToString(r["TermName"]) ?? "",
                AcademicSession = Convert.ToString(r["AcademicSession"]) ?? "",
                StartDate = Convert.ToDateTime(r["StartDate"]),
                EndDate = Convert.ToDateTime(r["EndDate"]),
                TimetableJson = r.Table.Columns.Contains("TimetableJson")
                    ? Convert.ToString(r["TimetableJson"]) : null
            }).ToList();
        }

        /// <summary>Saved clearance row for student + term (if any).</summary>
        public ExamClearanceDto? GetSavedClearance(int studentId, int examTermId)
        {
            var student = new StudentManager().GetById(studentId);
            var term = GetActiveTerms().FirstOrDefault(t => t.ExamTermId == examTermId);

            if (DbConfig.OfflineMode)
            {
                var offline = OfflineStore.Instance.Clearances
                    .FirstOrDefault(c => c.StudentId == studentId && c.ExamTermId == examTermId);
                if (offline == null) return null;
                if (student != null)
                {
                    offline.RegistrationNo = student.RegistrationNo;
                    offline.FullName = student.FullName;
                    offline.ClassName = student.ClassName;
                    offline.RollNumber = student.RollNumber;
                }
                if (term != null) offline.TermName = term.TermName;
                return offline;
            }

            var dt = new tblExam().GetClearance(studentId, examTermId);
            if (dt == null || dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new ExamClearanceDto
            {
                ClearanceId = Convert.ToInt64(r["ClearanceId"]),
                StudentId = Convert.ToInt32(r["StudentId"]),
                RegistrationNo = student?.RegistrationNo ?? "",
                FullName = student?.FullName ?? "",
                ClassName = student?.ClassName ?? "",
                RollNumber = student?.RollNumber ?? "",
                ExamTermId = Convert.ToInt32(r["ExamTermId"]),
                TermName = term?.TermName ?? "",
                DuesAtClearance = r["DuesAtClearance"] != DBNull.Value ? Convert.ToDecimal(r["DuesAtClearance"]) : 0,
                IsCleared = Convert.ToBoolean(r["IsCleared"]),
                AdminOverride = Convert.ToBoolean(r["AdminOverride"]),
                OverrideReason = r.Table.Columns.Contains("OverrideReason") ? Convert.ToString(r["OverrideReason"]) : null
            };
        }

        public ExamClearanceDto VerifyClearance(
            int studentId, int examTermId, bool adminOverride = false,
            string? overrideReason = null, int? userId = null, UserRole? role = null)
        {
            var student = new StudentManager().GetById(studentId)
                          ?? throw new InvalidOperationException("Student not found.");
            var term = GetActiveTerms().FirstOrDefault(t => t.ExamTermId == examTermId)
                       ?? throw new InvalidOperationException("Exam term not found.");

            var summary = new FeeManager().GetSummary(studentId);
            var hasDues = summary.NetDue > 0;

            if (hasDues && !adminOverride)
            {
                return new ExamClearanceDto
                {
                    StudentId = student.StudentId,
                    RegistrationNo = student.RegistrationNo,
                    FullName = student.FullName,
                    ClassName = student.ClassName,
                    RollNumber = student.RollNumber,
                    ExamTermId = term.ExamTermId,
                    TermName = term.TermName,
                    DuesAtClearance = summary.NetDue,
                    IsCleared = false
                };
            }

            if (hasDues && adminOverride && role != UserRole.SuperAdmin && role != UserRole.AccountsManager)
                throw new UnauthorizedAccessException("Only Super Admin or Accounts Manager can override dues.");

            if (hasDues && adminOverride && string.IsNullOrWhiteSpace(overrideReason))
                throw new ArgumentException("Override reason is required when dues exist.");

            long clearanceId = 0;
            if (!DbConfig.OfflineMode)
            {
                clearanceId = new tblExam().UpsertClearance(
                    studentId, examTermId, summary.NetDue, true,
                    adminOverride && hasDues, overrideReason, userId,
                    hasDues ? "Cleared with admin override" : "Cleared — no outstanding dues");
            }
            else
            {
                var store = OfflineStore.Instance;
                var existing = store.Clearances.FirstOrDefault(c => c.StudentId == studentId && c.ExamTermId == examTermId);
                if (existing == null)
                {
                    clearanceId = store.NextClearanceId++;
                    existing = new ExamClearanceDto { ClearanceId = clearanceId, StudentId = studentId, ExamTermId = examTermId };
                    store.Clearances.Add(existing);
                }
                else
                {
                    clearanceId = existing.ClearanceId;
                }

                existing.RegistrationNo = student.RegistrationNo;
                existing.FullName = student.FullName;
                existing.ClassName = student.ClassName;
                existing.RollNumber = student.RollNumber;
                existing.TermName = term.TermName;
                existing.DuesAtClearance = summary.NetDue;
                existing.IsCleared = true;
                existing.AdminOverride = adminOverride && hasDues;
                existing.OverrideReason = overrideReason;
            }

            return new ExamClearanceDto
            {
                ClearanceId = clearanceId,
                StudentId = student.StudentId,
                RegistrationNo = student.RegistrationNo,
                FullName = student.FullName,
                ClassName = student.ClassName,
                RollNumber = student.RollNumber,
                ExamTermId = term.ExamTermId,
                TermName = term.TermName,
                DuesAtClearance = summary.NetDue,
                IsCleared = true,
                AdminOverride = adminOverride && hasDues,
                OverrideReason = overrideReason
            };
        }

        /// <summary>
        /// Admit print eligibility:
        /// 1) Saved ExamClearances.IsCleared = true (including admin override from Clearance form) → allowed
        /// 2) Else no current dues → auto-clear and allow
        /// 3) Else blocked (must clear dues or verify with override on Clearance form)
        /// </summary>
        public ExamClearanceDto GetAdmitPrintEligibility(int studentId, int examTermId)
        {
            var saved = GetSavedClearance(studentId, examTermId);
            if (saved != null && saved.IsCleared)
                return saved;

            return VerifyClearance(studentId, examTermId, adminOverride: false);
        }

        public AdmitCardDto IssueAdmitCard(
            int studentId, int examTermId, int issuedByUserId,
            bool adminOverride = false, string? overrideReason = null, UserRole? role = null)
        {
            // Prefer existing clearance from Exam Clearance Verification.
            // Optional adminOverride here is unused by Admit Print UI (kept for API compatibility).
            ExamClearanceDto clearance;
            var saved = GetSavedClearance(studentId, examTermId);
            if (saved != null && saved.IsCleared)
            {
                clearance = saved;
            }
            else if (adminOverride)
            {
                clearance = VerifyClearance(studentId, examTermId, true, overrideReason, issuedByUserId, role);
            }
            else
            {
                clearance = VerifyClearance(studentId, examTermId, false, null, issuedByUserId, role);
            }

            if (!clearance.IsCleared)
                throw new InvalidOperationException(
                    $"Cannot issue admit card. Outstanding dues: ৳{clearance.DuesAtClearance:N2}. " +
                    "Clear dues, or verify with Admin Override on Exam Clearance Verification first.");

            var student = new StudentManager().GetById(studentId)!;
            var term = GetActiveTerms().First(t => t.ExamTermId == examTermId);
            var admitCardNo = $"AC-{examTermId:D2}-{student.RegistrationNo}";
            var barcode = $"{student.RegistrationNo}|{examTermId}|{DateTime.Now:yyyyMMdd}";

            long admitCardId = studentId * 100L + examTermId;
            if (!DbConfig.OfflineMode)
            {
                long? clearanceFk = clearance.ClearanceId > 0 ? clearance.ClearanceId : null;
                admitCardId = new tblExam().IssueAdmitCard(
                    studentId, examTermId, admitCardNo, barcode, issuedByUserId, clearanceFk);
            }

            return new AdmitCardDto
            {
                AdmitCardId = admitCardId,
                AdmitCardNo = admitCardNo,
                BarcodeValue = barcode,
                RegistrationNo = student.RegistrationNo,
                FullName = student.FullName,
                FatherName = student.FatherName,
                ClassName = student.ClassName,
                SectionName = student.SectionName,
                RollNumber = student.RollNumber,
                TermName = term.TermName,
                TimetableJson = term.TimetableJson,
                IssuedAt = DateTime.Now,
                PhotoPath = student.PhotoPath,
                FeeClearanceNote = clearance.AdminOverride
                    ? "Fee Clearance Verified: Issued with admin override (Exam Clearance)."
                    : "Fee Clearance Verified: All required tuition accounts verified."
            };
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<ExamTerm>> GetActiveTermsAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetActiveTerms());
        public System.Threading.Tasks.Task<ExamClearanceDto?> GetSavedClearanceAsync(int studentId, int examTermId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetSavedClearance(studentId, examTermId));
        public System.Threading.Tasks.Task<ExamClearanceDto> GetAdmitPrintEligibilityAsync(int studentId, int examTermId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetAdmitPrintEligibility(studentId, examTermId));
        public System.Threading.Tasks.Task<ExamClearanceDto> VerifyClearanceAsync(
            int studentId, int examTermId, bool adminOverride = false, string? overrideReason = null,
            int? userId = null, UserRole? role = null, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(VerifyClearance(studentId, examTermId, adminOverride, overrideReason, userId, role));
        public System.Threading.Tasks.Task<AdmitCardDto> IssueAdmitCardAsync(
            int studentId, int examTermId, int issuedByUserId, bool adminOverride = false,
            string? overrideReason = null, UserRole? role = null, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(IssueAdmitCard(studentId, examTermId, issuedByUserId, adminOverride, overrideReason, role));

    }
}
