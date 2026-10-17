using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    partial class frmMainMenu
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContentContainer;
        private Panel pnlProfile;
        private FlowLayoutPanel navFlow;
        private StatusStrip statusBar;
        private ToolStripStatusLabel lblDbStatus;
        private ToolStripStatusLabel lblSync;
        private Label lblLogo;
        private Label lblUserRole;
        private Label lblSession;
        private Label lblClock;
        private Label lblAvatar;
        private Label lblProfileName;
        private Label lblProfileStatus;
        private Label lblSecOps;
        private Label lblSecFees;
        private Label lblSecExam;
        private Label lblSecSystem;
        private Button btnToggleSidebar;
        private Button btnDashboard;
        private Button btnAdmission;
        private Button btnSearch;
        private Button btnFeeCollection;
        private Button btnLedger;
        private Button btnExamClearance;
        private Button btnAdmitCard;
        private Button btnClass;
        private Button btnSection;
        private Button btnUsers;
        private Button btnLogout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.navFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlProfile = new System.Windows.Forms.Panel();
            this.lblAvatar = new System.Windows.Forms.Label();
            this.lblProfileName = new System.Windows.Forms.Label();
            this.lblProfileStatus = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblLogo = new System.Windows.Forms.Label();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.lblSession = new System.Windows.Forms.Label();
            this.lblClock = new System.Windows.Forms.Label();
            this.pnlContentContainer = new System.Windows.Forms.Panel();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblDbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSync = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnToggleSidebar = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnAdmission = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnFeeCollection = new System.Windows.Forms.Button();
            this.btnLedger = new System.Windows.Forms.Button();
            this.btnExamClearance = new System.Windows.Forms.Button();
            this.btnAdmitCard = new System.Windows.Forms.Button();
            this.btnClass = new System.Windows.Forms.Button();
            this.btnSection = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblSecOps = new System.Windows.Forms.Label();
            this.lblSecFees = new System.Windows.Forms.Label();
            this.lblSecExam = new System.Windows.Forms.Label();
            this.lblSecSystem = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            this.navFlow.SuspendLayout();
            this.pnlProfile.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlSidebar
            //
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(20)))));
            this.pnlSidebar.Controls.Add(this.navFlow);
            this.pnlSidebar.Controls.Add(this.pnlProfile);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.pnlSidebar.Size = new System.Drawing.Size(300, 700);
            this.pnlSidebar.TabIndex = 0;
            this.pnlSidebar.Tag = "sidebar";
            //
            // pnlProfile
            //
            this.pnlProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(20)))));
            this.pnlProfile.Controls.Add(this.lblAvatar);
            this.pnlProfile.Controls.Add(this.lblProfileName);
            this.pnlProfile.Controls.Add(this.lblProfileStatus);
            this.pnlProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProfile.Location = new System.Drawing.Point(10, 12);
            this.pnlProfile.Name = "pnlProfile";
            this.pnlProfile.Padding = new System.Windows.Forms.Padding(8, 4, 8, 8);
            this.pnlProfile.Size = new System.Drawing.Size(280, 72);
            this.pnlProfile.TabIndex = 0;
            //
            // lblAvatar
            //
            this.lblAvatar.BackColor = System.Drawing.Color.Transparent;
            this.lblAvatar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAvatar.ForeColor = System.Drawing.Color.White;
            this.lblAvatar.Location = new System.Drawing.Point(8, 10);
            this.lblAvatar.Name = "lblAvatar";
            this.lblAvatar.Size = new System.Drawing.Size(44, 44);
            this.lblAvatar.TabIndex = 0;
            this.lblAvatar.Tag = "AD";
            this.lblAvatar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAvatar.Paint += new System.Windows.Forms.PaintEventHandler(this.Avatar_Paint);
            //
            // lblProfileName
            //
            this.lblProfileName.AutoSize = true;
            this.lblProfileName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblProfileName.ForeColor = System.Drawing.Color.White;
            this.lblProfileName.Location = new System.Drawing.Point(60, 12);
            this.lblProfileName.Name = "lblProfileName";
            this.lblProfileName.Size = new System.Drawing.Size(100, 19);
            this.lblProfileName.TabIndex = 1;
            this.lblProfileName.Text = "Administrator";
            //
            // lblProfileStatus
            //
            this.lblProfileStatus.AutoSize = true;
            this.lblProfileStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblProfileStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblProfileStatus.Location = new System.Drawing.Point(60, 36);
            this.lblProfileStatus.Name = "lblProfileStatus";
            this.lblProfileStatus.Size = new System.Drawing.Size(150, 15);
            this.lblProfileStatus.TabIndex = 2;
            this.lblProfileStatus.Text = "Administrator (Online)";

            this.btnToggleSidebar.AccessibleName = "Collapse";
            this.btnToggleSidebar.BackColor = System.Drawing.Color.Transparent;
            this.btnToggleSidebar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleSidebar.FlatAppearance.BorderSize = 0;
            this.btnToggleSidebar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnToggleSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSidebar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnToggleSidebar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnToggleSidebar.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnToggleSidebar.Name = "btnToggleSidebar";
            this.btnToggleSidebar.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnToggleSidebar.Size = new System.Drawing.Size(272, 50);
            this.btnToggleSidebar.TabIndex = 0;
            this.btnToggleSidebar.Tag = "nav";
            this.btnToggleSidebar.Text = "Collapse";
            this.btnToggleSidebar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToggleSidebar.Click += new System.EventHandler(this.btnToggleSidebar_Click);
            this.btnDashboard.AccessibleName = "Dashboard";
            this.btnDashboard.BackColor = System.Drawing.Color.Transparent;
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnDashboard.Size = new System.Drawing.Size(272, 50);
            this.btnDashboard.TabIndex = 1;
            this.btnDashboard.Tag = "nav";
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            this.btnAdmission.AccessibleName = "Student Registration";
            this.btnAdmission.BackColor = System.Drawing.Color.Transparent;
            this.btnAdmission.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdmission.FlatAppearance.BorderSize = 0;
            this.btnAdmission.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnAdmission.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdmission.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnAdmission.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnAdmission.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnAdmission.Name = "btnAdmission";
            this.btnAdmission.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnAdmission.Size = new System.Drawing.Size(272, 50);
            this.btnAdmission.TabIndex = 2;
            this.btnAdmission.Tag = "nav";
            this.btnAdmission.Text = "Student Registration";
            this.btnAdmission.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdmission.Click += new System.EventHandler(this.btnAdmission_Click);
            this.btnSearch.AccessibleName = "Student Directory";
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnSearch.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnSearch.Size = new System.Drawing.Size(272, 50);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Tag = "nav";
            this.btnSearch.Text = "Student Directory";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnFeeCollection.AccessibleName = "Fee Collection Counter";
            this.btnFeeCollection.BackColor = System.Drawing.Color.Transparent;
            this.btnFeeCollection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFeeCollection.FlatAppearance.BorderSize = 0;
            this.btnFeeCollection.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnFeeCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFeeCollection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnFeeCollection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnFeeCollection.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnFeeCollection.Name = "btnFeeCollection";
            this.btnFeeCollection.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnFeeCollection.Size = new System.Drawing.Size(272, 50);
            this.btnFeeCollection.TabIndex = 4;
            this.btnFeeCollection.Tag = "nav";
            this.btnFeeCollection.Text = "Fee Collection Counter";
            this.btnFeeCollection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFeeCollection.Click += new System.EventHandler(this.btnFeeCollection_Click);
            this.btnLedger.AccessibleName = "Student Ledger";
            this.btnLedger.BackColor = System.Drawing.Color.Transparent;
            this.btnLedger.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLedger.FlatAppearance.BorderSize = 0;
            this.btnLedger.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnLedger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLedger.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnLedger.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnLedger.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnLedger.Name = "btnLedger";
            this.btnLedger.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnLedger.Size = new System.Drawing.Size(272, 50);
            this.btnLedger.TabIndex = 5;
            this.btnLedger.Tag = "nav";
            this.btnLedger.Text = "Student Ledger";
            this.btnLedger.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLedger.Click += new System.EventHandler(this.btnLedger_Click);
            this.btnExamClearance.AccessibleName = "Exam Fee Eligibility";
            this.btnExamClearance.BackColor = System.Drawing.Color.Transparent;
            this.btnExamClearance.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExamClearance.FlatAppearance.BorderSize = 0;
            this.btnExamClearance.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnExamClearance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExamClearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnExamClearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnExamClearance.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnExamClearance.Name = "btnExamClearance";
            this.btnExamClearance.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnExamClearance.Size = new System.Drawing.Size(272, 50);
            this.btnExamClearance.TabIndex = 6;
            this.btnExamClearance.Tag = "nav";
            this.btnExamClearance.Text = "Exam Fee Eligibility";
            this.btnExamClearance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExamClearance.Click += new System.EventHandler(this.btnExamClearance_Click);
            this.btnAdmitCard.AccessibleName = "Admit Card Printing";
            this.btnAdmitCard.BackColor = System.Drawing.Color.Transparent;
            this.btnAdmitCard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdmitCard.FlatAppearance.BorderSize = 0;
            this.btnAdmitCard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnAdmitCard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdmitCard.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnAdmitCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnAdmitCard.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnAdmitCard.Name = "btnAdmitCard";
            this.btnAdmitCard.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnAdmitCard.Size = new System.Drawing.Size(272, 50);
            this.btnAdmitCard.TabIndex = 7;
            this.btnAdmitCard.Tag = "nav";
            this.btnAdmitCard.Text = "Admit Card Printing";
            this.btnAdmitCard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdmitCard.Click += new System.EventHandler(this.btnAdmitCard_Click);
            this.btnClass.AccessibleName = "Class Setup";
            this.btnClass.BackColor = System.Drawing.Color.Transparent;
            this.btnClass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClass.FlatAppearance.BorderSize = 0;
            this.btnClass.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClass.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnClass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnClass.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnClass.Name = "btnClass";
            this.btnClass.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnClass.Size = new System.Drawing.Size(272, 50);
            this.btnClass.TabIndex = 8;
            this.btnClass.Tag = "nav";
            this.btnClass.Text = "Class Setup";
            this.btnClass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClass.Click += new System.EventHandler(this.btnClass_Click);
            this.btnSection.AccessibleName = "Section Setup";
            this.btnSection.BackColor = System.Drawing.Color.Transparent;
            this.btnSection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSection.FlatAppearance.BorderSize = 0;
            this.btnSection.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnSection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnSection.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnSection.Name = "btnSection";
            this.btnSection.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnSection.Size = new System.Drawing.Size(272, 50);
            this.btnSection.TabIndex = 9;
            this.btnSection.Tag = "nav";
            this.btnSection.Text = "Section Setup";
            this.btnSection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSection.Click += new System.EventHandler(this.btnSection_Click);
            this.btnUsers.AccessibleName = "User Access & Security";
            this.btnUsers.BackColor = System.Drawing.Color.Transparent;
            this.btnUsers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnUsers.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnUsers.Size = new System.Drawing.Size(272, 50);
            this.btnUsers.TabIndex = 10;
            this.btnUsers.Tag = "nav";
            this.btnUsers.Text = "User Access & Security";
            this.btnUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            this.btnLogout.AccessibleName = "Sign Out";
            this.btnLogout.BackColor = System.Drawing.Color.Transparent;
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.btnLogout.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Padding = new System.Windows.Forms.Padding(12, 10, 8, 10);
            this.btnLogout.Size = new System.Drawing.Size(272, 50);
            this.btnLogout.TabIndex = 11;
            this.btnLogout.Tag = "nav-danger";
            this.btnLogout.Text = "Sign Out";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            this.lblSecOps.AutoSize = true;
            this.lblSecOps.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSecOps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSecOps.Margin = new System.Windows.Forms.Padding(12, 16, 4, 6);
            this.lblSecOps.Name = "lblSecOps";
            this.lblSecOps.Size = new System.Drawing.Size(120, 13);
            this.lblSecOps.Tag = "muted";
            this.lblSecOps.Text = "OPERATIONAL CORE";

            this.lblSecFees.AutoSize = true;
            this.lblSecFees.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSecFees.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSecFees.Margin = new System.Windows.Forms.Padding(12, 16, 4, 6);
            this.lblSecFees.Name = "lblSecFees";
            this.lblSecFees.Size = new System.Drawing.Size(120, 13);
            this.lblSecFees.Tag = "muted";
            this.lblSecFees.Text = "ACCOUNTS & FEES";

            this.lblSecExam.AutoSize = true;
            this.lblSecExam.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSecExam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSecExam.Margin = new System.Windows.Forms.Padding(12, 16, 4, 6);
            this.lblSecExam.Name = "lblSecExam";
            this.lblSecExam.Size = new System.Drawing.Size(120, 13);
            this.lblSecExam.Tag = "muted";
            this.lblSecExam.Text = "EXAM & ADMIT CARDS";

            this.lblSecSystem.AutoSize = true;
            this.lblSecSystem.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSecSystem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSecSystem.Margin = new System.Windows.Forms.Padding(12, 16, 4, 6);
            this.lblSecSystem.Name = "lblSecSystem";
            this.lblSecSystem.Size = new System.Drawing.Size(120, 13);
            this.lblSecSystem.Tag = "muted";
            this.lblSecSystem.Text = "SYSTEM";
            //
            // navFlow
            //
            this.navFlow.AutoScroll = true;
            this.navFlow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(20)))));
            this.navFlow.Controls.Add(this.btnToggleSidebar);
            this.navFlow.Controls.Add(this.lblSecOps);
            this.navFlow.Controls.Add(this.btnDashboard);
            this.navFlow.Controls.Add(this.btnAdmission);
            this.navFlow.Controls.Add(this.btnSearch);
            this.navFlow.Controls.Add(this.lblSecFees);
            this.navFlow.Controls.Add(this.btnFeeCollection);
            this.navFlow.Controls.Add(this.btnLedger);
            this.navFlow.Controls.Add(this.lblSecExam);
            this.navFlow.Controls.Add(this.btnExamClearance);
            this.navFlow.Controls.Add(this.btnAdmitCard);
            this.navFlow.Controls.Add(this.lblSecSystem);
            this.navFlow.Controls.Add(this.btnClass);
            this.navFlow.Controls.Add(this.btnSection);
            this.navFlow.Controls.Add(this.btnUsers);
            this.navFlow.Controls.Add(this.btnLogout);
            this.navFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.navFlow.Name = "navFlow";
            this.navFlow.Padding = new System.Windows.Forms.Padding(2);
            this.navFlow.Size = new System.Drawing.Size(280, 600);
            this.navFlow.TabIndex = 1;
            this.navFlow.Tag = "sidebar";
            this.navFlow.WrapContents = false;
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(20)))));
            this.pnlHeader.Controls.Add(this.lblLogo);
            this.pnlHeader.Controls.Add(this.lblUserRole);
            this.pnlHeader.Controls.Add(this.lblSession);
            this.pnlHeader.Controls.Add(this.lblClock);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(300, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlHeader.Size = new System.Drawing.Size(840, 64);
            this.pnlHeader.TabIndex = 1;
            this.pnlHeader.Tag = "header";
            this.pnlHeader.Resize += new System.EventHandler(this.pnlHeader_Resize);
            //
            // lblLogo
            //
            this.lblLogo.AutoSize = true;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.White;
            this.lblLogo.Location = new System.Drawing.Point(16, 18);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(320, 25);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "FOUZDARHAT SAFIRIA DAKHIL MADRASAH";
            //
            // lblUserRole
            //
            this.lblUserRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserRole.AutoSize = true;
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserRole.ForeColor = System.Drawing.Color.White;
            this.lblUserRole.Location = new System.Drawing.Point(700, 10);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(36, 19);
            this.lblUserRole.TabIndex = 1;
            this.lblUserRole.Text = "User";
            //
            // lblSession
            //
            this.lblSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSession.AutoSize = true;
            this.lblSession.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSession.Location = new System.Drawing.Point(700, 34);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(48, 15);
            this.lblSession.TabIndex = 2;
            this.lblSession.Tag = "muted";
            this.lblSession.Text = "Session";
            //
            // lblClock
            //
            this.lblClock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClock.AutoSize = true;
            this.lblClock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblClock.Location = new System.Drawing.Point(920, 22);
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(80, 15);
            this.lblClock.TabIndex = 3;
            this.lblClock.Text = "--";
            //
            // pnlContentContainer
            //
            this.pnlContentContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.pnlContentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContentContainer.Location = new System.Drawing.Point(300, 64);
            this.pnlContentContainer.Name = "pnlContentContainer";
            this.pnlContentContainer.Padding = new System.Windows.Forms.Padding(8);
            this.pnlContentContainer.Size = new System.Drawing.Size(840, 614);
            this.pnlContentContainer.TabIndex = 2;
            //
            // statusBar
            //
            this.statusBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(20)))));
            this.statusBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblDbStatus,
            this.lblSync});
            this.statusBar.Location = new System.Drawing.Point(300, 678);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(840, 22);
            this.statusBar.TabIndex = 3;
            //
            // lblDbStatus
            //
            this.lblDbStatus.Name = "lblDbStatus";
            this.lblDbStatus.Size = new System.Drawing.Size(120, 17);
            this.lblDbStatus.Text = "Database: Checking...";
            //
            // lblSync
            //
            this.lblSync.Name = "lblSync";
            this.lblSync.Size = new System.Drawing.Size(705, 17);
            this.lblSync.Spring = true;
            this.lblSync.Text = "Ledger sync: -";
            this.lblSync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // frmMainMenu
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.pnlContentContainer);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.pnlSidebar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "frmMainMenu";
            this.Text = "Fouzdarhat Safiria Dakhil Madrasah - Student Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlSidebar.ResumeLayout(false);
            this.navFlow.ResumeLayout(false);
            this.navFlow.PerformLayout();
            this.pnlProfile.ResumeLayout(false);
            this.pnlProfile.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Avatar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(37, 99, 235)))
            {
                e.Graphics.FillEllipse(brush, 0, 0, this.lblAvatar.Width - 1, this.lblAvatar.Height - 1);
            }
            string initials = "AD";
            if (this.lblAvatar.Tag != null)
            {
                string tagText = this.lblAvatar.Tag.ToString();
                if (!string.IsNullOrEmpty(tagText))
                    initials = tagText;
            }
            TextRenderer.DrawText(
                e.Graphics,
                initials,
                this.lblAvatar.Font,
                this.lblAvatar.ClientRectangle,
                System.Drawing.Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void pnlHeader_Resize(object sender, EventArgs e)
        {
            this.lblClock.Left = this.pnlHeader.Width - this.lblClock.Width - 20;
            this.lblUserRole.Left = this.pnlHeader.Width - 360;
            this.lblSession.Left = this.pnlHeader.Width - 360;
        }
    }
}
