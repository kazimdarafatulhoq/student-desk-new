using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmMainMenu : Form
    {
        private Form? _activeChild;
        private bool _sidebarExpanded = true;
        private System.Windows.Forms.Timer? _clockTimer;

        public frmMainMenu()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;

            UITheme.ApplyForm(this);
            ApplyNavIcons();
            ApplySidebarLayout(true);
            _clockTimer = new System.Windows.Forms.Timer();
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();
            Load += frmMainMenu_Load;
            FormClosed += frmMainMenu_FormClosed;
        }

        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("dd-MMM-yyyy  HH:mm:ss");
        }

        private void frmMainMenu_FormClosed(object? sender, FormClosedEventArgs e)
        {
            ReleaseScopeAndTimer();
        }

        /// <summary>Safe cleanup for close / sign-out (never throws to the UI).</summary>
        private void ReleaseScopeAndTimer()
        {
            try
            {
                if (_clockTimer != null)
                {
                    _clockTimer.Stop();
                    _clockTimer.Dispose();
                    _clockTimer = null;
                }
            }
            catch
            {
                _clockTimer = null;
            }

        }

        private async void frmMainMenu_Load(object? sender, EventArgs e)
        {
            if (AppSession.Current == null)
            {
                Close();
                return;
            }

            lblLogo.Text = AppSession.InstitutionName.ToUpperInvariant();
            Text = AppSession.InstitutionName + " - Student Management";
            lblUserRole.Text = AppSession.Current.FullName + "  ·  " + AppSession.RoleDisplay;
            lblSession.Text = "Session: " + AppSession.Current.AcademicSession;
            lblProfileName.Text = AppSession.Current.FullName;
            lblProfileStatus.Text = "●  " + AppSession.RoleDisplay + " (Online)";
            lblProfileStatus.ForeColor = UITheme.Online;
            string initials = GetInitials(AppSession.Current.FullName);
            lblAvatar.Tag = initials;
            lblAvatar.Invalidate();
            ApplyRoleVisibility();
            await RefreshDbStatusAsync();
            OpenChild<frmDashboard>();
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "AD";
            string[] parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0]).ToUpperInvariant();
        }

        private void ApplyRoleVisibility()
        {
            if (AppSession.Current == null)
                return;
            UserRole role = AppSession.Current.Role;
            btnAdmission.Visible = AuthManager.CanAccess(role, "Admission");
            btnFeeCollection.Visible = AuthManager.CanAccess(role, "FeeCollection");
            btnLedger.Visible = AuthManager.CanAccess(role, "Ledger");
            btnExamClearance.Visible = AuthManager.CanAccess(role, "ExamClearance");
            btnAdmitCard.Visible = AuthManager.CanAccess(role, "AdmitCard");
            btnUsers.Visible = AuthManager.CanAccess(role, "Users");
        }

        private async System.Threading.Tasks.Task RefreshDbStatusAsync()
        {
            try
            {
                if (AppSession.OfflineMode)
                {
                    lblDbStatus.Text = "Database: Design Mode (Offline)";
                    lblDbStatus.ForeColor = UITheme.Warning;
                    lblSync.Text = "Ledger sync: Demo data";
                    return;
                }

                bool ok = await new AuthManager().TestConnectionAsync();
                lblDbStatus.Text = ok ? "Database: Connected" : "Database: Offline";
                lblDbStatus.ForeColor = ok ? UITheme.Success : UITheme.Danger;
                lblSync.Text = ok ? "Ledger sync: Live" : "Ledger sync: Paused";
            }
            catch
            {
                lblDbStatus.Text = "Database: Error";
                lblDbStatus.ForeColor = UITheme.Danger;
            }
        }

        private void OpenChild<T>() where T : Form, new()
        {
            Form form = new T();
            ShowInContent(form, typeof(T).Name);
            WireStudentDirectoryEvents(form as frmStudentSearch);
        }

        private void WireStudentDirectoryEvents(frmStudentSearch? search)
        {
            if (search == null)
                return;
            search.EditStudentRequested -= OpenStudentEditor;
            search.EditStudentRequested += OpenStudentEditor;
        }

        private void ShowInContent(Form form, string navFormName)
        {
            if (_activeChild != null)
            {
                pnlContentContainer.Controls.Remove(_activeChild);
                _activeChild.Dispose();
                _activeChild = null;
            }

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.BackColor = UITheme.Canvas;
            pnlContentContainer.Controls.Add(form);
            form.Show();
            _activeChild = form;
            HighlightNav(navFormName);
        }

        /// <summary>Open Edit Student Profile inside the main content panel (not a popup).</summary>
        private void OpenStudentEditor(int studentId)
        {
            if (studentId <= 0)
                return;

            frmStudentAdmission editor = new frmStudentAdmission(studentId);
            editor.EditCompleted += OnStudentEditCompleted;
            ShowInContent(editor, nameof(frmStudentAdmission));
            HighlightNav(nameof(frmStudentSearch));
        }

        private void OnStudentEditCompleted()
        {
            OpenChild<frmStudentSearch>();
        }

        private void HighlightNav(string formName)
        {
            foreach (Control host in pnlSidebar.Controls)
            {
                foreach (Control c in GetAllButtons(host))
                {
                    Button? b = c as Button;
                    if (b == null)
                        continue;
                    string tag = b.Tag?.ToString() ?? string.Empty;
                    if (tag == "nav" || tag == "nav-active")
                        UITheme.SetNavActive(b, false);
                }
            }

            Button? active = null;
            if (formName == nameof(frmDashboard)) active = btnDashboard;
            else if (formName == nameof(frmStudentAdmission)) active = btnAdmission;
            else if (formName == nameof(frmStudentSearch)) active = btnSearch;
            else if (formName == nameof(frmFeeCollection)) active = btnFeeCollection;
            else if (formName == nameof(frmStudentLedger)) active = btnLedger;
            else if (formName == nameof(frmExamClearance)) active = btnExamClearance;
            else if (formName == nameof(frmAdmitCardPrint)) active = btnAdmitCard;
            else if (formName == nameof(frmAppUser)) active = btnUsers;

            if (active != null)
                UITheme.SetNavActive(active, true);

            RefreshNavIconColors();
        }

        private void RefreshNavIconColors()
        {
            RecolorNav(btnToggleSidebar, UITheme.TextMuted);
            RecolorNav(btnDashboard, IsActive(btnDashboard) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnAdmission, IsActive(btnAdmission) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnSearch, IsActive(btnSearch) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnFeeCollection, IsActive(btnFeeCollection) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnLedger, IsActive(btnLedger) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnExamClearance, IsActive(btnExamClearance) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnAdmitCard, IsActive(btnAdmitCard) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnUsers, IsActive(btnUsers) ? Color.White : UITheme.TextMuted);
            RecolorNav(btnLogout, Color.White);
        }

        private static bool IsActive(Button button)
        {
            return (button.Tag?.ToString() ?? string.Empty) == "nav-active";
        }

        private void RecolorNav(Button button, Color color)
        {
            Image icon;
            if (button == btnToggleSidebar) icon = NavIcons.Toggle(color);
            else if (button == btnDashboard) icon = NavIcons.Dashboard(color);
            else if (button == btnAdmission) icon = NavIcons.Admission(color);
            else if (button == btnSearch) icon = NavIcons.Search(color);
            else if (button == btnFeeCollection) icon = NavIcons.Fee(color);
            else if (button == btnLedger) icon = NavIcons.Ledger(color);
            else if (button == btnExamClearance) icon = NavIcons.Exam(color);
            else if (button == btnAdmitCard) icon = NavIcons.AdmitCard(color);
            else if (button == btnUsers) icon = NavIcons.Users(color);
            else if (button == btnLogout) icon = NavIcons.Logout(color);
            else return;

            if (button.Image != null)
            {
                Image old = button.Image;
                button.Image = null;
                old.Dispose();
            }
            button.Image = icon;
        }

        private System.Collections.Generic.IEnumerable<Control> GetAllButtons(Control root)
        {
            yield return root;
            foreach (Control child in root.Controls)
            {
                foreach (Control nested in GetAllButtons(child))
                    yield return nested;
            }
        }

        private void ApplyNavIcons()
        {
            Color mute = UITheme.TextMuted;
            Color white = Color.White;

            AssignNav(btnToggleSidebar, NavIcons.Toggle(mute), "Collapse");
            AssignNav(btnDashboard, NavIcons.Dashboard(mute), "Dashboard");
            AssignNav(btnAdmission, NavIcons.Admission(mute), "Student Registration");
            AssignNav(btnSearch, NavIcons.Search(mute), "Student Directory");
            AssignNav(btnFeeCollection, NavIcons.Fee(mute), "Fee Collection Counter");
            AssignNav(btnLedger, NavIcons.Ledger(mute), "Student Ledger");
            AssignNav(btnExamClearance, NavIcons.Exam(mute), "Exam Fee Eligibility");
            AssignNav(btnAdmitCard, NavIcons.AdmitCard(mute), "Admit Card Printing");
            AssignNav(btnUsers, NavIcons.Users(mute), "User Access & Security");

            btnLogout.Tag = "nav-danger";
            AssignNav(btnLogout, NavIcons.Logout(white), "Sign Out");
            UITheme.StyleButton(btnLogout);
        }

        private static void AssignNav(Button button, Image icon, string label)
        {
            if (button.Image != null)
            {
                Image old = button.Image;
                button.Image = null;
                old.Dispose();
            }

            button.Image = icon;
            button.AccessibleName = label;
            button.Text = label;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Height = 50;
            button.UseCompatibleTextRendering = true;
        }

        private void ApplySidebarLayout(bool expanded)
        {
            _sidebarExpanded = expanded;
            pnlSidebar.Width = expanded ? 260 : 72;
            pnlProfile.Visible = expanded;

            foreach (Control host in pnlSidebar.Controls)
            {
                foreach (Control c in GetAllButtons(host))
                {
                    Button? b = c as Button;
                    if (b == null)
                        continue;

                    string tag = b.Tag?.ToString() ?? string.Empty;
                    bool isNav = tag == "nav" || tag == "nav-active" || tag == "nav-danger";
                    if (!isNav && b != btnToggleSidebar)
                        continue;

                    string label = b.AccessibleName ?? string.Empty;
                    if (expanded)
                    {
                        b.Width = 232;
                        b.Text = label;
                        b.TextAlign = ContentAlignment.MiddleLeft;
                        b.ImageAlign = ContentAlignment.MiddleLeft;
                        b.Padding = new Padding(12, 10, 8, 10);
                        b.TextImageRelation = TextImageRelation.ImageBeforeText;
                    }
                    else
                    {
                        // Icons stay visible; labels hide.
                        b.Width = 52;
                        b.Text = string.Empty;
                        b.TextAlign = ContentAlignment.MiddleCenter;
                        b.ImageAlign = ContentAlignment.MiddleCenter;
                        b.Padding = new Padding(0);
                        b.TextImageRelation = TextImageRelation.Overlay;
                    }

                    b.Height = 50;
                }

                foreach (Control child in host.Controls)
                {
                    Label? section = child as Label;
                    if (section != null && (section.Tag?.ToString() ?? string.Empty) == "muted")
                        section.Visible = expanded;
                }
            }

            // Refresh active/danger colors after layout Padding changes.
            foreach (Control host in pnlSidebar.Controls)
            {
                foreach (Control c in GetAllButtons(host))
                {
                    Button? b = c as Button;
                    if (b == null)
                        continue;
                    string tag = b.Tag?.ToString() ?? string.Empty;
                    if (tag == "nav" || tag == "nav-active" || tag == "nav-danger")
                        UITheme.StyleButton(b);
                }
            }

            // Keep collapsed icon-only alignment after StyleButton resets padding.
            if (!expanded)
            {
                foreach (Control host in pnlSidebar.Controls)
                {
                    foreach (Control c in GetAllButtons(host))
                    {
                        Button? b = c as Button;
                        if (b == null)
                            continue;
                        string tag = b.Tag?.ToString() ?? string.Empty;
                        if (tag != "nav" && tag != "nav-active" && tag != "nav-danger")
                            continue;
                        b.Text = string.Empty;
                        b.Width = 52;
                        b.Padding = new Padding(0);
                        b.ImageAlign = ContentAlignment.MiddleCenter;
                        b.TextAlign = ContentAlignment.MiddleCenter;
                        b.TextImageRelation = TextImageRelation.Overlay;
                    }
                }
            }
        }

        private void btnToggleSidebar_Click(object? sender, EventArgs e)
        {
            ApplySidebarLayout(!_sidebarExpanded);
        }

        private void btnDashboard_Click(object? sender, EventArgs e) { OpenChild<frmDashboard>(); }
        private void btnAdmission_Click(object? sender, EventArgs e) { OpenChild<frmStudentAdmission>(); }
        private void btnSearch_Click(object? sender, EventArgs e) => OpenChild<frmStudentSearch>();
        private void btnFeeCollection_Click(object? sender, EventArgs e) { OpenChild<frmFeeCollection>(); }
        private void btnLedger_Click(object? sender, EventArgs e) { OpenChild<frmStudentLedger>(); }
        private void btnExamClearance_Click(object? sender, EventArgs e) { OpenChild<frmExamClearance>(); }
        private void btnAdmitCard_Click(object? sender, EventArgs e) { OpenChild<frmAdmitCardPrint>(); }
        private void btnUsers_Click(object? sender, EventArgs e) { OpenChild<frmAppUser>(); }

        public void OpenFeeCollectionFromDashboard()
        {
            OpenChild<frmFeeCollection>();
        }

        private void btnLogout_Click(object? sender, EventArgs e)
        {
            AppSession.Clear();
            ReleaseScopeAndTimer();
            System.Windows.Forms.Application.Restart();
        }
    }
}
