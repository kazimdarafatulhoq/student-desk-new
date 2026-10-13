
using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;


using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmExamClearance : Form
    {
        private StudentManager? _students;
        private StudentManager Students => _students ??= new StudentManager();
        private ExamManager? _exams;
        private ExamManager Exams => _exams ??= new ExamManager();
        private List<StudentDto> _lookup = new();
        private List<ExamTerm> _terms = new();
        private List<ClassDto> _classes = new();
        private bool _suppressStudentFilter;

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmExamClearance()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            Load += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (DesignTime.IsActive)
                return;

            _suppressStudentFilter = true;
            try
            {
                _lookup = (await Students.GetLookupAsync()).ToList();

                _classes = (await Students.GetClassesAsync()).ToList();
                _classes.Insert(0, new ClassDto { ClassId = 0, ClassName = "All Classes" });
                cboClass.DisplayMember = nameof(ClassDto.ClassName);
                cboClass.ValueMember = nameof(ClassDto.ClassId);
                cboClass.DataSource = _classes;

                _terms = (await Exams.GetActiveTermsAsync()).ToList();
                cboTerm.DisplayMember = nameof(ExamTerm.TermName);
                cboTerm.ValueMember = nameof(ExamTerm.ExamTermId);
                cboTerm.DataSource = _terms;

                RefreshStudentList();
            }
            finally
            {
                _suppressStudentFilter = false;
            }
        }

        private void RefreshStudentList()
        {
            var classId = StudentComboBinder.TryGetSelectedInt(cboClass);
            StudentComboBinder.Bind(cboStudent, _lookup, txtStudentSearch.Text, classId);
            var matchCount = cboStudent.Items.Count;
            lblResult.ForeColor = UITheme.TextMuted;
            lblResult.Text = matchCount == 0
                ? "No students match this search/class filter."
                : $"Showing {matchCount} student(s). Select a term and verify clearance.";
        }

        private void txtStudentSearch_TextChanged(object? sender, EventArgs e)
        {
            if (_suppressStudentFilter) return;
            RefreshStudentList();
        }

        private void cboClass_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressStudentFilter) return;
            RefreshStudentList();
        }

        private async void btnVerify_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                var studentId = StudentComboBinder.TryGetSelectedInt(cboStudent);
                var termId = StudentComboBinder.TryGetSelectedInt(cboTerm);
                if (studentId is null || termId is null)
                {
                    lblResult.ForeColor = UITheme.Warning;
                    lblResult.Text = "Select a student and exam term first.";
                    return;
                }

                var result = await Exams.VerifyClearanceAsync(
                    studentId.Value,
                    termId.Value,
                    chkOverride.Checked,
                    txtOverrideReason.Text.Trim(),
                    AppSession.Current?.UserId,
                    AppSession.Current?.Role);

                if (!result.IsCleared)
                {
                    lblResult.ForeColor = UITheme.Danger;
                    lblResult.Text = $"BLOCKED — Outstanding dues ৳ {result.DuesAtClearance:N2}. Admit card cannot be issued unless an admin overrides.";
                }
                else
                {
                    lblResult.ForeColor = UITheme.Success;
                    lblResult.Text = result.AdminOverride
                        ? $"CLEARED with admin override (dues were ৳ {result.DuesAtClearance:N2})."
                        : "CLEARED — No outstanding dues. Eligible for admit card.";
                }
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = UITheme.Danger;
                lblResult.Text = ex.Message;
            }
        }
    }
}
