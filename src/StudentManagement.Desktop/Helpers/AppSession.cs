using System;
using System.IO;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.Desktop.Helpers
{
    public static class AppSession
    {
        public static AuthSession? Current { get; private set; }
        public static string InstitutionName { get; set; } = "Ideal High School & College";
        public static string InstitutionCampus { get; set; } = "Dhanmondi Campus, Dhaka-1205";
        public static string ReportsPath { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StudentManagement", "Reports");

        /// <summary>When true, the app uses in-memory demo data (no SQL Server).</summary>
        public static bool OfflineMode { get; set; }

        public static bool IsAuthenticated => Current != null;

        public static void Set(AuthSession session) => Current = session;

        public static void Clear() => Current = null;

        public static string RoleDisplay => Current != null ? Current.Role.ToString() : "Guest";
    }
}
