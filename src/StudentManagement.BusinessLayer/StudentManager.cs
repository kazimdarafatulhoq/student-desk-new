using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using StudentManagement.BusinessLayer.Offline;
using StudentManagement.BusinessLayer.Security;
using StudentManagement.DataAccessLayer;
using StudentManagement.DataAccessLayer.Data_Access_Methods;
using StudentManagement.UserObjects.Enums;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.BusinessLayer
{
    /// <summary>Business layer for students — same role as DyeERP CompanyManager.</summary>
    public class StudentManager
    {
        public List<StudentDto> SearchStudents(StudentSearchFilter filter)
        {
            if (DbConfig.OfflineMode)
            {
                IEnumerable<StudentDto> q = OfflineStore.Instance.Students;
                if (filter.ClassId is > 0) q = q.Where(s => s.ClassId == filter.ClassId.Value);
                if (filter.SectionId is > 0) q = q.Where(s => s.SectionId == filter.SectionId.Value);
                if (filter.Status.HasValue) q = q.Where(s => s.Status == filter.Status.Value);
                if (!string.IsNullOrWhiteSpace(filter.Query))
                {
                    var t = filter.Query.Trim();
                    q = q.Where(s =>
                        s.FullName.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.RegistrationNo.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.GuardianPhone.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.RollNumber.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                return q.ToList();
            }

            var objTbl = new tblStudent();
            var dt = objTbl.SearchStudents(
                filter.Query,
                filter.ClassId,
                filter.SectionId,
                filter.Status.HasValue ? (int?)filter.Status.Value : null);
            return MapStudents(dt);
        }

        public StudentDto? GetById(int studentId)
        {
            if (DbConfig.OfflineMode)
            {
                var s = OfflineStore.Instance.FindStudent(studentId);
                if (s != null) s.AgeDisplay = AgeCalculator.Format(s.DateOfBirth);
                return s;
            }

            var dt = new tblStudent().GetById(studentId);
            return dt.Rows.Count == 0 ? null : MapStudent(dt.Rows[0]);
        }

        public StudentDto? FindQuick(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            query = query.Trim();

            if (DbConfig.OfflineMode)
            {
                return OfflineStore.Instance.Students.FirstOrDefault(s =>
                    s.RegistrationNo.Equals(query, StringComparison.OrdinalIgnoreCase) ||
                    s.GuardianPhone == query);
            }

            var dt = new tblStudent().FindQuick(query);
            return dt.Rows.Count == 0 ? null : MapStudent(dt.Rows[0]);
        }

        public List<StudentDto> GetLookup()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.Students.Where(s => s.Status == StudentStatus.Active).ToList();

            return MapStudents(new tblStudent().GetLookup());
        }

        public StudentDto Admit(StudentAdmissionRequest request)
        {
            ValidateAdmission(request);

            if (DbConfig.OfflineMode)
            {
                var store = OfflineStore.Instance;
                var year = DateTime.Now.Year;
                var student = new StudentDto
                {
                    StudentId = store.NextStudentId++,
                    RegistrationNo = $"REG-{year}-{store.Students.Count + 1:D4}",
                    FullName = request.FullName.Trim(),
                    FatherName = request.FatherName.Trim(),
                    MotherName = request.MotherName?.Trim() ?? string.Empty,
                    BloodGroup = request.BloodGroup,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth.Date,
                    GuardianPhone = request.GuardianPhone.Trim(),
                    PresentAddress = request.PresentAddress,
                    PermanentAddress = request.PermanentAddress,
                    ClassId = request.ClassId,
                    ClassName = store.Classes.FirstOrDefault(c => c.ClassId == request.ClassId)?.ClassName ?? "",
                    SectionId = request.SectionId,
                    SectionName = store.Sections.FirstOrDefault(s => s.SectionId == request.SectionId)?.SectionName ?? "",
                    RollNumber = request.RollNumber.Trim(),
                    MonthlyTuitionFee = request.MonthlyTuitionFee,
                    AdmissionDate = DateTime.Today,
                    Status = StudentStatus.Active,
                    AcademicSession = request.AcademicSession,
                    PhotoPath = request.PhotoPath,
                    NetDue = request.MonthlyTuitionFee,
                    AgeDisplay = AgeCalculator.Format(request.DateOfBirth.Date)
                };
                store.Students.Add(student);
                store.Ledger.Add(new LedgerEntryDto
                {
                    LedgerId = store.NextLedgerId++,
                    StudentId = student.StudentId,
                    TransactionDate = DateTime.Now,
                    VoucherNo = $"INV-{DateTime.Now:yyyyMMdd}-{student.StudentId:D4}",
                    Particulars = $"Admission / Tuition Fee - {DateTime.Today:MMM-yyyy}",
                    FeePeriod = DateTime.Today.ToString("MMM-yyyy"),
                    DebitAmount = request.MonthlyTuitionFee,
                    RunningBalance = request.MonthlyTuitionFee,
                    Status = LedgerStatus.Open,
                    EntryType = LedgerEntryType.Invoice
                });
                return student;
            }

            var (id, _) = new tblStudent().AdmitStudent(request);
            return GetById(id) ?? throw new InvalidOperationException("Student was admitted but could not be reloaded.");
        }

        public void Update(StudentDto student, string? updatedBy = null)
        {
            if (student.StudentId <= 0) throw new ArgumentException("Student id is required.");
            if (string.IsNullOrWhiteSpace(student.FullName)) throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(student.FatherName)) throw new ArgumentException("Father's name is required.");
            if (string.IsNullOrWhiteSpace(student.GuardianPhone)) throw new ArgumentException("Guardian phone is required.");
            if (student.ClassId <= 0 || student.SectionId <= 0) throw new ArgumentException("Class and section are required.");
            if (string.IsNullOrWhiteSpace(student.RollNumber)) throw new ArgumentException("Roll number is required.");
            if (student.MonthlyTuitionFee < 0) throw new ArgumentException("Tuition fee cannot be negative.");

            if (DbConfig.OfflineMode)
            {
                var existing = OfflineStore.Instance.FindStudent(student.StudentId)
                               ?? throw new InvalidOperationException("Student not found.");
                existing.FullName = student.FullName.Trim();
                existing.FatherName = student.FatherName.Trim();
                existing.MotherName = student.MotherName?.Trim() ?? string.Empty;
                existing.BloodGroup = student.BloodGroup;
                existing.Gender = student.Gender;
                existing.DateOfBirth = student.DateOfBirth.Date;
                existing.GuardianPhone = student.GuardianPhone.Trim();
                existing.PresentAddress = student.PresentAddress;
                existing.PermanentAddress = student.PermanentAddress;
                existing.ClassId = student.ClassId;
                existing.ClassName = OfflineStore.Instance.Classes.FirstOrDefault(c => c.ClassId == student.ClassId)?.ClassName ?? existing.ClassName;
                existing.SectionId = student.SectionId;
                existing.SectionName = OfflineStore.Instance.Sections.FirstOrDefault(s => s.SectionId == student.SectionId)?.SectionName ?? existing.SectionName;
                existing.RollNumber = student.RollNumber.Trim();
                existing.MonthlyTuitionFee = student.MonthlyTuitionFee;
                existing.Status = student.Status;
                existing.PhotoPath = student.PhotoPath;
                if (!string.IsNullOrWhiteSpace(student.AcademicSession))
                    existing.AcademicSession = student.AcademicSession;
                return;
            }

            new tblStudent().UpdateStudent(student, updatedBy);
        }

        public void SetStatus(int studentId, StudentStatus status, string? updatedBy = null)
        {
            if (studentId <= 0) throw new ArgumentException("Student id is required.");

            if (DbConfig.OfflineMode)
            {
                var s = OfflineStore.Instance.FindStudent(studentId)
                        ?? throw new InvalidOperationException("Student not found.");
                s.Status = status;
                return;
            }

            if (GetById(studentId) == null)
                throw new InvalidOperationException("Student not found.");

            new tblStudent().SetStatus(studentId, status, updatedBy);
        }

        public void MarkInactive(int studentId, string? updatedBy = null)
            => SetStatus(studentId, StudentStatus.Inactive, updatedBy);

        public List<ClassDto> GetClasses()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.Classes.ToList();

            var dt = new tblAcademic().GetClasses();
            return dt.AsEnumerable().Select(r => new ClassDto
            {
                ClassId = r.Field<int>("ClassId"),
                ClassName = r.Field<string>("ClassName") ?? string.Empty,
                ClassCode = r.Table.Columns.Contains("ClassCode") ? r.Field<string?>("ClassCode") : null
            }).ToList();
        }

        public List<SectionDto> GetSections(int classId)
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.Sections.Where(s => s.ClassId == classId).ToList();

            var dt = new tblAcademic().GetSectionsByClass(classId);
            return dt.AsEnumerable().Select(r => new SectionDto
            {
                SectionId = r.Field<int>("SectionId"),
                ClassId = r.Field<int>("ClassId"),
                SectionName = r.Field<string>("SectionName") ?? string.Empty
            }).ToList();
        }

        private static void ValidateAdmission(StudentAdmissionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName)) throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.FatherName)) throw new ArgumentException("Father's name is required.");
            if (string.IsNullOrWhiteSpace(request.GuardianPhone)) throw new ArgumentException("Guardian phone is required.");
            if (request.ClassId <= 0 || request.SectionId <= 0) throw new ArgumentException("Class and section are required.");
            if (string.IsNullOrWhiteSpace(request.RollNumber)) throw new ArgumentException("Roll number is required.");
            if (request.MonthlyTuitionFee < 0) throw new ArgumentException("Tuition fee cannot be negative.");
            if (request.DateOfBirth.Date >= DateTime.Today) throw new ArgumentException("Date of birth must be in the past.");
        }

        private static List<StudentDto> MapStudents(DataTable dt)
            => dt.AsEnumerable().Select(MapStudent).ToList();

        private static StudentDto MapStudent(DataRow r)
        {
            var dob = r.Table.Columns.Contains("DateOfBirth") && r["DateOfBirth"] != DBNull.Value
                ? Convert.ToDateTime(r["DateOfBirth"])
                : DateTime.Today.AddYears(-10);

            return new StudentDto
            {
                StudentId = Convert.ToInt32(r["StudentId"]),
                RegistrationNo = Convert.ToString(r["RegistrationNo"]) ?? string.Empty,
                FullName = Convert.ToString(r["FullName"]) ?? string.Empty,
                FatherName = r.Table.Columns.Contains("FatherName") ? Convert.ToString(r["FatherName"]) ?? "" : "",
                MotherName = r.Table.Columns.Contains("MotherName") ? Convert.ToString(r["MotherName"]) ?? "" : "",
                BloodGroup = r.Table.Columns.Contains("BloodGroup") && r["BloodGroup"] != DBNull.Value
                    ? (BloodGroup)Convert.ToInt32(r["BloodGroup"]) : BloodGroup.OPositive,
                Gender = r.Table.Columns.Contains("Gender") && r["Gender"] != DBNull.Value
                    ? (Gender)Convert.ToInt32(r["Gender"]) : Gender.Other,
                DateOfBirth = dob,
                AgeDisplay = AgeCalculator.Format(dob),
                GuardianPhone = r.Table.Columns.Contains("GuardianPhone") ? Convert.ToString(r["GuardianPhone"]) ?? "" : "",
                PresentAddress = r.Table.Columns.Contains("PresentAddress") ? Convert.ToString(r["PresentAddress"]) : null,
                PermanentAddress = r.Table.Columns.Contains("PermanentAddress") ? Convert.ToString(r["PermanentAddress"]) : null,
                ClassId = r.Table.Columns.Contains("ClassId") && r["ClassId"] != DBNull.Value ? Convert.ToInt32(r["ClassId"]) : 0,
                ClassName = r.Table.Columns.Contains("ClassName") ? Convert.ToString(r["ClassName"]) ?? "" : "",
                SectionId = r.Table.Columns.Contains("SectionId") && r["SectionId"] != DBNull.Value ? Convert.ToInt32(r["SectionId"]) : 0,
                SectionName = r.Table.Columns.Contains("SectionName") ? Convert.ToString(r["SectionName"]) ?? "" : "",
                RollNumber = r.Table.Columns.Contains("RollNumber") ? Convert.ToString(r["RollNumber"]) ?? "" : "",
                MonthlyTuitionFee = r.Table.Columns.Contains("MonthlyTuitionFee") && r["MonthlyTuitionFee"] != DBNull.Value
                    ? Convert.ToDecimal(r["MonthlyTuitionFee"]) : 0,
                AdmissionDate = r.Table.Columns.Contains("AdmissionDate") && r["AdmissionDate"] != DBNull.Value
                    ? Convert.ToDateTime(r["AdmissionDate"]) : DateTime.Today,
                Status = r.Table.Columns.Contains("Status") && r["Status"] != DBNull.Value
                    ? (StudentStatus)Convert.ToInt32(r["Status"]) : StudentStatus.Active,
                AcademicSession = r.Table.Columns.Contains("AcademicSession")
                    ? Convert.ToString(r["AcademicSession"]) ?? "2025-2026" : "2025-2026",
                NetDue = r.Table.Columns.Contains("NetDue") && r["NetDue"] != DBNull.Value
                    ? Convert.ToDecimal(r["NetDue"]) : 0,
                PhotoPath = r.Table.Columns.Contains("PhotoPath") ? Convert.ToString(r["PhotoPath"]) : null
            };
        }

        // Async wrappers (WinForms can keep await; logic stays sync like DyeERP).
        public System.Threading.Tasks.Task<System.Collections.Generic.List<StudentDto>> SearchAsync(StudentSearchFilter filter, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(SearchStudents(filter));
        public System.Threading.Tasks.Task<StudentDto?> GetByIdAsync(int studentId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetById(studentId));
        public System.Threading.Tasks.Task<StudentDto?> FindQuickAsync(string query, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(FindQuick(query));
        public System.Threading.Tasks.Task<System.Collections.Generic.List<StudentDto>> GetLookupAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetLookup());
        public System.Threading.Tasks.Task<StudentDto> AdmitAsync(StudentAdmissionRequest request, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(Admit(request));
        public System.Threading.Tasks.Task UpdateAsync(StudentDto student, string? updatedBy = null, System.Threading.CancellationToken ct = default)
            { Update(student, updatedBy); return System.Threading.Tasks.Task.CompletedTask; }
        public System.Threading.Tasks.Task SetStatusAsync(int studentId, StudentStatus status, string? updatedBy = null, System.Threading.CancellationToken ct = default)
            { SetStatus(studentId, status, updatedBy); return System.Threading.Tasks.Task.CompletedTask; }
        public System.Threading.Tasks.Task MarkInactiveAsync(int studentId, string? updatedBy = null, System.Threading.CancellationToken ct = default)
            { MarkInactive(studentId, updatedBy); return System.Threading.Tasks.Task.CompletedTask; }
        public System.Threading.Tasks.Task<System.Collections.Generic.List<ClassDto>> GetClassesAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetClasses());
        public System.Threading.Tasks.Task<System.Collections.Generic.List<SectionDto>> GetSectionsAsync(int classId, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetSections(classId));

    }
}
