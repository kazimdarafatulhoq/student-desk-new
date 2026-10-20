using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    partial class frmAdmitCardPrint
    {
        private IContainer components = null;
        private Label lblTitle;
        private Panel pnlCard;
        private Label lblTerm;
        private ComboBox cboTerm;
        private Label lblStudent;
        private TextBox txtStudentSearch;
        private ComboBox cboStudent;
        private Label lblClass;
        private ComboBox cboClass;
        private Button btnIssue;
        private Button btnBatch;
        private Label lblStatus;

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
            this.pnlCard = new System.Windows.Forms.Panel();
            this.lblTerm = new System.Windows.Forms.Label();
            this.cboTerm = new System.Windows.Forms.ComboBox();
            this.lblStudent = new System.Windows.Forms.Label();
            this.txtStudentSearch = new System.Windows.Forms.TextBox();
            this.cboStudent = new System.Windows.Forms.ComboBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.cboClass = new System.Windows.Forms.ComboBox();
            this.btnIssue = new System.Windows.Forms.Button();
            this.btnBatch = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(260, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Admit Card Designer & Print";
            //
            // pnlCard
            //
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(20)))));
            this.pnlCard.Controls.Add(this.lblTerm);
            this.pnlCard.Controls.Add(this.cboTerm);
            this.pnlCard.Controls.Add(this.lblStudent);
            this.pnlCard.Controls.Add(this.txtStudentSearch);
            this.pnlCard.Controls.Add(this.cboStudent);
            this.pnlCard.Controls.Add(this.lblClass);
            this.pnlCard.Controls.Add(this.cboClass);
            this.pnlCard.Controls.Add(this.btnIssue);
            this.pnlCard.Controls.Add(this.btnBatch);
            this.pnlCard.Location = new System.Drawing.Point(16, 55);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(760, 260);
            this.pnlCard.TabIndex = 1;
            this.pnlCard.Tag = "card";
            //
            // lblTerm
            //
            this.lblTerm.AutoSize = true;
            this.lblTerm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblTerm.Location = new System.Drawing.Point(20, 20);
            this.lblTerm.Name = "lblTerm";
            this.lblTerm.Size = new System.Drawing.Size(70, 15);
            this.lblTerm.TabIndex = 0;
            this.lblTerm.Text = "Exam Term";
            //
            // cboTerm
            //
            this.cboTerm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboTerm.Location = new System.Drawing.Point(20, 42);
            this.cboTerm.Name = "cboTerm";
            this.cboTerm.Size = new System.Drawing.Size(320, 23);
            this.cboTerm.TabIndex = 1;
            //
            // lblStudent
            //
            this.lblStudent.AutoSize = true;
            this.lblStudent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblStudent.Location = new System.Drawing.Point(20, 85);
            this.lblStudent.Name = "lblStudent";
            this.lblStudent.Size = new System.Drawing.Size(250, 15);
            this.lblStudent.TabIndex = 2;
            this.lblStudent.Text = "Student (search name / REG / phone / roll)";
            //
            // txtStudentSearch
            //
            this.txtStudentSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtStudentSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStudentSearch.ForeColor = System.Drawing.Color.White;
            this.txtStudentSearch.Location = new System.Drawing.Point(20, 107);
            this.txtStudentSearch.Name = "txtStudentSearch";
            this.txtStudentSearch.PlaceholderText = "Type to filter student list…";
            this.txtStudentSearch.Size = new System.Drawing.Size(420, 23);
            this.txtStudentSearch.TabIndex = 3;
            this.txtStudentSearch.TextChanged += new System.EventHandler(this.txtStudentSearch_TextChanged);
            //
            // cboStudent
            //
            this.cboStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStudent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboStudent.Location = new System.Drawing.Point(20, 137);
            this.cboStudent.Name = "cboStudent";
            this.cboStudent.Size = new System.Drawing.Size(420, 23);
            this.cboStudent.TabIndex = 4;
            //
            // lblClass
            //
            this.lblClass.AutoSize = true;
            this.lblClass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblClass.Location = new System.Drawing.Point(460, 85);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(180, 15);
            this.lblClass.TabIndex = 5;
            this.lblClass.Text = "Class (required for bulk print)";
            //
            // cboClass
            //
            this.cboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboClass.Location = new System.Drawing.Point(460, 107);
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(260, 23);
            this.cboClass.TabIndex = 6;
            this.cboClass.SelectedIndexChanged += new System.EventHandler(this.cboClass_SelectedIndexChanged);
            //
            // btnIssue
            //
            this.btnIssue.Location = new System.Drawing.Point(20, 190);
            this.btnIssue.Name = "btnIssue";
            this.btnIssue.Size = new System.Drawing.Size(190, 36);
            this.btnIssue.TabIndex = 7;
            this.btnIssue.Text = "Preview & Print Single";
            this.btnIssue.Click += new System.EventHandler(this.btnIssue_Click);
            //
            // btnBatch
            //
            this.btnBatch.Location = new System.Drawing.Point(220, 190);
            this.btnBatch.Name = "btnBatch";
            this.btnBatch.Size = new System.Drawing.Size(200, 36);
            this.btnBatch.TabIndex = 8;
            this.btnBatch.Tag = "success";
            this.btnBatch.Text = "Bulk Print (Class)";
            this.btnBatch.Click += new System.EventHandler(this.btnBatch_Click);
            //
            // lblStatus
            //
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblStatus.Location = new System.Drawing.Point(16, 330);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(760, 70);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Single = one student. Bulk = selected class; due students are skipped, rest print.";
            //
            // frmAdmitCardPrint
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(12)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(820, 420);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlCard);
            this.Controls.Add(this.lblStatus);
            this.Name = "frmAdmitCardPrint";
            this.Text = "Admit Card Print";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
