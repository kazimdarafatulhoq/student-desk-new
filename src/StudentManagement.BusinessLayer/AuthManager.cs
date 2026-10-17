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
    public class AuthManager
    {
        public AuthSession Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new UnauthorizedAccessException("Username and password are required.");

            if (DbConfig.OfflineMode)
            {
                var user = OfflineStore.Instance.Users.FirstOrDefault(u =>
                    u.Username.Equals(request.Username.Trim(), StringComparison.OrdinalIgnoreCase) && u.IsActive)
                    ?? throw new UnauthorizedAccessException("Invalid username or password.");

                var ok =
                    (user.Username == "admin" && request.Password == "Admin@123") ||
                    (user.Username == "accounts" && request.Password == "Accounts@123") ||
                    (user.Username == "admission" && request.Password == "Admission@123") ||
                    (user.Username == "teacher" && request.Password == "Teacher@123");
                if (!ok)
                    throw new UnauthorizedAccessException("Invalid username or password.");

                return new AuthSession
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role,
                    RoleName = ResolveRoleName(user.Role),
                    AcademicSession = "2025-2026",
                    LoginAt = DateTime.Now
                };
            }

            EnsureDemoPasswords();

            var dt = new tblUser().GetByUsername(request.Username.Trim());
            if (dt.Rows.Count == 0)
                throw new UnauthorizedAccessException("Invalid username or password.");

            var row = dt.Rows[0];
            var hash = Convert.ToString(row["PasswordHash"]) ?? "";
            var isActive = Convert.ToBoolean(row["IsActive"]);
            if (!isActive || !PasswordHasher.Verify(request.Password, hash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            var userId = Convert.ToInt32(row["UserId"]);
            var role = (UserRole)Convert.ToInt32(row["Role"]);
            new tblUser().UpdateLastLogin(userId);

            return new AuthSession
            {
                UserId = userId,
                Username = Convert.ToString(row["Username"]) ?? "",
                FullName = Convert.ToString(row["FullName"]) ?? "",
                Role = role,
                RoleName = ResolveRoleName(role),
                AcademicSession = "2025-2026",
                LoginAt = DateTime.Now
            };
        }

        public List<UserRoleDto> GetUserRoles()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.UserRoles.Where(r => r.IsActive).OrderBy(r => r.SortOrder).ToList();

            return new tblUser().GetUserRoles().AsEnumerable().Select(r => new UserRoleDto
            {
                UserRoleId = Convert.ToInt32(r["UserRoleId"]),
                Name = Convert.ToString(r["Name"]) ?? string.Empty,
                Code = Convert.ToString(r["Code"]) ?? string.Empty,
                SortOrder = r.Table.Columns.Contains("SortOrder") && r["SortOrder"] != DBNull.Value
                    ? Convert.ToInt32(r["SortOrder"]) : 0,
                IsActive = !r.Table.Columns.Contains("IsActive") || r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"])
            }).ToList();
        }

        public string ResolveRoleName(UserRole role)
        {
            try
            {
                var match = GetUserRoles().FirstOrDefault(r => r.UserRoleId == (int)role);
                if (match != null && !string.IsNullOrWhiteSpace(match.Name))
                    return match.Name;
            }
            catch
            {
                // Fall back to enum label if lookup table is unavailable.
            }

            return EnumDisplay.UserRoleLabel(role);
        }

        public UserAccountDto CreateUser(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required.");

            if (DbConfig.OfflineMode)
            {
                if (OfflineStore.Instance.Users.Any(u =>
                        u.Username.Equals(request.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Username already exists.");

                var user = new UserAccountDto
                {
                    UserId = OfflineStore.Instance.NextUserId++,
                    Username = request.Username.Trim(),
                    FullName = request.FullName.Trim(),
                    Email = request.Email,
                    Phone = request.Phone,
                    Role = request.Role,
                    RoleName = ResolveRoleName(request.Role),
                    IsActive = true,
                    PasswordHash = PasswordHasher.Hash(request.Password)
                };
                OfflineStore.Instance.Users.Add(user);
                return user;
            }

            var existing = new tblUser().GetByUsername(request.Username.Trim());
            if (existing.Rows.Count > 0)
                throw new InvalidOperationException("Username already exists.");

            var id = new tblUser().InsertUser(
                request.Username.Trim(), request.FullName.Trim(),
                PasswordHasher.Hash(request.Password),
                request.Email, request.Phone, request.Role, request.CreatedBy);

            return GetUsers().First(u => u.UserId == id);
        }

        public List<UserAccountDto> GetUsers()
        {
            if (DbConfig.OfflineMode)
                return OfflineStore.Instance.Users.Select(u => new UserAccountDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    RoleName = string.IsNullOrWhiteSpace(u.RoleName) ? ResolveRoleName(u.Role) : u.RoleName,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt
                }).ToList();

            var dt = new tblUser().GetAll();
            return dt.AsEnumerable().Select(r =>
            {
                var role = (UserRole)Convert.ToInt32(r["Role"]);
                var roleName = r.Table.Columns.Contains("RoleName")
                    ? Convert.ToString(r["RoleName"])
                    : null;
                return new UserAccountDto
                {
                    UserId = Convert.ToInt32(r["UserId"]),
                    Username = Convert.ToString(r["Username"]) ?? "",
                    FullName = Convert.ToString(r["FullName"]) ?? "",
                    Email = r.Table.Columns.Contains("Email") ? Convert.ToString(r["Email"]) : null,
                    Phone = r.Table.Columns.Contains("Phone") ? Convert.ToString(r["Phone"]) : null,
                    Role = role,
                    RoleName = string.IsNullOrWhiteSpace(roleName) ? EnumDisplay.UserRoleLabel(role) : roleName!,
                    IsActive = Convert.ToBoolean(r["IsActive"]),
                    LastLoginAt = r.Table.Columns.Contains("LastLoginAt") && r["LastLoginAt"] != DBNull.Value
                        ? Convert.ToDateTime(r["LastLoginAt"]) : null
                };
            }).ToList();
        }

        public void SetActive(int userId, bool isActive)
        {
            if (DbConfig.OfflineMode)
            {
                var u = OfflineStore.Instance.Users.FirstOrDefault(x => x.UserId == userId)
                        ?? throw new InvalidOperationException("User not found.");
                u.IsActive = isActive;
                return;
            }

            new tblUser().SetActive(userId, isActive);
        }

        public static bool CanAccess(UserRole role, string module)
        {
            return module switch
            {
                "Dashboard" => true,
                "Admission" => role is UserRole.SuperAdmin or UserRole.AdmissionOfficer,
                "Search" => true,
                "FeeCollection" => role is UserRole.SuperAdmin or UserRole.AccountsManager,
                "Ledger" => role is UserRole.SuperAdmin or UserRole.AccountsManager or UserRole.AdmissionOfficer,
                "ExamClearanceDto" => role is UserRole.SuperAdmin or UserRole.AccountsManager or UserRole.Teacher,
                "AdmitCard" => role is UserRole.SuperAdmin or UserRole.AccountsManager or UserRole.Teacher,
                "ClassSetup" => role is UserRole.SuperAdmin or UserRole.AdmissionOfficer,
                "SectionSetup" => role is UserRole.SuperAdmin or UserRole.AdmissionOfficer,
                "Users" => role is UserRole.SuperAdmin,
                _ => role == UserRole.SuperAdmin
            };
        }

        public void EnsureDemoPasswords()
        {
            if (DbConfig.OfflineMode) return;
            try
            {
                new tblUser().EnsureDemoPasswords(
                    PasswordHasher.Hash("Admin@123"),
                    PasswordHasher.Hash("Accounts@123"),
                    PasswordHasher.Hash("Admission@123"),
                    PasswordHasher.Hash("Teacher@123"));
            }
            catch
            {
                // Bootstrap is best-effort.
            }
        }

        public bool TestConnection() => DbConfig.TestConnection();

        public System.Threading.Tasks.Task<AuthSession> LoginAsync(LoginRequest request, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(Login(request));
        public System.Threading.Tasks.Task<UserAccountDto> CreateUserAsync(CreateUserRequest request, System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(CreateUser(request));
        public System.Threading.Tasks.Task<System.Collections.Generic.List<UserAccountDto>> GetUsersAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetUsers());
        public System.Threading.Tasks.Task<System.Collections.Generic.List<UserRoleDto>> GetUserRolesAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(GetUserRoles());
        public System.Threading.Tasks.Task SetActiveAsync(int userId, bool isActive, System.Threading.CancellationToken ct = default)
            { SetActive(userId, isActive); return System.Threading.Tasks.Task.CompletedTask; }
        public System.Threading.Tasks.Task<bool> TestConnectionAsync(System.Threading.CancellationToken ct = default)
            => System.Threading.Tasks.Task.FromResult(TestConnection());

    }
}
