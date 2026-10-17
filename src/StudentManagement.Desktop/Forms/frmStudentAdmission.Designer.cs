using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    partial class frmStudentAdmission
    {
        private IContainer components = null;
        private Label lblTitle;
        private Label lblCap;
        private GroupBox grpPersonal;
        private GroupBox grpAddress;
        private GroupBox grpAcademic;
        private GroupBox grpPhoto;
        private PictureBox picStudent;
        private Button btnBrowsePhoto;
        private Button btnClearPhoto;
        private Label lblPhotoHint;
        private Label lblFullName;
        private TextBox txtFullName;
        private Label lblFatherName;
        private TextBox txtFatherName;
        private Label lblMotherName;
        private TextBox txtMotherName;
        private Label lblBloodGroup;
        private ComboBox cboBloodGroup;
        private Label lblGender;
        private ComboBox cboGender;
        private Label lblDob;
        private DateTimePicker dtpDateOfBirth;
        private Label lblAge;
        private TextBox txtAge;
        private Label lblGuardianPhone;
        private TextBox txtGuardianPhone;
        private Label lblPresentAddress;
        private TextBox txtPresentAddress;
        private Label lblPermanentAddress;
        private TextBox txtPermanentAddress;
        private CheckBox chkSameAddress;
        private Label lblClass;
        private ComboBox cboClass;
        private Label lblSection;
        private ComboBox cboSection;
        private Label lblRollNumber;
        private TextBox txtRollNumber;
        private Label lblTuitionFee;
        private TextBox txtTuitionFee;
        private Label lblStatus;
        private ComboBox cboStatus;
        private Label lblRegNo;
        private TextBox txtRegNo;
        private Button btnSave;
        private Button btnReset;
        private Button btnMarkInactive;

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
            this.lblCap = new System.Windows.Forms.Label();
            this.grpPhoto = new System.Windows.Forms.GroupBox();
            this.picStudent = new System.Windows.Forms.PictureBox();
            this.lblPhotoHint = new System.Windows.Forms.Label();
            this.btnBrowsePhoto = new System.Windows.Forms.Button();
            this.btnClearPhoto = new System.Windows.Forms.Button();
            this.grpPersonal = new System.Windows.Forms.GroupBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblFatherName = new System.Windows.Forms.Label();
            this.txtFatherName = new System.Windows.Forms.TextBox();
            this.lblMotherName = new System.Windows.Forms.Label();
            this.txtMotherName = new System.Windows.Forms.TextBox();
            this.lblBloodGroup = new System.Windows.Forms.Label();
            this.cboBloodGroup = new System.Windows.Forms.ComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.cboGender = new System.Windows.Forms.ComboBox();
            this.lblDob = new System.Windows.Forms.Label();
            this.dtpDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.lblAge = new System.Windows.Forms.Label();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.lblGuardianPhone = new System.Windows.Forms.Label();
            this.txtGuardianPhone = new System.Windows.Forms.TextBox();
            this.grpAddress = new System.Windows.Forms.GroupBox();
            this.lblPresentAddress = new System.Windows.Forms.Label();
            this.txtPresentAddress = new System.Windows.Forms.TextBox();
            this.lblPermanentAddress = new System.Windows.Forms.Label();
            this.txtPermanentAddress = new System.Windows.Forms.TextBox();
            this.chkSameAddress = new System.Windows.Forms.CheckBox();
            this.grpAcademic = new System.Windows.Forms.GroupBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.cboClass = new System.Windows.Forms.ComboBox();
            this.lblSection = new System.Windows.Forms.Label();
            this.cboSection = new System.Windows.Forms.ComboBox();
            this.lblRollNumber = new System.Windows.Forms.Label();
            this.txtRollNumber = new System.Windows.Forms.TextBox();
            this.lblTuitionFee = new System.Windows.Forms.Label();
            this.txtTuitionFee = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblRegNo = new System.Windows.Forms.Label();
            this.txtRegNo = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnMarkInactive = new System.Windows.Forms.Button();
            this.grpPhoto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStudent)).BeginInit();
            this.grpPersonal.SuspendLayout();
            this.grpAddress.SuspendLayout();
            this.grpAcademic.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Student Registration & Admission Form";
            //
            // lblCap
            //
            this.lblCap.AutoSize = true;
            this.lblCap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblCap.Location = new System.Drawing.Point(16, 44);
            this.lblCap.Name = "lblCap";
            this.lblCap.Size = new System.Drawing.Size(400, 15);
            this.lblCap.TabIndex = 1;
            this.lblCap.Tag = "muted";
            this.lblCap.Text = "Capture student photo, biodata, guardian contacts, and class placement.";
            //
            // grpPhoto
            //
            this.grpPhoto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.grpPhoto.Controls.Add(this.picStudent);
            this.grpPhoto.Controls.Add(this.lblPhotoHint);
            this.grpPhoto.Controls.Add(this.btnBrowsePhoto);
            this.grpPhoto.Controls.Add(this.btnClearPhoto);
            this.grpPhoto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpPhoto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.grpPhoto.Location = new System.Drawing.Point(960, 72);
            this.grpPhoto.Name = "grpPhoto";
            this.grpPhoto.Size = new System.Drawing.Size(220, 280);
            this.grpPhoto.TabIndex = 2;
            this.grpPhoto.TabStop = false;
            this.grpPhoto.Text = "STUDENT PHOTO *";
            //
            // picStudent
            //
            this.picStudent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.picStudent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picStudent.Location = new System.Drawing.Point(20, 30);
            this.picStudent.Name = "picStudent";
            this.picStudent.Size = new System.Drawing.Size(180, 180);
            this.picStudent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picStudent.TabIndex = 0;
            this.picStudent.TabStop = false;
            //
            // lblPhotoHint
            //
            this.lblPhotoHint.AutoSize = true;
            this.lblPhotoHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblPhotoHint.Location = new System.Drawing.Point(20, 215);
            this.lblPhotoHint.Name = "lblPhotoHint";
            this.lblPhotoHint.Size = new System.Drawing.Size(148, 15);
            this.lblPhotoHint.TabIndex = 1;
            this.lblPhotoHint.Tag = "muted";
            this.lblPhotoHint.Text = "Photo required — upload";
            //
            // btnBrowsePhoto
            //
            this.btnBrowsePhoto.Location = new System.Drawing.Point(20, 238);
            this.btnBrowsePhoto.Name = "btnBrowsePhoto";
            this.btnBrowsePhoto.Size = new System.Drawing.Size(110, 32);
            this.btnBrowsePhoto.TabIndex = 2;
            this.btnBrowsePhoto.Text = "Upload Photo *";
            this.btnBrowsePhoto.Click += new System.EventHandler(this.btnBrowsePhoto_Click);
            //
            // btnClearPhoto
            //
            this.btnClearPhoto.Location = new System.Drawing.Point(136, 238);
            this.btnClearPhoto.Name = "btnClearPhoto";
            this.btnClearPhoto.Size = new System.Drawing.Size(64, 32);
            this.btnClearPhoto.TabIndex = 3;
            this.btnClearPhoto.Tag = "ghost";
            this.btnClearPhoto.Text = "Clear";
            this.btnClearPhoto.Click += new System.EventHandler(this.btnClearPhoto_Click);
            //
            // grpPersonal
            //
            this.grpPersonal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.grpPersonal.Controls.Add(this.lblFullName);
            this.grpPersonal.Controls.Add(this.txtFullName);
            this.grpPersonal.Controls.Add(this.lblFatherName);
            this.grpPersonal.Controls.Add(this.txtFatherName);
            this.grpPersonal.Controls.Add(this.lblMotherName);
            this.grpPersonal.Controls.Add(this.txtMotherName);
            this.grpPersonal.Controls.Add(this.lblBloodGroup);
            this.grpPersonal.Controls.Add(this.cboBloodGroup);
            this.grpPersonal.Controls.Add(this.lblGender);
            this.grpPersonal.Controls.Add(this.cboGender);
            this.grpPersonal.Controls.Add(this.lblDob);
            this.grpPersonal.Controls.Add(this.dtpDateOfBirth);
            this.grpPersonal.Controls.Add(this.lblAge);
            this.grpPersonal.Controls.Add(this.txtAge);
            this.grpPersonal.Controls.Add(this.lblGuardianPhone);
            this.grpPersonal.Controls.Add(this.txtGuardianPhone);
            this.grpPersonal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpPersonal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.grpPersonal.Location = new System.Drawing.Point(16, 72);
            this.grpPersonal.Name = "grpPersonal";
            this.grpPersonal.Size = new System.Drawing.Size(920, 180);
            this.grpPersonal.TabIndex = 3;
            this.grpPersonal.TabStop = false;
            this.grpPersonal.Text = "1. STUDENT INFORMATION";
            //
            // lblFullName
            //
            this.lblFullName.AutoSize = true;
            this.lblFullName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblFullName.Location = new System.Drawing.Point(20, 40);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(60, 15);
            this.lblFullName.TabIndex = 0;
            this.lblFullName.Text = "Full Name";
            //
            // txtFullName
            //
            this.txtFullName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFullName.ForeColor = System.Drawing.Color.White;
            this.txtFullName.Location = new System.Drawing.Point(20, 60);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(260, 23);
            this.txtFullName.TabIndex = 1;
            //
            // lblFatherName
            //
            this.lblFatherName.AutoSize = true;
            this.lblFatherName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblFatherName.Location = new System.Drawing.Point(300, 40);
            this.lblFatherName.Name = "lblFatherName";
            this.lblFatherName.Size = new System.Drawing.Size(80, 15);
            this.lblFatherName.TabIndex = 2;
            this.lblFatherName.Text = "Father's Name";
            //
            // txtFatherName
            //
            this.txtFatherName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtFatherName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFatherName.ForeColor = System.Drawing.Color.White;
            this.txtFatherName.Location = new System.Drawing.Point(300, 60);
            this.txtFatherName.Name = "txtFatherName";
            this.txtFatherName.Size = new System.Drawing.Size(260, 23);
            this.txtFatherName.TabIndex = 3;
            //
            // lblMotherName
            //
            this.lblMotherName.AutoSize = true;
            this.lblMotherName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblMotherName.Location = new System.Drawing.Point(580, 40);
            this.lblMotherName.Name = "lblMotherName";
            this.lblMotherName.Size = new System.Drawing.Size(85, 15);
            this.lblMotherName.TabIndex = 4;
            this.lblMotherName.Text = "Mother's Name";
            //
            // txtMotherName
            //
            this.txtMotherName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtMotherName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMotherName.ForeColor = System.Drawing.Color.White;
            this.txtMotherName.Location = new System.Drawing.Point(580, 60);
            this.txtMotherName.Name = "txtMotherName";
            this.txtMotherName.Size = new System.Drawing.Size(260, 23);
            this.txtMotherName.TabIndex = 5;
            //
            // lblBloodGroup
            //
            this.lblBloodGroup.AutoSize = true;
            this.lblBloodGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblBloodGroup.Location = new System.Drawing.Point(20, 100);
            this.lblBloodGroup.Name = "lblBloodGroup";
            this.lblBloodGroup.Size = new System.Drawing.Size(75, 15);
            this.lblBloodGroup.TabIndex = 6;
            this.lblBloodGroup.Text = "Blood Group";
            //
            // cboBloodGroup
            //
            this.cboBloodGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.cboBloodGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBloodGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboBloodGroup.ForeColor = System.Drawing.Color.White;
            this.cboBloodGroup.Location = new System.Drawing.Point(20, 120);
            this.cboBloodGroup.Name = "cboBloodGroup";
            this.cboBloodGroup.Size = new System.Drawing.Size(160, 23);
            this.cboBloodGroup.TabIndex = 7;
            //
            // lblGender
            //
            this.lblGender.AutoSize = true;
            this.lblGender.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblGender.Location = new System.Drawing.Point(200, 100);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(45, 15);
            this.lblGender.TabIndex = 8;
            this.lblGender.Text = "Gender";
            //
            // cboGender
            //
            this.cboGender.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.cboGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboGender.ForeColor = System.Drawing.Color.White;
            this.cboGender.Location = new System.Drawing.Point(200, 120);
            this.cboGender.Name = "cboGender";
            this.cboGender.Size = new System.Drawing.Size(140, 23);
            this.cboGender.TabIndex = 9;
            //
            // lblDob
            //
            this.lblDob.AutoSize = true;
            this.lblDob.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblDob.Location = new System.Drawing.Point(360, 100);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(80, 15);
            this.lblDob.TabIndex = 10;
            this.lblDob.Text = "Date of Birth";
            //
            // dtpDateOfBirth
            //
            this.dtpDateOfBirth.CustomFormat = "dd-MMM-yyyy";
            this.dtpDateOfBirth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateOfBirth.Location = new System.Drawing.Point(360, 120);
            this.dtpDateOfBirth.Name = "dtpDateOfBirth";
            this.dtpDateOfBirth.Size = new System.Drawing.Size(160, 23);
            this.dtpDateOfBirth.TabIndex = 11;
            //
            // lblAge
            //
            this.lblAge.AutoSize = true;
            this.lblAge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblAge.Location = new System.Drawing.Point(540, 100);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(28, 15);
            this.lblAge.TabIndex = 12;
            this.lblAge.Text = "Age";
            //
            // txtAge
            //
            this.txtAge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtAge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAge.ForeColor = System.Drawing.Color.White;
            this.txtAge.Location = new System.Drawing.Point(540, 120);
            this.txtAge.Name = "txtAge";
            this.txtAge.ReadOnly = true;
            this.txtAge.Size = new System.Drawing.Size(120, 23);
            this.txtAge.TabIndex = 13;
            //
            // lblGuardianPhone
            //
            this.lblGuardianPhone.AutoSize = true;
            this.lblGuardianPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblGuardianPhone.Location = new System.Drawing.Point(680, 100);
            this.lblGuardianPhone.Name = "lblGuardianPhone";
            this.lblGuardianPhone.Size = new System.Drawing.Size(95, 15);
            this.lblGuardianPhone.TabIndex = 14;
            this.lblGuardianPhone.Text = "Guardian Phone";
            //
            // txtGuardianPhone
            //
            this.txtGuardianPhone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtGuardianPhone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGuardianPhone.ForeColor = System.Drawing.Color.White;
            this.txtGuardianPhone.Location = new System.Drawing.Point(680, 120);
            this.txtGuardianPhone.Name = "txtGuardianPhone";
            this.txtGuardianPhone.Size = new System.Drawing.Size(160, 23);
            this.txtGuardianPhone.TabIndex = 15;
            //
            // grpAddress
            //
            this.grpAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.grpAddress.Controls.Add(this.lblPresentAddress);
            this.grpAddress.Controls.Add(this.txtPresentAddress);
            this.grpAddress.Controls.Add(this.lblPermanentAddress);
            this.grpAddress.Controls.Add(this.txtPermanentAddress);
            this.grpAddress.Controls.Add(this.chkSameAddress);
            this.grpAddress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.grpAddress.Location = new System.Drawing.Point(16, 262);
            this.grpAddress.Name = "grpAddress";
            this.grpAddress.Size = new System.Drawing.Size(920, 170);
            this.grpAddress.TabIndex = 4;
            this.grpAddress.TabStop = false;
            this.grpAddress.Text = "2. ADDRESS DETAILS";
            //
            // lblPresentAddress
            //
            this.lblPresentAddress.AutoSize = true;
            this.lblPresentAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblPresentAddress.Location = new System.Drawing.Point(20, 40);
            this.lblPresentAddress.Name = "lblPresentAddress";
            this.lblPresentAddress.Size = new System.Drawing.Size(95, 15);
            this.lblPresentAddress.TabIndex = 0;
            this.lblPresentAddress.Text = "Present Address";
            //
            // txtPresentAddress
            //
            this.txtPresentAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtPresentAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPresentAddress.ForeColor = System.Drawing.Color.White;
            this.txtPresentAddress.Location = new System.Drawing.Point(20, 60);
            this.txtPresentAddress.Multiline = true;
            this.txtPresentAddress.Name = "txtPresentAddress";
            this.txtPresentAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPresentAddress.Size = new System.Drawing.Size(420, 70);
            this.txtPresentAddress.TabIndex = 1;
            //
            // lblPermanentAddress
            //
            this.lblPermanentAddress.AutoSize = true;
            this.lblPermanentAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblPermanentAddress.Location = new System.Drawing.Point(460, 40);
            this.lblPermanentAddress.Name = "lblPermanentAddress";
            this.lblPermanentAddress.Size = new System.Drawing.Size(110, 15);
            this.lblPermanentAddress.TabIndex = 2;
            this.lblPermanentAddress.Text = "Permanent Address";
            //
            // txtPermanentAddress
            //
            this.txtPermanentAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtPermanentAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPermanentAddress.ForeColor = System.Drawing.Color.White;
            this.txtPermanentAddress.Location = new System.Drawing.Point(460, 60);
            this.txtPermanentAddress.Multiline = true;
            this.txtPermanentAddress.Name = "txtPermanentAddress";
            this.txtPermanentAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPermanentAddress.Size = new System.Drawing.Size(420, 70);
            this.txtPermanentAddress.TabIndex = 3;
            //
            // chkSameAddress
            //
            this.chkSameAddress.AutoSize = true;
            this.chkSameAddress.ForeColor = System.Drawing.Color.White;
            this.chkSameAddress.Location = new System.Drawing.Point(20, 140);
            this.chkSameAddress.Name = "chkSameAddress";
            this.chkSameAddress.Size = new System.Drawing.Size(190, 19);
            this.chkSameAddress.TabIndex = 4;
            this.chkSameAddress.Text = "Permanent same as Present";
            this.chkSameAddress.CheckedChanged += new System.EventHandler(this.chkSameAddress_CheckedChanged);
            //
            // grpAcademic
            //
            this.grpAcademic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(20)))), ((int)(((byte)(48)))));
            this.grpAcademic.Controls.Add(this.lblClass);
            this.grpAcademic.Controls.Add(this.cboClass);
            this.grpAcademic.Controls.Add(this.lblSection);
            this.grpAcademic.Controls.Add(this.cboSection);
            this.grpAcademic.Controls.Add(this.lblRollNumber);
            this.grpAcademic.Controls.Add(this.txtRollNumber);
            this.grpAcademic.Controls.Add(this.lblTuitionFee);
            this.grpAcademic.Controls.Add(this.txtTuitionFee);
            this.grpAcademic.Controls.Add(this.lblStatus);
            this.grpAcademic.Controls.Add(this.cboStatus);
            this.grpAcademic.Controls.Add(this.lblRegNo);
            this.grpAcademic.Controls.Add(this.txtRegNo);
            this.grpAcademic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpAcademic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.grpAcademic.Location = new System.Drawing.Point(16, 442);
            this.grpAcademic.Name = "grpAcademic";
            this.grpAcademic.Size = new System.Drawing.Size(920, 160);
            this.grpAcademic.TabIndex = 5;
            this.grpAcademic.TabStop = false;
            this.grpAcademic.Text = "3. ACADEMIC ENROLLMENT";
            //
            // lblClass
            //
            this.lblClass.AutoSize = true;
            this.lblClass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblClass.Location = new System.Drawing.Point(20, 40);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(95, 15);
            this.lblClass.TabIndex = 0;
            this.lblClass.Text = "Admission Class";
            //
            // cboClass
            //
            this.cboClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.cboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboClass.ForeColor = System.Drawing.Color.White;
            this.cboClass.Location = new System.Drawing.Point(20, 60);
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(180, 23);
            this.cboClass.TabIndex = 1;
            //
            // lblSection
            //
            this.lblSection.AutoSize = true;
            this.lblSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblSection.Location = new System.Drawing.Point(220, 40);
            this.lblSection.Name = "lblSection";
            this.lblSection.Size = new System.Drawing.Size(46, 15);
            this.lblSection.TabIndex = 2;
            this.lblSection.Text = "Section";
            //
            // cboSection
            //
            this.cboSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.cboSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboSection.ForeColor = System.Drawing.Color.White;
            this.cboSection.Location = new System.Drawing.Point(220, 60);
            this.cboSection.Name = "cboSection";
            this.cboSection.Size = new System.Drawing.Size(120, 23);
            this.cboSection.TabIndex = 3;
            //
            // lblRollNumber
            //
            this.lblRollNumber.AutoSize = true;
            this.lblRollNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblRollNumber.Location = new System.Drawing.Point(360, 40);
            this.lblRollNumber.Name = "lblRollNumber";
            this.lblRollNumber.Size = new System.Drawing.Size(75, 15);
            this.lblRollNumber.TabIndex = 4;
            this.lblRollNumber.Text = "Roll Number";
            //
            // txtRollNumber
            //
            this.txtRollNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtRollNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRollNumber.ForeColor = System.Drawing.Color.White;
            this.txtRollNumber.Location = new System.Drawing.Point(360, 60);
            this.txtRollNumber.Name = "txtRollNumber";
            this.txtRollNumber.Size = new System.Drawing.Size(120, 23);
            this.txtRollNumber.TabIndex = 5;
            //
            // lblTuitionFee
            //
            this.lblTuitionFee.AutoSize = true;
            this.lblTuitionFee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblTuitionFee.Location = new System.Drawing.Point(500, 40);
            this.lblTuitionFee.Name = "lblTuitionFee";
            this.lblTuitionFee.Size = new System.Drawing.Size(110, 15);
            this.lblTuitionFee.TabIndex = 6;
            this.lblTuitionFee.Text = "Monthly Tuition Fee";
            //
            // txtTuitionFee
            //
            this.txtTuitionFee.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtTuitionFee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTuitionFee.ForeColor = System.Drawing.Color.White;
            this.txtTuitionFee.Location = new System.Drawing.Point(500, 60);
            this.txtTuitionFee.Name = "txtTuitionFee";
            this.txtTuitionFee.Size = new System.Drawing.Size(160, 23);
            this.txtTuitionFee.TabIndex = 7;
            this.txtTuitionFee.Text = "2500";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblStatus.Location = new System.Drawing.Point(680, 40);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 15);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Status";
            this.lblStatus.Visible = false;
            //
            // cboStatus
            //
            this.cboStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboStatus.ForeColor = System.Drawing.Color.White;
            this.cboStatus.Location = new System.Drawing.Point(680, 60);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(160, 23);
            this.cboStatus.TabIndex = 9;
            this.cboStatus.Visible = false;
            //
            // lblRegNo
            //
            this.lblRegNo.AutoSize = true;
            this.lblRegNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(147)))), ((int)(((byte)(166)))));
            this.lblRegNo.Location = new System.Drawing.Point(20, 100);
            this.lblRegNo.Name = "lblRegNo";
            this.lblRegNo.Size = new System.Drawing.Size(95, 15);
            this.lblRegNo.TabIndex = 10;
            this.lblRegNo.Text = "Registration No";
            this.lblRegNo.Visible = false;
            //
            // txtRegNo
            //
            this.txtRegNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.txtRegNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRegNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.txtRegNo.Location = new System.Drawing.Point(20, 120);
            this.txtRegNo.Name = "txtRegNo";
            this.txtRegNo.ReadOnly = true;
            this.txtRegNo.Size = new System.Drawing.Size(220, 23);
            this.txtRegNo.TabIndex = 11;
            this.txtRegNo.Visible = false;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(16, 622);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(220, 40);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save & Complete Admission";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(250, 622);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(140, 40);
            this.btnReset.TabIndex = 7;
            this.btnReset.Tag = "ghost";
            this.btnReset.Text = "Reset Form";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnMarkInactive
            //
            this.btnMarkInactive.Location = new System.Drawing.Point(410, 622);
            this.btnMarkInactive.Name = "btnMarkInactive";
            this.btnMarkInactive.Size = new System.Drawing.Size(160, 40);
            this.btnMarkInactive.TabIndex = 8;
            this.btnMarkInactive.Tag = "danger";
            this.btnMarkInactive.Text = "Mark Inactive";
            this.btnMarkInactive.Visible = false;
            this.btnMarkInactive.Click += new System.EventHandler(this.btnMarkInactive_Click);
            //
            // frmStudentAdmission
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(12)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblCap);
            this.Controls.Add(this.grpPersonal);
            this.Controls.Add(this.grpPhoto);
            this.Controls.Add(this.grpAddress);
            this.Controls.Add(this.grpAcademic);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnMarkInactive);
            this.Name = "frmStudentAdmission";
            this.Text = "Student Admission";
            this.grpPhoto.ResumeLayout(false);
            this.grpPhoto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStudent)).EndInit();
            this.grpPersonal.ResumeLayout(false);
            this.grpPersonal.PerformLayout();
            this.grpAddress.ResumeLayout(false);
            this.grpAddress.PerformLayout();
            this.grpAcademic.ResumeLayout(false);
            this.grpAcademic.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
