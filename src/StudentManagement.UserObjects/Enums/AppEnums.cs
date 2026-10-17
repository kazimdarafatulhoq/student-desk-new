namespace StudentManagement.UserObjects.Enums
{
    public enum BloodGroup
    {
        APositive = 1, ANegative = 2, BPositive = 3, BNegative = 4,
        ABPositive = 5, ABNegative = 6, OPositive = 7, ONegative = 8
    }

    public enum Gender { Male = 1, Female = 2, Other = 3 }

    public enum StudentStatus
    {
        Active = 1, Inactive = 2, Graduated = 3, Transferred = 4, Suspended = 5
    }

    public enum PaymentMethod
    {
        Cash = 1, Bkash = 2, Nagad = 3, BankTransfer = 4
    }

    public enum UserRole
    {
        SuperAdmin = 1, AccountsManager = 2, AdmissionOfficer = 3, Teacher = 4
    }

    public enum LedgerStatus
    {
        Open = 1, Paid = 2, Partial = 3, Waived = 4, Cancelled = 5
    }

    public enum LedgerEntryType
    {
        Invoice = 1, Payment = 2, Fine = 3, Waiver = 4, Adjustment = 5, Advance = 6
    }

    public static class EnumDisplay
    {
        public static string BloodGroupLabel(BloodGroup value) => value switch
        {
            BloodGroup.APositive => "A+",
            BloodGroup.ANegative => "A-",
            BloodGroup.BPositive => "B+",
            BloodGroup.BNegative => "B-",
            BloodGroup.ABPositive => "AB+",
            BloodGroup.ABNegative => "AB-",
            BloodGroup.OPositive => "O+",
            BloodGroup.ONegative => "O-",
            _ => value.ToString()
        };

        public static string PaymentMethodLabel(PaymentMethod value) => value switch
        {
            PaymentMethod.Cash => "Cash",
            PaymentMethod.Bkash => "bKash",
            PaymentMethod.Nagad => "Nagad",
            PaymentMethod.BankTransfer => "Bank Transfer",
            _ => value.ToString()
        };

        public static string UserRoleLabel(UserRole value) => value switch
        {
            UserRole.SuperAdmin => "Super Admin",
            UserRole.AccountsManager => "Accounts Manager",
            UserRole.AdmissionOfficer => "Admission Officer",
            UserRole.Teacher => "Teacher",
            _ => value.ToString()
        };
    }
}
