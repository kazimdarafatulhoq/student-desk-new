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
    public partial class frmAdmitCardPrint : Form
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
        public frmAdmitCardPrint()
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

                _terms = (await Exams.GetActiveTermsAsync()).ToList();
                cboTerm.DisplayMember = nameof(ExamTerm.TermName);
                cboTerm.ValueMember = nameof(ExamTerm.ExamTermId);
                cboTerm.DataSource = _terms;

                _classes = (await Students.GetClassesAsync()).ToList();
                _classes.Insert(0, new ClassDto { ClassId = 0, ClassName = "All Classes" });
                cboClass.DisplayMember = nameof(ClassDto.ClassName);
                cboClass.ValueMember = nameof(ClassDto.ClassId);
                cboClass.DataSource = _classes;

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

            var matchCount = FilterLookup(classId, txtStudentSearch.Text).Count;
            var classLabel = classId is int id
                ? (_classes.FirstOrDefault(c => c.ClassId == id)?.ClassName ?? $"Class {id}")
                : "All Classes";

            lblStatus.ForeColor = UITheme.TextMuted;
            lblStatus.Text = matchCount == 0
                ? $"No students in {classLabel} match this search."
                : $"Showing {matchCount} student(s) · Class filter: {classLabel}. Single print uses the selected student; bulk uses this class filter.";
        }

        private List<StudentDto> FilterLookup(int? classId, string? search)
        {
            IEnumerable<StudentDto> query = _lookup;
            if (classId is > 0)
                query = query.Where(s => s.ClassId == classId.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.FullName) &&
                     s.FullName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.RegistrationNo) &&
                     s.RegistrationNo.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.GuardianPhone) &&
                     s.GuardianPhone.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.RollNumber) &&
                     s.RollNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            }
            return query.ToList();
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

        private async void btnIssue_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                var studentId = StudentComboBinder.TryGetSelectedInt(cboStudent);
                var termId = StudentComboBinder.TryGetSelectedInt(cboTerm);
                if (studentId is null || termId is null)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = "Select a student and exam term first.";
                    return;
                }

                btnIssue.Enabled = false;
                var card = await Exams.IssueAdmitCardAsync(
                    studentId.Value,
                    termId.Value,
                    AppSession.Current?.UserId ?? 0,
                    chkOverride.Checked,
                    txtReason.Text.Trim(),
                    AppSession.Current?.Role);

                using var preview = new frmAdmitCardPreview(
                    new[] { card },
                    AppSession.InstitutionName,
                    AppSession.InstitutionCampus);
                var result = preview.ShowDialog(FindForm());
                if (result == DialogResult.OK)
                {
                    lblStatus.ForeColor = UITheme.Success;
                    lblStatus.Text = $"Admit card {card.AdmitCardNo} printed. PDF: {preview.SavedPdfPath}";
                }
                else
                {
                    lblStatus.ForeColor = UITheme.TextMuted;
                    lblStatus.Text = $"Admit card {card.AdmitCardNo} issued. Preview closed without printing.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = UITheme.Danger;
                lblStatus.Text = ex.Message;
            }
            finally
            {
                btnIssue.Enabled = true;
            }
        }

        private async void btnBatch_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                var termId = StudentComboBinder.TryGetSelectedInt(cboTerm);
                if (termId is null)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = "Select an exam term first.";
                    return;
                }

                btnBatch.Enabled = false;
                var classId = StudentComboBinder.TryGetSelectedInt(cboClass);
                var classLabel = classId is int id
                    ? (_classes.FirstOrDefault(c => c.ClassId == id)?.ClassName ?? $"Class {id}")
                    : "All Classes";

                // Bulk uses the same class + search filter currently shown in the student list.
                var students = FilterLookup(classId, txtStudentSearch.Text);

                if (students.Count == 0)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = $"No active students found for bulk print ({classLabel}).";
                    return;
                }

                var issued = new List<AdmitCardDto>();
                var skipped = 0;

                foreach (var s in students)
                {
                    try
                    {
                        var card = await Exams.IssueAdmitCardAsync(
                            s.StudentId, termId.Value, AppSession.Current?.UserId ?? 0,
                            chkOverride.Checked, txtReason.Text.Trim(), AppSession.Current?.Role);
                        issued.Add(card);
                    }
                    catch
                    {
                        skipped++;
                    }
                }

                if (issued.Count == 0)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = $"No admit cards issued for {classLabel}. {skipped} student(s) blocked (dues) or failed.";
                    return;
                }

                using var preview = new frmAdmitCardPreview(
                    issued,
                    AppSession.InstitutionName,
                    AppSession.InstitutionCampus);
                var result = preview.ShowDialog(FindForm());
                if (result == DialogResult.OK)
                {
                    lblStatus.ForeColor = UITheme.Success;
                    lblStatus.Text = $"Batch printed ({classLabel}): {issued.Count} card(s)" +
                                     (skipped > 0 ? $", {skipped} skipped." : ".") +
                                     $" PDF: {preview.SavedPdfPath}";
                }
                else
                {
                    lblStatus.ForeColor = UITheme.TextMuted;
                    lblStatus.Text = $"Batch issued {issued.Count} card(s) for {classLabel}. Preview closed without printing.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = UITheme.Danger;
                lblStatus.Text = ex.Message;
            }
            finally
            {
                btnBatch.Enabled = true;
            }
        }
    }
}
