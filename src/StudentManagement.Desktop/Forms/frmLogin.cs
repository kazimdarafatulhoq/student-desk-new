using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmLogin : Form
    {
        private AuthManager? _auth;
        private AuthManager Auth => _auth ??= new AuthManager();

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmLogin()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            pnlCard.BackColor = UITheme.Canvas;
            AcceptButton = btnLogin;
            if (AppSession.OfflineMode)
            {
                lblHint.Text = "Design mode (no database) — use demo users / passwords";
                lblStatus.Text = "OfflineMode: SQL Server disconnected";
                lblStatus.ForeColor = UITheme.Warning;
            }
        }



        private async void btnLogin_Click(object? sender, EventArgs e)
        {
            try
            {
                btnLogin.Enabled = false;
                lblStatus.Text = "Authenticating...";
                lblStatus.ForeColor = UITheme.TextMuted;

                string user = txtUsername.Text.Trim();
                string pass = txtPassword.Text;

                AuthSession session = await Auth.LoginAsync(new LoginRequest
                {
                    Username = user,
                    Password = pass
                });

                AppSession.Set(session);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
                lblStatus.ForeColor = UITheme.Danger;
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}
