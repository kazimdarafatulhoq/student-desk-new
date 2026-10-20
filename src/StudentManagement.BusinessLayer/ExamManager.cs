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
                clearanceId = studentId * 1000L + examTermId;
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

        public AdmitCardDto IssueAdmitCard(
            int studentId, int examTermId, int issuedByUserId,
            bool adminOverride = false, string? overrideReason = null, UserRole? role = null)
        {
            var clearance = VerifyClearance(studentId, examTermId, adminOverride, overrideReason, issuedByUserId, role);
            if (!clearance.IsCleared)
                throw new InvalidOperationException(
                    $"Cannot issue admit card. Outstanding dues: ৳{clearance.DuesAtClearance:N2}. " +
                    "Contact Madrasah Authority to clear dues. Admit card prints only when there is no due.");

            var student = new StudentManager().GetById(studentId)!;
            var term = GetActiveTerms().First(t => t.ExamTermId == examTermId);
            var admitCardNo = $"AC-{examTermId:D2}-{student.RegistrationNo}";
            var barcode = $"{student.RegistrationNo}|{examTermId}|{DateTime.Now:yyyyMMdd}";

            long admitCardId = studentId * 100L + examTermId;
            if (!DbConfig.OfflineMode)
            {
                admitCardId = new tblExam().IssueAdmitCard(
                    studentId, examTermId, admitCardNo, barcode, issuedByUserId, clearance.ClearanceId);
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
                    ? "Fee Clearance Verified: Issued with admin override."
                    : "Fee Clearance Verified: All required tuition accounts verified."
            };
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<ExamTerm>> GetActiveTermsAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetActiveTerms());
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
