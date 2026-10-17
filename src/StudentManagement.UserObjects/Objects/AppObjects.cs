using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagement.UserObjects.Enums;

namespace StudentManagement.UserObjects.Objects
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AgeDisplay { get; set; } = string.Empty;
        public string GuardianPhone { get; set; } = string.Empty;
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public decimal MonthlyTuitionFee { get; set; }
        public DateTime AdmissionDate { get; set; }
        public StudentStatus Status { get; set; }
        public string AcademicSession { get; set; } = "2025-2026";
        public decimal NetDue { get; set; }
        public string? PhotoPath { get; set; }

        /// <summary>Guardian requested school transport (Gari) — waitlisted until a seat is assigned.</summary>
        public bool WantTransport { get; set; }

        /// <summary>Transport seat assigned; monthly transport fee bills from the month after activation.</summary>
        public bool TransportActive { get; set; }

        public decimal MonthlyTransportFee { get; set; }

        /// <summary>Date seat was activated. Billing starts the following calendar month.</summary>
        public DateTime? TransportActivatedOn { get; set; }
    }

    public class StudentAdmissionRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string GuardianPhone { get; set; } = string.Empty;
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public decimal MonthlyTuitionFee { get; set; }
        public string AcademicSession { get; set; } = "2025-2026";
        public string? CreatedBy { get; set; }
        public string? PhotoPath { get; set; }

        /// <summary>Collect transport interest at admission (service stays inactive until seat assigned).</summary>
        public bool WantTransport { get; set; }

        public decimal MonthlyTransportFee { get; set; }
    }

    public class StudentSearchFilter
    {
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public string? Query { get; set; }
        public StudentStatus? Status { get; set; }
    }

    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? ClassCode { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SectionDto
    {
        public int SectionId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int Capacity { get; set; } = 40;
        public bool IsActive { get; set; } = true;
    }

    public class BloodGroupDto
    {
        public int BloodGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PaymentMethodDto
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UserRoleDto
    {
        public int UserRoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class LedgerSummaryDto
    {
        public int StudentId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal AdvanceBalance { get; set; }
        public decimal NetDue { get; set; }
    }

    public class LedgerEntryDto
    {
        public long LedgerId { get; set; }
        public int StudentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string Particulars { get; set; } = string.Empty;
        public string? FeePeriod { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
        public LedgerStatus Status { get; set; }
        public LedgerEntryType EntryType { get; set; }
    }

    public class FeeCollectionRequest
    {
        public int StudentId { get; set; }
        public List<string> FeePeriods { get; set; } = new();
        public decimal TuitionAmount { get; set; }
        public decimal FineAmount { get; set; }
        public decimal WaiverAmount { get; set; }
        public List<NamedFeeAmount> OtherFees { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? TransactionRef { get; set; }
        public string? Remarks { get; set; }
        public int CollectedByUserId { get; set; }
        public decimal OtherFeesTotal => OtherFees.Where(f => f.Amount > 0).Sum(f => f.Amount);
    }

    public class NamedFeeAmount
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class ReceiptLineItemDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsCredit { get; set; }
    }

    public class FeeCollectionResult
    {
        public long CollectionId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RegistrationNo { get; set; } = string.Empty;
        public string FeePeriods { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodDisplay { get; set; } = string.Empty;
        public List<ReceiptLineItemDto> LineItems { get; set; } = new();
    }

    public class UserAccountDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthSession
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string AcademicSession { get; set; } = "2025-2026";
        public DateTime LoginAt { get; set; } = DateTime.Now;
    }

    public class ExamTerm
    {
        public int ExamTermId { get; set; }
        public string TermName { get; set; } = string.Empty;
        public string AcademicSession { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? TimetableJson { get; set; }
    }

    public class ExamClearanceDto
    {
        public long ClearanceId { get; set; }
        public int StudentId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public int ExamTermId { get; set; }
        public string TermName { get; set; } = string.Empty;
        public decimal DuesAtClearance { get; set; }
        public bool IsCleared { get; set; }
        public bool AdminOverride { get; set; }
        public string? OverrideReason { get; set; }
    }

    public class AdmitCardDto
    {
        public long AdmitCardId { get; set; }
        public string AdmitCardNo { get; set; } = string.Empty;
        public string BarcodeValue { get; set; } = string.Empty;
        public string RegistrationNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string TermName { get; set; } = string.Empty;
        public string? TimetableJson { get; set; }
        public DateTime IssuedAt { get; set; }
        public string? PhotoPath { get; set; }
        public string Venue { get; set; } = "Main Auditorium & Hall 204";
        public string CampusAddress { get; set; } = "Fouzdarhat, Chittagong, Bangladesh, 4317";
        public string FeeClearanceNote { get; set; } = "Fee Clearance Verified: All required tuition accounts verified.";
    }

    public class DashboardStats
    {
        public int ActiveStudents { get; set; }
        public int ClearStudents { get; set; }
        public int StudentsWithDues { get; set; }
        public decimal TotalReceivables { get; set; }
        public int FeeCollectionRatePct { get; set; }
        public decimal CollectionsToday { get; set; }
        public decimal CollectionsThisMonth { get; set; }
    }
}
