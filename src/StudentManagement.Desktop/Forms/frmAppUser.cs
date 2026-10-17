using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmAppUser : Form
    {
        private AuthManager? _auth;
        private AuthManager Auth => _auth ??= new AuthManager();

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmAppUser()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            Load += async (s, e) =>
            {
                await BindRolesAsync();
                await RefreshUsersAsync();
            };
        }

        private async Task BindRolesAsync()
        {
            if (DesignTime.IsActive)
                return;

            var roles = await Auth.GetUserRolesAsync();
            cboRole.DisplayMember = nameof(UserRoleDto.Name);
            cboRole.ValueMember = nameof(UserRoleDto.UserRoleId);
            cboRole.DataSource = roles.ToList();
            if (cboRole.Items.Count > 0)
                cboRole.SelectedIndex = 0;
        }

        private async Task RefreshUsersAsync()
        {
            if (DesignTime.IsActive)
                return;

            var users = await Auth.GetUsersAsync();
            dgvUsers.Rows.Clear();
            foreach (var u in users)
            {
                var roleLabel = string.IsNullOrWhiteSpace(u.RoleName)
                    ? EnumDisplay.UserRoleLabel(u.Role)
                    : u.RoleName;
                dgvUsers.Rows.Add(u.UserId, u.Username, u.FullName, roleLabel, u.IsActive ? "Active" : "Disabled",
                    u.LastLoginAt?.ToString("dd-MMM-yyyy HH:mm") ?? "—");
            }
        }

        private UserRole ReadSelectedRole()
        {
            if (cboRole.SelectedValue != null)
                return (UserRole)Convert.ToInt32(cboRole.SelectedValue);
            return UserRole.Teacher;
        }

        private async void btnCreate_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                if (cboRole.SelectedValue == null)
                {
                    MessageBox.Show("Select a role.", "User Management", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await Auth.CreateUserAsync(new CreateUserRequest
                {
                    Username = txtUsername.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Password = txtPassword.Text,
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Role = ReadSelectedRole(),
                    CreatedBy = AppSession.Current?.Username
                });
                txtUsername.Clear();
                txtFullName.Clear();
                txtPassword.Clear();
                txtEmail.Clear();
                txtPhone.Clear();
                if (cboRole.Items.Count > 0)
                    cboRole.SelectedIndex = 0;
                await RefreshUsersAsync();
                MessageBox.Show("User created successfully.", "User Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnToggle_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive || dgvUsers.CurrentRow is null)
                return;
            var userId = Convert.ToInt32(dgvUsers.CurrentRow.Cells[0].Value);
            var active = dgvUsers.CurrentRow.Cells[4].Value?.ToString() == "Active";
            await Auth.SetActiveAsync(userId, !active);
            await RefreshUsersAsync();
        }
    }
}
