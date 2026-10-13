using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagement.UserObjects.Enums;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.BusinessLayer.Offline
{
    /// <summary>In-memory demo data when OfflineMode=true (UI without SQL Server).</summary>
    internal sealed class OfflineStore
    {
        public static readonly OfflineStore Instance = new OfflineStore();

        public List<StudentDto> Students { get; } = new();
        public List<ClassDto> Classes { get; } = new();
        public List<SectionDto> Sections { get; } = new();
        public List<LedgerEntryDto> Ledger { get; } = new();
        public List<UserAccountDto> Users { get; } = new();
        public List<ExamTerm> Terms { get; } = new();
        public List<FeeCollectionResult> Payments { get; } = new();
        public long NextLedgerId = 1000;
        public long NextPaymentId = 1000;
        public int NextStudentId = 10;
        public int NextUserId = 10;

        private OfflineStore()
        {
            Classes.AddRange(new[]
            {
                new ClassDto { ClassId = 1, ClassName = "Class 6", ClassCode = "C6" },
                new ClassDto { ClassId = 2, ClassName = "Class 7", ClassCode = "C7" },
                new ClassDto { ClassId = 3, ClassName = "Class 8", ClassCode = "C8" },
                new ClassDto { ClassId = 4, ClassName = "Class 9", ClassCode = "C9" },
                new ClassDto { ClassId = 5, ClassName = "Class 10", ClassCode = "C10" }
            });
            foreach (var c in Classes)
            {
                Sections.Add(new SectionDto { SectionId = c.ClassId * 10 + 1, ClassId = c.ClassId, SectionName = "A" });
                Sections.Add(new SectionDto { SectionId = c.ClassId * 10 + 2, ClassId = c.ClassId, SectionName = "B" });
            }

            Students.Add(new StudentDto
            {
                StudentId = 1, RegistrationNo = "REG-2026-0001", FullName = "Ayesha Rahman",
                FatherName = "Karim Rahman", MotherName = "Salma Rahman", BloodGroup = BloodGroup.BPositive,
                Gender = Gender.Female, DateOfBirth = new DateTime(2012, 5, 14), GuardianPhone = "01711000001",
                ClassId = 5, ClassName = "Class 10", SectionId = 51, SectionName = "A", RollNumber = "01",
                MonthlyTuitionFee = 3500, AdmissionDate = new DateTime(2024, 1, 10), Status = StudentStatus.Active,
                NetDue = 3500, PresentAddress = "Dhanmondi, Dhaka"
            });
            Students.Add(new StudentDto
            {
                StudentId = 2, RegistrationNo = "REG-2026-0002", FullName = "Rahim Uddin",
                FatherName = "Jalal Uddin", MotherName = "Nasrin Akter", BloodGroup = BloodGroup.OPositive,
                Gender = Gender.Male, DateOfBirth = new DateTime(2013, 8, 2), GuardianPhone = "01711000002",
                ClassId = 4, ClassName = "Class 9", SectionId = 41, SectionName = "A", RollNumber = "05",
                MonthlyTuitionFee = 3200, AdmissionDate = new DateTime(2024, 2, 1), Status = StudentStatus.Active,
                NetDue = 0
            });

            Ledger.Add(new LedgerEntryDto
            {
                LedgerId = NextLedgerId++, StudentId = 1, TransactionDate = DateTime.Today.AddDays(-5),
                VoucherNo = "INV-DEMO-1", Particulars = "Tuition Fee - Jan-2026", FeePeriod = "Jan-2026",
                DebitAmount = 3500, CreditAmount = 0, RunningBalance = 3500,
                Status = LedgerStatus.Open, EntryType = LedgerEntryType.Invoice
            });

            Users.Add(new UserAccountDto
            {
                UserId = 1, Username = "admin", FullName = "System Super Admin",
                Role = UserRole.SuperAdmin, IsActive = true, PasswordHash = "offline"
            });
            Users.Add(new UserAccountDto
            {
                UserId = 2, Username = "accounts", FullName = "Accounts Manager",
                Role = UserRole.AccountsManager, IsActive = true, PasswordHash = "offline"
            });
            Users.Add(new UserAccountDto
            {
                UserId = 3, Username = "admission", FullName = "Admission Officer",
                Role = UserRole.AdmissionOfficer, IsActive = true, PasswordHash = "offline"
            });
            Users.Add(new UserAccountDto
            {
                UserId = 4, Username = "teacher", FullName = "Class Teacher",
                Role = UserRole.Teacher, IsActive = true, PasswordHash = "offline"
            });

            Terms.Add(new ExamTerm
            {
                ExamTermId = 1, TermName = "Final Exam 2026", AcademicSession = "2025-2026",
                StartDate = new DateTime(2026, 11, 1), EndDate = new DateTime(2026, 11, 20),
                TimetableJson = "[{\"day\":\"Day 1\",\"subject\":\"Bangla\"},{\"day\":\"Day 2\",\"subject\":\"English\"}]"
            });
            Terms.Add(new ExamTerm
            {
                ExamTermId = 2, TermName = "Half-Yearly 2026", AcademicSession = "2025-2026",
                StartDate = new DateTime(2026, 6, 1), EndDate = new DateTime(2026, 6, 15)
            });
        }

        public StudentDto? FindStudent(int id) => Students.FirstOrDefault(s => s.StudentId == id);
    }
}
