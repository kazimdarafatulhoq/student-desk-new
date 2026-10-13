using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer.Security;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmStudentAdmission : Form
    {
        private StudentManager? _students;
        private StudentManager Students => _students ??= new StudentManager();
        private string? _selectedPhotoPath;
        private int? _editStudentId;
        private string? _existingPhotoPath;
        private bool _lookupsReady;
        private int _suppressSectionReload;
        private bool _classHandlerWired;
        private readonly SemaphoreSlim _dbGate = new SemaphoreSlim(1, 1);

        /// <summary>Raised after a successful edit save / inactive when hosted in main menu.</summary>
        public event System.Action EditCompleted = delegate { };

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmStudentAdmission()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            ApplyModeChrome(editMode: false);
            dtpDateOfBirth.ValueChanged += (s, e) => UpdateAge();
            Load += async (s, e) =>
            {
                if (IsEditMode)
                    await LoadStudentForEditAsync();
                else
                    await LoadLookupsAsync();
            };
        }

        /// <summary>Edit existing student (opened from Student Directory).</summary>
        public frmStudentAdmission(int studentId) : this()
        {
            _editStudentId = studentId;
            ApplyModeChrome(editMode: true);
        }

        public bool IsEditMode => _editStudentId.HasValue;

        private void ApplyModeChrome(bool editMode)
        {
            lblStatus.Visible = editMode;
            cboStatus.Visible = editMode;
            lblRegNo.Visible = editMode;
            txtRegNo.Visible = editMode;
            btnMarkInactive.Visible = editMode;
            btnReset.Visible = !editMode;
            btnSave.Text = editMode ? "Save Changes" : "Save & Complete Admission";
            lblTitle.Text = editMode ? "Edit Student Profile" : "Student Registration & Admission Form";
            lblCap.Text = editMode
                ? "Update biodata, class placement, and active/inactive status."
                : "Capture student photo, biodata, guardian contacts, and class placement.";
            Text = editMode ? "Edit Student Profile" : "Student Admission";
        }

        private void WireClassChangedHandler()
        {
            if (_classHandlerWired)
                return;
            _classHandlerWired = true;
            cboClass.SelectedIndexChanged += async (s, e) =>
            {
                if (_suppressSectionReload > 0)
                    return;
                await LoadSectionsAsync();
            };
        }

        private async Task LoadLookupsAsync(bool loadDefaultSections = true)
        {
            if (DesignTime.IsActive || _lookupsReady)
                return;

            cboBloodGroup.DataSource = Enum.GetValues(typeof(BloodGroup));
            cboGender.DataSource = Enum.GetValues(typeof(Gender));
            cboStatus.DataSource = Enum.GetValues(typeof(StudentStatus));

            await _dbGate.WaitAsync();
            IReadOnlyList<ClassDto> classes;
            try
            {
                classes = await Students.GetClassesAsync();
            }
            finally
            {
                _dbGate.Release();
            }

            cboClass.DisplayMember = nameof(ClassDto.ClassName);
            cboClass.ValueMember = nameof(ClassDto.ClassId);

            _suppressSectionReload++;
            try
            {
                cboClass.DataSource = classes.ToList();
            }
            finally
            {
                _suppressSectionReload--;
            }

            // Do not wire class-change until after initial bind (avoids concurrent DbContext use).
            if (loadDefaultSections && classes.Count > 0)
                await LoadSectionsAsync();

            if (!IsEditMode)
                WireClassChangedHandler();

            UpdateAge();
            _lookupsReady = true;
        }

        private async Task LoadSectionsAsync()
        {
            if (DesignTime.IsActive)
                return;
            if (!(cboClass.SelectedValue is int classId) || classId <= 0)
                return;

            await _dbGate.WaitAsync();
            try
            {
                IReadOnlyList<SectionDto> sections = await Students.GetSectionsAsync(classId);
                cboSection.DisplayMember = nameof(SectionDto.SectionName);
                cboSection.ValueMember = nameof(SectionDto.SectionId);
                cboSection.DataSource = sections.ToList();
            }
            finally
            {
                _dbGate.Release();
            }
        }

        private async Task LoadStudentForEditAsync()
        {
            if (DesignTime.IsActive || !_editStudentId.HasValue)
                return;

            _suppressSectionReload++;
            try
            {
                // Load class list only — sections loaded once after SelectedValue is set.
                await LoadLookupsAsync(loadDefaultSections: false);

                await _dbGate.WaitAsync();
                StudentDto? student;
                try
                {
                    student = await Students.GetByIdAsync(_editStudentId.Value);
                }
                finally
                {
                    _dbGate.Release();
                }

                if (student is null)
                {
                    MessageBox.Show("Student not found.", "Edit Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    EditCompleted.Invoke();
                    return;
                }

                txtRegNo.Text = student.RegistrationNo;
                txtFullName.Text = student.FullName;
                txtFatherName.Text = student.FatherName;
                txtMotherName.Text = student.MotherName;
                cboBloodGroup.SelectedItem = student.BloodGroup;
                cboGender.SelectedItem = student.Gender;
                dtpDateOfBirth.Value = student.DateOfBirth < dtpDateOfBirth.MinDate
                    ? DateTime.Today.AddYears(-12)
                    : student.DateOfBirth;
                txtGuardianPhone.Text = student.GuardianPhone;
                txtPresentAddress.Text = student.PresentAddress ?? string.Empty;
                txtPermanentAddress.Text = student.PermanentAddress ?? string.Empty;
                chkSameAddress.Checked = string.Equals(
                    student.PresentAddress?.Trim(),
                    student.PermanentAddress?.Trim(),
                    StringComparison.OrdinalIgnoreCase);

                cboClass.SelectedValue = student.ClassId;
                await LoadSectionsAsync();
                cboSection.SelectedValue = student.SectionId;
                txtRollNumber.Text = student.RollNumber;
                txtTuitionFee.Text = student.MonthlyTuitionFee.ToString("0.##");
                cboStatus.SelectedItem = student.Status;

                _existingPhotoPath = student.PhotoPath;
                if (!string.IsNullOrWhiteSpace(student.PhotoPath) && File.Exists(student.PhotoPath))
                {
                    try
                    {
                        using (Image src = Image.FromFile(student.PhotoPath))
                            picStudent.Image = new Bitmap(src);
                        lblPhotoHint.Text = Path.GetFileName(student.PhotoPath);
                        lblPhotoHint.ForeColor = UITheme.Success;
                    }
                    catch
                    {
                        lblPhotoHint.Text = "Photo file missing";
                    }
                }

                UpdateAge();
                btnMarkInactive.Enabled = student.Status == StudentStatus.Active;
            }
            finally
            {
                _suppressSectionReload--;
                WireClassChangedHandler();
            }
        }

        private void UpdateAge()
        {
            txtAge.Text = AgeCalculator.Format(dtpDateOfBirth.Value.Date);
        }

        private void chkSameAddress_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkSameAddress.Checked)
            {
                txtPermanentAddress.Text = txtPresentAddress.Text;
                txtPermanentAddress.Enabled = false;
            }
            else
            {
                txtPermanentAddress.Enabled = true;
            }
        }

        private void btnBrowsePhoto_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Student Photo";
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                dlg.CheckFileExists = true;
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    if (picStudent.Image != null)
                    {
                        Image old = picStudent.Image;
                        picStudent.Image = null;
                        old.Dispose();
                    }

                    using (Image src = Image.FromFile(dlg.FileName))
                        picStudent.Image = new Bitmap(src);

                    _selectedPhotoPath = dlg.FileName;
                    lblPhotoHint.Text = Path.GetFileName(dlg.FileName);
                    lblPhotoHint.ForeColor = UITheme.Success;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load image.\n" + ex.Message, "Photo Upload",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnClearPhoto_Click(object? sender, EventArgs e)
        {
            ClearPhoto();
            _existingPhotoPath = null;
        }

        private void ClearPhoto()
        {
            if (picStudent.Image != null)
            {
                Image old = picStudent.Image;
                picStudent.Image = null;
                old.Dispose();
            }
            _selectedPhotoPath = null;
            lblPhotoHint.Text = "No photo selected";
            lblPhotoHint.ForeColor = UITheme.TextMuted;
        }

        private string? SavePhotoForStudent(string registrationNo)
        {
            if (string.IsNullOrWhiteSpace(_selectedPhotoPath) || !File.Exists(_selectedPhotoPath))
                return null;

            string photosRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "StudentManagement",
                "Photos");
            Directory.CreateDirectory(photosRoot);

            string ext = Path.GetExtension(_selectedPhotoPath);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".jpg";

            string dest = Path.Combine(photosRoot, registrationNo + ext);
            File.Copy(_selectedPhotoPath, dest, true);
            return dest;
        }

        private async void btnSave_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                btnSave.Enabled = false;
                if (chkSameAddress.Checked)
                    txtPermanentAddress.Text = txtPresentAddress.Text;

                if (!decimal.TryParse(txtTuitionFee.Text.Trim(), out var tuition))
                    throw new ArgumentException("Monthly tuition fee must be a valid number.");

                if (IsEditMode)
                    await SaveEditAsync(tuition);
                else
                    await SaveAdmitAsync(tuition);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, IsEditMode ? "Update Failed" : "Admission Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }

        private async Task SaveAdmitAsync(decimal tuition)
        {
            StudentDto result = await Students.AdmitAsync(new StudentAdmissionRequest
            {
                FullName = txtFullName.Text,
                FatherName = txtFatherName.Text,
                MotherName = txtMotherName.Text,
                BloodGroup = (BloodGroup)cboBloodGroup.SelectedItem!,
                Gender = (Gender)cboGender.SelectedItem!,
                DateOfBirth = dtpDateOfBirth.Value.Date,
                GuardianPhone = txtGuardianPhone.Text,
                PresentAddress = txtPresentAddress.Text,
                PermanentAddress = txtPermanentAddress.Text,
                ClassId = (int)cboClass.SelectedValue!,
                SectionId = (int)cboSection.SelectedValue!,
                RollNumber = txtRollNumber.Text,
                MonthlyTuitionFee = tuition,
                AcademicSession = AppSession.Current != null ? AppSession.Current.AcademicSession : "2025-2026",
                CreatedBy = AppSession.Current != null ? AppSession.Current.Username : null,
                PhotoPath = _selectedPhotoPath
            });

            string? savedPhoto = SavePhotoForStudent(result.RegistrationNo);
            if (!string.IsNullOrEmpty(savedPhoto))
            {
                result.PhotoPath = savedPhoto;
                try { await Students.UpdateAsync(result, AppSession.Current?.Username); }
                catch { /* offline may skip */ }
            }

            MessageBox.Show(
                "Student admitted successfully.\nRegistration ID: " + result.RegistrationNo +
                (string.IsNullOrEmpty(savedPhoto) ? string.Empty : "\nPhoto saved."),
                "Admission Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            btnReset_Click(this, EventArgs.Empty);
        }

        private async Task SaveEditAsync(decimal tuition)
        {
            string regNo = txtRegNo.Text.Trim();
            string? photo = _existingPhotoPath;
            string? saved = SavePhotoForStudent(regNo);
            if (!string.IsNullOrEmpty(saved))
                photo = saved;

            var dto = new StudentDto
            {
                StudentId = _editStudentId!.Value,
                RegistrationNo = regNo,
                FullName = txtFullName.Text,
                FatherName = txtFatherName.Text,
                MotherName = txtMotherName.Text,
                BloodGroup = (BloodGroup)cboBloodGroup.SelectedItem!,
                Gender = (Gender)cboGender.SelectedItem!,
                DateOfBirth = dtpDateOfBirth.Value.Date,
                GuardianPhone = txtGuardianPhone.Text,
                PresentAddress = txtPresentAddress.Text,
                PermanentAddress = txtPermanentAddress.Text,
                ClassId = (int)cboClass.SelectedValue!,
                SectionId = (int)cboSection.SelectedValue!,
                RollNumber = txtRollNumber.Text,
                MonthlyTuitionFee = tuition,
                Status = cboStatus.SelectedItem is StudentStatus st ? st : StudentStatus.Active,
                AcademicSession = AppSession.Current != null ? AppSession.Current.AcademicSession : "2025-2026",
                PhotoPath = photo
            };

            await Students.UpdateAsync(dto, AppSession.Current?.Username);
            MessageBox.Show(
                "Student profile updated.\nRegistration ID: " + regNo + "\nStatus: " + dto.Status,
                "Profile Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            btnMarkInactive.Enabled = dto.Status == StudentStatus.Active;
            // Return to Student Directory inside main menu content area.
            EditCompleted?.Invoke();
        }

        private async void btnMarkInactive_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive || !_editStudentId.HasValue)
                return;

            var confirm = MessageBox.Show(
                "Mark this student as Inactive?\nThey will remain in the directory but cannot enroll for new fees/exams as Active.",
                "Mark Inactive",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                btnMarkInactive.Enabled = false;
                await Students.MarkInactiveAsync(_editStudentId.Value, AppSession.Current?.Username);
                cboStatus.SelectedItem = StudentStatus.Inactive;
                MessageBox.Show("Student marked Inactive.", "Status Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                EditCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Inactive Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnMarkInactive.Enabled = true;
            }
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            if (IsEditMode)
                return;

            txtFullName.Clear();
            txtFatherName.Clear();
            txtMotherName.Clear();
            txtGuardianPhone.Clear();
            txtPresentAddress.Clear();
            txtPermanentAddress.Clear();
            txtPermanentAddress.Enabled = true;
            chkSameAddress.Checked = false;
            txtRollNumber.Clear();
            txtTuitionFee.Text = "2500";
            dtpDateOfBirth.Value = DateTime.Today.AddYears(-12);
            ClearPhoto();
            UpdateAge();
            txtFullName.Focus();
        }
    }
}
