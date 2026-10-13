using System;
using System.Configuration;

namespace StudentManagement.DataAccessLayer
{
    /// <summary>Reads connection string / OfflineMode from App.config (like DyeERP).</summary>
    public static class DbConfig
    {
        public static string ConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["StudentManagementDb"]?.ConnectionString
                         ?? ConfigurationManager.ConnectionStrings["StudentManagementSDB"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(cs))
                    throw new InvalidOperationException(
                        "Connection string 'StudentManagementDb' is missing in App.config.");
                return cs;
            }
        }

        public static bool OfflineMode
        {
            get
            {
                var value = ConfigurationManager.AppSettings["OfflineMode"];
                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool TestConnection()
        {
            if (OfflineMode) return false;
            try
            {
                using var con = new Microsoft.Data.SqlClient.SqlConnection(ConnectionString);
                con.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
