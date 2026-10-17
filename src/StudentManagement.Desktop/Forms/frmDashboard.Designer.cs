using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    partial class frmDashboard
    {
        private IContainer components = null;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnQuickFee;
        private Panel cardStudents;
        private Panel cardCollection;
        private Panel cardDues;
        private Panel cardEligible;
        private Panel cardBreakdown;
        private Label lblStudents;
        private Label lblCollection;
        private Label lblDues;
        private Label lblEligible;
        private Label lblPaidHint;
        private Label lblCollectionHint;
        private Label lblDueHint;
        private Label lblEligibleHint;
        private Label lblStudentsTitle;
        private Label lblCollectionTitle;
        private Label lblDuesTitle;
        private Label lblEligibleTitle;
        private Label lblBreakdownTitle;
        private ProgressBar barTuition;
        private ProgressBar barIct;
        private ProgressBar barExam;
        private Label lblBarTuition;
        private Label lblBarIct;
        private Label lblBarExam;
        private Label lblPctTuition;
        private Label lblPctIct;
        private Label lblPctExam;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnQuickFee = new System.Windows.Forms.Button();
            this.cardStudents = new System.Windows.Forms.Panel();
            this.lblStudentsTitle = new System.Windows.Forms.Label();
            this.lblStudents = new System.Windows.Forms.Label();
            this.lblPaidHint = new System.Windows.Forms.Label();
            this.cardCollection = new System.Windows.Forms.Panel();
            this.lblCollectionTitle = new System.Windows.Forms.Label();
            this.lblCollection = new System.Windows.Forms.Label();
            this.lblCollectionHint = new System.Windows.Forms.Label();
            this.cardDues = new System.Windows.Forms.Panel();
            this.lblDuesTitle = new System.Windows.Forms.Label();
            this.lblDues = new System.Windows.Forms.Label();
            this.lblDueHint = new System.Windows.Forms.Label();
            this.cardEligible = new System.Windows.Forms.Panel();
            this.lblEligibleTitle = new System.Windows.Forms.Label();
            this.lblEligible = new System.Windows.Forms.Label();
            this.lblEligibleHint = new System.Windows.Forms.Label();
            this.cardBreakdown = new System.Windows.Forms.Panel();
            this.lblBreakdownTitle = new System.Windows.Forms.Label();
            this.lblBarTuition = new System.Windows.Forms.Label();
            this.barTuition = new System.Windows.Forms.ProgressBar();
            this.lblPctTuition = new System.Windows.Forms.Label();
            this.lblBarIct = new System.Windows.Forms.Label();
            this.barIct = new System.Windows.Forms.ProgressBar();
            this.lblPctIct = new System.Windows.Forms.Label();
            this.lblBarExam = new System.Windows.Forms.Label();
            this.barExam = new System.Windows.Forms.ProgressBar();
            this.lblPctExam = new System.Windows.Forms.Label();
            this.cardStudents.SuspendLayout();
            this.cardCollection.SuspendLayout();
            this.cardDues.SuspendLayout();
            this.cardEligible.SuspendLayout();
            this.cardBreakdown.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Main Dashboard & Real-Time Analytics";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSubtitle.Location = new System.Drawing.Point(16, 48);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(320, 15);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Tag = "muted";
            this.lblSubtitle.Text = "Institutional overview for Fouzdarhat Safiria Dakhil Madrasah.";
            //
            // btnQuickFee
            //
            this.btnQuickFee.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnQuickFee.Location = new System.Drawing.Point(960, 20);
            this.btnQuickFee.Name = "btnQuickFee";
            this.btnQuickFee.Size = new System.Drawing.Size(200, 36);
            this.btnQuickFee.TabIndex = 2;
            this.btnQuickFee.Text = "Quick Fee Collection";
            this.btnQuickFee.Click += new System.EventHandler(this.btnQuickFee_Click);
            //
            // cardStudents
            //
            this.cardStudents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.cardStudents.Controls.Add(this.lblStudentsTitle);
            this.cardStudents.Controls.Add(this.lblStudents);
            this.cardStudents.Controls.Add(this.lblPaidHint);
            this.cardStudents.Location = new System.Drawing.Point(16, 90);
            this.cardStudents.Name = "cardStudents";
            this.cardStudents.Size = new System.Drawing.Size(260, 168);
            this.cardStudents.TabIndex = 3;
            this.cardStudents.Tag = "card";
            this.cardStudents.Paint += new System.Windows.Forms.PaintEventHandler(this.cardStudents_Paint);
            //
            // lblStudentsTitle
            //
            this.lblStudentsTitle.AutoSize = true;
            this.lblStudentsTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblStudentsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblStudentsTitle.Location = new System.Drawing.Point(20, 18);
            this.lblStudentsTitle.Name = "lblStudentsTitle";
            this.lblStudentsTitle.Size = new System.Drawing.Size(140, 19);
            this.lblStudentsTitle.TabIndex = 0;
            this.lblStudentsTitle.Tag = "muted";
            this.lblStudentsTitle.Text = "Total Active Students";
            //
            // lblStudents
            //
            this.lblStudents.AutoSize = true;
            this.lblStudents.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblStudents.ForeColor = System.Drawing.Color.White;
            this.lblStudents.Location = new System.Drawing.Point(20, 50);
            this.lblStudents.Name = "lblStudents";
            this.lblStudents.Size = new System.Drawing.Size(40, 41);
            this.lblStudents.TabIndex = 1;
            this.lblStudents.Tag = "metric";
            this.lblStudents.Text = "...";
            //
            // lblPaidHint
            //
            this.lblPaidHint.AutoSize = true;
            this.lblPaidHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblPaidHint.Location = new System.Drawing.Point(20, 118);
            this.lblPaidHint.Name = "lblPaidHint";
            this.lblPaidHint.Size = new System.Drawing.Size(0, 15);
            this.lblPaidHint.TabIndex = 2;
            this.lblPaidHint.Tag = "success";
            //
            // cardCollection
            //
            this.cardCollection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.cardCollection.Controls.Add(this.lblCollectionTitle);
            this.cardCollection.Controls.Add(this.lblCollection);
            this.cardCollection.Controls.Add(this.lblCollectionHint);
            this.cardCollection.Location = new System.Drawing.Point(300, 90);
            this.cardCollection.Name = "cardCollection";
            this.cardCollection.Size = new System.Drawing.Size(260, 168);
            this.cardCollection.TabIndex = 4;
            this.cardCollection.Tag = "card";
            this.cardCollection.Paint += new System.Windows.Forms.PaintEventHandler(this.cardCollection_Paint);
            //
            // lblCollectionTitle
            //
            this.lblCollectionTitle.AutoSize = true;
            this.lblCollectionTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCollectionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblCollectionTitle.Location = new System.Drawing.Point(20, 18);
            this.lblCollectionTitle.Name = "lblCollectionTitle";
            this.lblCollectionTitle.Size = new System.Drawing.Size(120, 19);
            this.lblCollectionTitle.TabIndex = 0;
            this.lblCollectionTitle.Tag = "muted";
            this.lblCollectionTitle.Text = "Today's Collection";
            //
            // lblCollection
            //
            this.lblCollection.AutoSize = true;
            this.lblCollection.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCollection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblCollection.Location = new System.Drawing.Point(20, 50);
            this.lblCollection.Name = "lblCollection";
            this.lblCollection.Size = new System.Drawing.Size(40, 41);
            this.lblCollection.TabIndex = 1;
            this.lblCollection.Tag = "primary,metric";
            this.lblCollection.Text = "...";
            //
            // lblCollectionHint
            //
            this.lblCollectionHint.AutoSize = true;
            this.lblCollectionHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblCollectionHint.Location = new System.Drawing.Point(20, 118);
            this.lblCollectionHint.Name = "lblCollectionHint";
            this.lblCollectionHint.Size = new System.Drawing.Size(0, 15);
            this.lblCollectionHint.TabIndex = 2;
            this.lblCollectionHint.Tag = "muted";
            //
            // cardDues
            //
            this.cardDues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.cardDues.Controls.Add(this.lblDuesTitle);
            this.cardDues.Controls.Add(this.lblDues);
            this.cardDues.Controls.Add(this.lblDueHint);
            this.cardDues.Location = new System.Drawing.Point(584, 90);
            this.cardDues.Name = "cardDues";
            this.cardDues.Size = new System.Drawing.Size(260, 168);
            this.cardDues.TabIndex = 5;
            this.cardDues.Tag = "card";
            this.cardDues.Paint += new System.Windows.Forms.PaintEventHandler(this.cardDues_Paint);
            //
            // lblDuesTitle
            //
            this.lblDuesTitle.AutoSize = true;
            this.lblDuesTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDuesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblDuesTitle.Location = new System.Drawing.Point(20, 18);
            this.lblDuesTitle.Name = "lblDuesTitle";
            this.lblDuesTitle.Size = new System.Drawing.Size(130, 19);
            this.lblDuesTitle.TabIndex = 0;
            this.lblDuesTitle.Tag = "muted";
            this.lblDuesTitle.Text = "Current Month Due";
            //
            // lblDues
            //
            this.lblDues.AutoSize = true;
            this.lblDues.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblDues.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(204)))), ((int)(((byte)(21)))));
            this.lblDues.Location = new System.Drawing.Point(20, 50);
            this.lblDues.Name = "lblDues";
            this.lblDues.Size = new System.Drawing.Size(40, 41);
            this.lblDues.TabIndex = 1;
            this.lblDues.Tag = "warning,metric";
            this.lblDues.Text = "...";
            //
            // lblDueHint
            //
            this.lblDueHint.AutoSize = true;
            this.lblDueHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(63)))), ((int)(((byte)(94)))));
            this.lblDueHint.Location = new System.Drawing.Point(20, 118);
            this.lblDueHint.Name = "lblDueHint";
            this.lblDueHint.Size = new System.Drawing.Size(0, 15);
            this.lblDueHint.TabIndex = 2;
            this.lblDueHint.Tag = "danger";
            //
            // cardEligible
            //
            this.cardEligible.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.cardEligible.Controls.Add(this.lblEligibleTitle);
            this.cardEligible.Controls.Add(this.lblEligible);
            this.cardEligible.Controls.Add(this.lblEligibleHint);
            this.cardEligible.Location = new System.Drawing.Point(868, 90);
            this.cardEligible.Name = "cardEligible";
            this.cardEligible.Size = new System.Drawing.Size(260, 168);
            this.cardEligible.TabIndex = 6;
            this.cardEligible.Tag = "card";
            this.cardEligible.Paint += new System.Windows.Forms.PaintEventHandler(this.cardEligible_Paint);
            //
            // lblEligibleTitle
            //
            this.lblEligibleTitle.AutoSize = true;
            this.lblEligibleTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEligibleTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblEligibleTitle.Location = new System.Drawing.Point(20, 18);
            this.lblEligibleTitle.Name = "lblEligibleTitle";
            this.lblEligibleTitle.Size = new System.Drawing.Size(130, 19);
            this.lblEligibleTitle.TabIndex = 0;
            this.lblEligibleTitle.Tag = "muted";
            this.lblEligibleTitle.Text = "Exam Admit Eligible";
            //
            // lblEligible
            //
            this.lblEligible.AutoSize = true;
            this.lblEligible.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblEligible.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblEligible.Location = new System.Drawing.Point(20, 50);
            this.lblEligible.Name = "lblEligible";
            this.lblEligible.Size = new System.Drawing.Size(40, 41);
            this.lblEligible.TabIndex = 1;
            this.lblEligible.Tag = "success,metric";
            this.lblEligible.Text = "...";
            //
            // lblEligibleHint
            //
            this.lblEligibleHint.AutoSize = true;
            this.lblEligibleHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(63)))), ((int)(((byte)(94)))));
            this.lblEligibleHint.Location = new System.Drawing.Point(20, 118);
            this.lblEligibleHint.Name = "lblEligibleHint";
            this.lblEligibleHint.Size = new System.Drawing.Size(0, 15);
            this.lblEligibleHint.TabIndex = 2;
            this.lblEligibleHint.Tag = "danger";
            //
            // cardBreakdown
            //
            this.cardBreakdown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.cardBreakdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.cardBreakdown.Controls.Add(this.lblBreakdownTitle);
            this.cardBreakdown.Controls.Add(this.lblBarTuition);
            this.cardBreakdown.Controls.Add(this.barTuition);
            this.cardBreakdown.Controls.Add(this.lblPctTuition);
            this.cardBreakdown.Controls.Add(this.lblBarIct);
            this.cardBreakdown.Controls.Add(this.barIct);
            this.cardBreakdown.Controls.Add(this.lblPctIct);
            this.cardBreakdown.Controls.Add(this.lblBarExam);
            this.cardBreakdown.Controls.Add(this.barExam);
            this.cardBreakdown.Controls.Add(this.lblPctExam);
            this.cardBreakdown.Location = new System.Drawing.Point(16, 286);
            this.cardBreakdown.Name = "cardBreakdown";
            this.cardBreakdown.Size = new System.Drawing.Size(900, 220);
            this.cardBreakdown.TabIndex = 7;
            this.cardBreakdown.Tag = "card";
            this.cardBreakdown.Paint += new System.Windows.Forms.PaintEventHandler(this.cardBreakdown_Paint);
            //
            // lblBreakdownTitle
            //
            this.lblBreakdownTitle.AutoSize = true;
            this.lblBreakdownTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBreakdownTitle.ForeColor = System.Drawing.Color.White;
            this.lblBreakdownTitle.Location = new System.Drawing.Point(20, 16);
            this.lblBreakdownTitle.Name = "lblBreakdownTitle";
            this.lblBreakdownTitle.Size = new System.Drawing.Size(200, 19);
            this.lblBreakdownTitle.TabIndex = 0;
            this.lblBreakdownTitle.Text = "Monthly Collection Breakdown";
            //
            // lblBarTuition
            //
            this.lblBarTuition.AutoSize = true;
            this.lblBarTuition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblBarTuition.Location = new System.Drawing.Point(20, 48);
            this.lblBarTuition.Name = "lblBarTuition";
            this.lblBarTuition.Size = new System.Drawing.Size(70, 15);
            this.lblBarTuition.TabIndex = 1;
            this.lblBarTuition.Tag = "muted";
            this.lblBarTuition.Text = "1. Tuition Fees";
            //
            // barTuition
            //
            this.barTuition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.barTuition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.barTuition.Location = new System.Drawing.Point(20, 68);
            this.barTuition.Maximum = 100;
            this.barTuition.Name = "barTuition";
            this.barTuition.Size = new System.Drawing.Size(780, 16);
            this.barTuition.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.barTuition.TabIndex = 2;
            //
            // lblPctTuition
            //
            this.lblPctTuition.AutoSize = true;
            this.lblPctTuition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblPctTuition.Location = new System.Drawing.Point(820, 66);
            this.lblPctTuition.Name = "lblPctTuition";
            this.lblPctTuition.Size = new System.Drawing.Size(36, 15);
            this.lblPctTuition.TabIndex = 3;
            this.lblPctTuition.Text = "0%";
            //
            // lblBarIct
            //
            this.lblBarIct.AutoSize = true;
            this.lblBarIct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblBarIct.Location = new System.Drawing.Point(20, 98);
            this.lblBarIct.Name = "lblBarIct";
            this.lblBarIct.Size = new System.Drawing.Size(140, 15);
            this.lblBarIct.TabIndex = 4;
            this.lblBarIct.Tag = "muted";
            this.lblBarIct.Text = "2. ICT & Computer Lab Fees";
            //
            // barIct
            //
            this.barIct.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.barIct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.barIct.Location = new System.Drawing.Point(20, 118);
            this.barIct.Maximum = 100;
            this.barIct.Name = "barIct";
            this.barIct.Size = new System.Drawing.Size(780, 16);
            this.barIct.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.barIct.TabIndex = 5;
            //
            // lblPctIct
            //
            this.lblPctIct.AutoSize = true;
            this.lblPctIct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblPctIct.Location = new System.Drawing.Point(820, 116);
            this.lblPctIct.Name = "lblPctIct";
            this.lblPctIct.Size = new System.Drawing.Size(36, 15);
            this.lblPctIct.TabIndex = 6;
            this.lblPctIct.Text = "0%";
            //
            // lblBarExam
            //
            this.lblBarExam.AutoSize = true;
            this.lblBarExam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblBarExam.Location = new System.Drawing.Point(20, 148);
            this.lblBarExam.Name = "lblBarExam";
            this.lblBarExam.Size = new System.Drawing.Size(130, 15);
            this.lblBarExam.TabIndex = 7;
            this.lblBarExam.Tag = "muted";
            this.lblBarExam.Text = "3. Term Examination Fees";
            //
            // barExam
            //
            this.barExam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.barExam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.barExam.Location = new System.Drawing.Point(20, 168);
            this.barExam.Maximum = 100;
            this.barExam.Name = "barExam";
            this.barExam.Size = new System.Drawing.Size(780, 16);
            this.barExam.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.barExam.TabIndex = 8;
            //
            // lblPctExam
            //
            this.lblPctExam.AutoSize = true;
            this.lblPctExam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.lblPctExam.Location = new System.Drawing.Point(820, 166);
            this.lblPctExam.Name = "lblPctExam";
            this.lblPctExam.Size = new System.Drawing.Size(36, 15);
            this.lblPctExam.TabIndex = 9;
            this.lblPctExam.Text = "0%";
            //
            // frmDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(12)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(1180, 560);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.btnQuickFee);
            this.Controls.Add(this.cardStudents);
            this.Controls.Add(this.cardCollection);
            this.Controls.Add(this.cardDues);
            this.Controls.Add(this.cardEligible);
            this.Controls.Add(this.cardBreakdown);
            this.Name = "frmDashboard";
            this.Text = "Dashboard";
            this.Resize += new System.EventHandler(this.frmDashboard_Resize);
            this.cardStudents.ResumeLayout(false);
            this.cardStudents.PerformLayout();
            this.cardCollection.ResumeLayout(false);
            this.cardCollection.PerformLayout();
            this.cardDues.ResumeLayout(false);
            this.cardDues.PerformLayout();
            this.cardEligible.ResumeLayout(false);
            this.cardEligible.PerformLayout();
            this.cardBreakdown.ResumeLayout(false);
            this.cardBreakdown.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void cardStudents_Paint(object sender, PaintEventArgs e)
        {
            PaintCard(e, this.cardStudents, System.Drawing.Color.FromArgb(37, 99, 235));
        }

        private void cardCollection_Paint(object sender, PaintEventArgs e)
        {
            PaintCard(e, this.cardCollection, System.Drawing.Color.FromArgb(37, 99, 235));
        }

        private void cardDues_Paint(object sender, PaintEventArgs e)
        {
            PaintCard(e, this.cardDues, System.Drawing.Color.FromArgb(250, 204, 21));
        }

        private void cardEligible_Paint(object sender, PaintEventArgs e)
        {
            PaintCard(e, this.cardEligible, System.Drawing.Color.FromArgb(16, 185, 129));
        }

        private void cardBreakdown_Paint(object sender, PaintEventArgs e)
        {
            PaintCard(e, this.cardBreakdown, System.Drawing.Color.FromArgb(139, 92, 246));
        }

        private static void PaintCard(PaintEventArgs e, Panel card, Color accent)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Pen border = new Pen(System.Drawing.Color.FromArgb(36, 48, 73)))
            {
                e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
            }
            using (Pen accentPen = new Pen(accent, 3))
            {
                e.Graphics.DrawLine(accentPen, 0, 8, 0, card.Height - 8);
            }
        }

        private void frmDashboard_Resize(object sender, EventArgs e)
        {
            this.LayoutDashboard();
        }

        private void LayoutDashboard()
        {
            int areaW = this.ClientSize.Width;
            if (areaW < 100)
                return;

            // Keep Quick Fee on the right without Right-anchor fighting the title.
            this.btnQuickFee.Anchor = AnchorStyles.Top;
            this.btnQuickFee.Top = 18;
            this.btnQuickFee.Left = Math.Max(16, areaW - this.btnQuickFee.Width - 24);

            // Title must stop before the button so it never paints underneath.
            int titleMaxW = Math.Max(120, this.btnQuickFee.Left - this.lblTitle.Left - 16);
            this.lblTitle.AutoSize = false;
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.Size = new System.Drawing.Size(titleMaxW, 28);
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.AutoEllipsis = true;
            this.lblSubtitle.Size = new System.Drawing.Size(titleMaxW, 20);

            int gap = 16;
            int side = 16;
            int usable = Math.Max(400, areaW - side * 2);
            int cardW = Math.Max(180, (usable - gap * 3) / 4);

            // Taller metric cards so footer hints are never clipped (DPI / AutoScale).
            int metricsTop = 90;
            int metricsHeight = 168;
            this.cardStudents.Top = metricsTop;
            this.cardStudents.Left = side;
            this.cardStudents.Size = new System.Drawing.Size(cardW, metricsHeight);
            this.cardCollection.Top = metricsTop;
            this.cardCollection.Left = this.cardStudents.Right + gap;
            this.cardCollection.Size = new System.Drawing.Size(cardW, metricsHeight);
            this.cardDues.Top = metricsTop;
            this.cardDues.Left = this.cardCollection.Right + gap;
            this.cardDues.Size = new System.Drawing.Size(cardW, metricsHeight);
            this.cardEligible.Top = metricsTop;
            this.cardEligible.Left = this.cardDues.Right + gap;
            this.cardEligible.Size = new System.Drawing.Size(cardW, metricsHeight);

            LayoutMetricCard(this.cardStudents, this.lblStudentsTitle, this.lblStudents, this.lblPaidHint);
            LayoutMetricCard(this.cardCollection, this.lblCollectionTitle, this.lblCollection, this.lblCollectionHint);
            LayoutMetricCard(this.cardDues, this.lblDuesTitle, this.lblDues, this.lblDueHint);
            LayoutMetricCard(this.cardEligible, this.lblEligibleTitle, this.lblEligible, this.lblEligibleHint);

            // Breakdown sits clearly below the metric cards (no overlap).
            int breakdownTop = this.cardStudents.Bottom + 24;
            this.cardBreakdown.Left = side;
            this.cardBreakdown.Top = breakdownTop;
            this.cardBreakdown.Width = usable;
            this.cardBreakdown.Height = 220;

            int barLeft = 20;
            int pctWidth = 56;
            int barW = Math.Max(120, this.cardBreakdown.Width - barLeft - pctWidth - 28);
            int pctLeft = barLeft + barW + 12;

            this.lblBreakdownTitle.Location = new System.Drawing.Point(barLeft, 16);
            this.lblBreakdownTitle.Visible = true;
            this.lblBreakdownTitle.BringToFront();

            this.lblBarTuition.Location = new System.Drawing.Point(barLeft, 52);
            this.barTuition.Location = new System.Drawing.Point(barLeft, 74);
            this.lblPctTuition.Location = new System.Drawing.Point(pctLeft, 72);

            this.lblBarIct.Location = new System.Drawing.Point(barLeft, 104);
            this.barIct.Location = new System.Drawing.Point(barLeft, 126);
            this.lblPctIct.Location = new System.Drawing.Point(pctLeft, 124);

            this.lblBarExam.Location = new System.Drawing.Point(barLeft, 156);
            this.barExam.Location = new System.Drawing.Point(barLeft, 178);
            this.lblPctExam.Location = new System.Drawing.Point(pctLeft, 176);

            this.barTuition.Size = new System.Drawing.Size(barW, 16);
            this.barIct.Size = new System.Drawing.Size(barW, 16);
            this.barExam.Size = new System.Drawing.Size(barW, 16);

            this.lblBarTuition.Visible = true;
            this.lblBarIct.Visible = true;
            this.lblBarExam.Visible = true;
            this.barTuition.Visible = true;
            this.barIct.Visible = true;
            this.barExam.Visible = true;
            this.lblPctTuition.Visible = true;
            this.lblPctIct.Visible = true;
            this.lblPctExam.Visible = true;

            // Keep metric cards above the breakdown in z-order.
            this.cardStudents.BringToFront();
            this.cardCollection.BringToFront();
            this.cardDues.BringToFront();
            this.cardEligible.BringToFront();
            this.cardBreakdown.BringToFront();

            this.cardBreakdown.Invalidate();
        }

        private static void LayoutMetricCard(Panel card, Label title, Label value, Label hint)
        {
            if (card == null || title == null || value == null || hint == null)
                return;

            int pad = 16;
            title.Location = new System.Drawing.Point(pad, 14);
            value.Location = new System.Drawing.Point(pad, 44);

            // Pin footer hint above the bottom edge with breathing room for DPI/font growth.
            int hintTop = Math.Max(value.Bottom + 8, card.Height - hint.PreferredHeight - 18);
            hint.Location = new System.Drawing.Point(pad, hintTop);
            hint.MaximumSize = new System.Drawing.Size(Math.Max(60, card.Width - pad * 2), 0);
            hint.BringToFront();
        }

        private void btnQuickFee_Click(object sender, EventArgs e)
        {
            Control host = this.Parent;
            while (host != null)
            {
                frmMainMenu menu = host as frmMainMenu;
                if (menu != null)
                {
                    menu.OpenFeeCollectionFromDashboard();
                    return;
                }
                host = host.Parent;
            }
        }
    }
}
