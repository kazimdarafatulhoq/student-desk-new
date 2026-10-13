using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmStudentSearch : Form
    {
        private readonly StudentManager _students = new StudentManager();
        private List<StudentDto> _rows = new();

        public event System.Action<StudentDto>? OpenLedgerRequested;
        public event System.Action<StudentDto>? CollectFeeRequested;
        public event System.Action<StudentDto>? PrintAdmitRequested;

        /// <summary>Ask main menu to open Edit Profile inside the content panel.</summary>
        public event System.Action<int> EditStudentRequested = delegate { };

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmStudentSearch()
        {
            InitializeComponent();
            if (!DesignTime.IsActive)
                UITheme.ApplyForm(this);
            Load += async (s, e) =>
            {
                await LoadLookupsAsync();
                await SearchAsync();
            };
        }

        private async Task LoadLookupsAsync()
        {
            if (_students == null)
                return;

            var classes = (await _students.GetClassesAsync()).ToList();
            classes.Insert(0, new ClassDto { ClassId = 0, ClassName = "All Classes" });
            cboClass.DisplayMember = nameof(ClassDto.ClassName);
            cboClass.ValueMember = nameof(ClassDto.ClassId);
            cboClass.DataSource = classes;
            cboClass.SelectedIndexChanged += async (_, _) => await LoadSectionsAsync();

            cboStatus.Items.Clear();
            cboStatus.Items.Add("All Status");
            foreach (StudentStatus st in Enum.GetValues(typeof(StudentStatus)))
                cboStatus.Items.Add(st.ToString());
            cboStatus.SelectedIndex = 0;

            await LoadSectionsAsync();
        }

        private async Task LoadSectionsAsync()
        {
            if (_students == null)
                return;

            var classId = cboClass.SelectedValue is int id ? id : 0;
            var sections = classId > 0
                ? (await _students.GetSectionsAsync(classId)).ToList()
                : new List<SectionDto>();
            sections.Insert(0, new SectionDto { SectionId = 0, SectionName = "All Sections" });
            cboSection.DisplayMember = nameof(SectionDto.SectionName);
            cboSection.ValueMember = nameof(SectionDto.SectionId);
            cboSection.DataSource = sections;
        }

        private async void btnSearch_Click(object? sender, EventArgs e) => await SearchAsync();

        private async Task SearchAsync()
        {
            if (_students == null)
                return;

            StudentStatus? statusFilter = null;
            if (cboStatus.SelectedIndex > 0
                && Enum.TryParse(cboStatus.SelectedItem?.ToString(), out StudentStatus parsed))
                statusFilter = parsed;

            var filter = new StudentSearchFilter
            {
                Query = txtQuery.Text.Trim(),
                ClassId = cboClass.SelectedValue is int c && c > 0 ? c : null,
                SectionId = cboSection.SelectedValue is int s && s > 0 ? s : null,
                Status = statusFilter
            };

            _rows = (await _students.SearchAsync(filter)).ToList();
            dgvStudents.Rows.Clear();
            foreach (var row in _rows)
            {
                int idx = dgvStudents.Rows.Add("●", row.RegistrationNo, row.FullName, row.ClassName, row.RollNumber,
                    row.GuardianPhone, row.AdmissionDate.ToString("dd-MMM-yyyy"), row.Status.ToString());
                if (row.Status != StudentStatus.Active)
                    dgvStudents.Rows[idx].DefaultCellStyle.ForeColor = UITheme.TextMuted;
            }
            lblCount.Text = $"{_rows.Count} record(s)";
        }

        private StudentDto? Selected()
        {
            if (dgvStudents.CurrentRow is null || dgvStudents.CurrentRow.Index < 0 || dgvStudents.CurrentRow.Index >= _rows.Count)
                return null;
            return _rows[dgvStudents.CurrentRow.Index];
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            var s = Selected();
            if (s is null)
            {
                MessageBox.Show("Select a student first.", "Edit Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EditStudentRequested(s.StudentId);
        }

        private async void btnInactive_Click(object? sender, EventArgs e)
        {
            var s = Selected();
            if (s is null || _students == null)
            {
                MessageBox.Show("Select a student first.", "Mark Inactive", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (s.Status == StudentStatus.Inactive)
            {
                MessageBox.Show($"{s.RegistrationNo} is already Inactive.", "Mark Inactive",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Mark {s.RegistrationNo} — {s.FullName} as Inactive?",
                "Mark Inactive",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                await _students.MarkInactiveAsync(s.StudentId, AppSession.Current?.Username);
                await SearchAsync();
                MessageBox.Show("Student marked Inactive.", "Status Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Inactive Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLedger_Click(object? sender, EventArgs e)
        {
            var s = Selected();
            if (s is null) return;
            OpenLedgerRequested?.Invoke(s);
            MessageBox.Show($"Navigate to Student Ledger for {s.RegistrationNo}", "View Ledger", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCollect_Click(object? sender, EventArgs e)
        {
            var s = Selected();
            if (s is null) return;
            CollectFeeRequested?.Invoke(s);
            MessageBox.Show($"Navigate to Fee Collection for {s.RegistrationNo}", "Collect Fee", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdmit_Click(object? sender, EventArgs e)
        {
            var s = Selected();
            if (s is null) return;
            PrintAdmitRequested?.Invoke(s);
            MessageBox.Show($"Navigate to Admit Card for {s.RegistrationNo}", "Admit Card", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
