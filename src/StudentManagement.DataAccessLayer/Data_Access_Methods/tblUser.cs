using System;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentManagement.UserObjects.Enums;

namespace StudentManagement.DataAccessLayer.Data_Access_Methods
{
    public class tblUser
    {
        public DataTable GetByUsername(string username)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT TOP 1 * FROM dbo.Users
WHERE Username = @Username AND IsDeleted = 0", con);
            cmd.Parameters.AddWithValue("@Username", username.Trim());
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetById(int userId)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT TOP 1 * FROM dbo.Users WHERE UserId = @UserId AND IsDeleted = 0", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetAll()
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT u.UserId, u.Username, u.FullName, u.Email, u.Phone, u.Role,
       ISNULL(r.Name, CAST(u.Role AS NVARCHAR(20))) AS RoleName,
       u.IsActive, u.LastLoginAt
FROM dbo.Users u
LEFT JOIN dbo.UserRoles r ON r.UserRoleId = u.Role AND r.IsDeleted = 0
WHERE u.IsDeleted = 0
ORDER BY u.Username", con);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetUserRoles()
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetUserRoles", ReturnType.DataTable, CommandType.StoredProcedure)!;
        }

        public void UpdateLastLogin(int userId)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(
                "UPDATE dbo.Users SET LastLoginAt = SYSUTCDATETIME() WHERE UserId = @UserId", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public int InsertUser(string username, string fullName, string passwordHash,
            string? email, string? phone, UserRole role, string? createdBy)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Users (Username, FullName, PasswordHash, Email, Phone, Role, IsActive, CreatedBy)
VALUES (@Username, @FullName, @PasswordHash, @Email, @Phone, @Role, 1, @CreatedBy);
SELECT CAST(SCOPE_IDENTITY() AS INT);", con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@FullName", fullName);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object?)phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Role", (int)role);
            cmd.Parameters.AddWithValue("@CreatedBy", (object?)createdBy ?? DBNull.Value);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void SetActive(int userId, bool isActive)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
UPDATE dbo.Users SET IsActive = @IsActive, UpdatedAt = SYSUTCDATETIME()
WHERE UserId = @UserId", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        /// <summary>Ensures demo users have working password hashes (first-run bootstrap).</summary>
        public void EnsureDemoPasswords(
            string adminHash, string accountsHash, string admissionHash, string teacherHash)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            con.Open();
            UpdateHash(con, "admin", adminHash);
            UpdateHash(con, "accounts", accountsHash);
            UpdateHash(con, "admission", admissionHash);
            UpdateHash(con, "teacher", teacherHash);
        }

        private static void UpdateHash(SqlConnection con, string username, string hash)
        {
            using var cmd = new SqlCommand(@"
UPDATE dbo.Users SET PasswordHash = @Hash
WHERE Username = @Username AND (PasswordHash LIKE 'SHA256$PLACEHOLDER%' OR PasswordHash IS NULL OR PasswordHash = '')", con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Hash", hash);
            cmd.ExecuteNonQuery();
        }
    }
}
