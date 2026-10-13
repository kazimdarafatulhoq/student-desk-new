using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
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
            Load += async (s, e) => await RefreshUsersAsync();
        }

        private async Task RefreshUsersAsync()
        {
            if (DesignTime.IsActive)
                return;

            var users = await Auth.GetUsersAsync();
            dgvUsers.Rows.Clear();
            foreach (var u in users)
            {
                dgvUsers.Rows.Add(u.UserId, u.Username, u.FullName, u.Role.ToString(), u.IsActive ? "Active" : "Disabled",
                    u.LastLoginAt?.ToString("dd-MMM-yyyy HH:mm") ?? "—");
            }
        }

        private async void btnCreate_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                await Auth.CreateUserAsync(new CreateUserRequest
                {
                    Username = txtUsername.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Password = txtPassword.Text,
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Role = (UserRole)cboRole.SelectedItem!,
                    CreatedBy = AppSession.Current?.Username
                });
                txtUsername.Clear();
                txtFullName.Clear();
                txtPassword.Clear();
                txtEmail.Clear();
                txtPhone.Clear();
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
