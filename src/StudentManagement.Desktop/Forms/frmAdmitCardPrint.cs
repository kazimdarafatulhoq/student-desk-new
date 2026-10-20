using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                : $"Showing {matchCount} student(s) · Class: {classLabel}. Single = selected student. Bulk = whole class (dues skipped).";
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

        private static string BuildDueSms(StudentDto student, decimal dues)
        {
            var phone = string.IsNullOrWhiteSpace(student.GuardianPhone) ? "—" : student.GuardianPhone.Trim();
            return
                $"Assalamu Alaikum. {student.FullName} ({student.RegistrationNo}) er due ৳{dues:N2} ache. " +
                $"Admit card print hobe na. Madrasah Authority-r shathe jogajog korun. " +
                $"Guardian phone: {phone}. — {AppSession.InstitutionName}";
        }

        private void ShowDueBlockedMessage(StudentDto student, decimal dues)
        {
            var sms = BuildDueSms(student, dues);
            MessageBox.Show(
                "Admit card print blocked — outstanding dues / not cleared.\n\n" +
                $"Student: {student.FullName} ({student.RegistrationNo})\n" +
                $"Due: ৳{dues:N2}\n\n" +
                "Option 1: Clear dues, then print.\n" +
                "Option 2: Exam Clearance Verification → Admin override → then print here.\n\n" +
                "SMS (guardian-ke pathan):\n" + sms,
                "Due — Contact Madrasah Authority",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            lblStatus.ForeColor = UITheme.Warning;
            lblStatus.Text = $"Blocked: {student.FullName} · due ৳{dues:N2}. Clear dues or verify on Exam Clearance first.";
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

                // Uses saved Exam Clearance if already verified; otherwise live dues check.
                var clearance = await Exams.GetAdmitPrintEligibilityAsync(studentId.Value, termId.Value);

                if (!clearance.IsCleared)
                {
                    var student = _lookup.FirstOrDefault(s => s.StudentId == studentId.Value)
                                  ?? await Students.GetByIdAsync(studentId.Value);
                    if (student != null)
                        ShowDueBlockedMessage(student, clearance.DuesAtClearance);
                    else
                    {
                        lblStatus.ForeColor = UITheme.Danger;
                        lblStatus.Text = $"Cannot print. Outstanding dues: ৳{clearance.DuesAtClearance:N2}. Verify on Exam Clearance or clear dues.";
                    }
                    return;
                }

                var card = await Exams.IssueAdmitCardAsync(
                    studentId.Value,
                    termId.Value,
                    AppSession.Current?.UserId ?? 0);

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

                // Bulk is class-wise: a concrete class must be selected (not All Classes).
                var classId = StudentComboBinder.TryGetSelectedInt(cboClass);
                if (classId is null || classId.Value <= 0)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = "Bulk print is class-wise. Select a Class first (not All Classes).";
                    MessageBox.Show(
                        "Bulk print class onujayi hoy.\n\nPehele Class select korun (All Classes na).",
                        "Select Class",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                btnBatch.Enabled = false;
                var selectedClass = _classes.FirstOrDefault(c => c.ClassId == classId.Value);
                var classLabel = selectedClass?.ClassName ?? $"Class {classId.Value}";

                // Whole class roster by ClassId (also match ClassName in case of rename edge cases).
                var students = _lookup
                    .Where(s => s.ClassId == classId.Value
                                || (!string.IsNullOrWhiteSpace(selectedClass?.ClassName)
                                    && string.Equals(s.ClassName, selectedClass!.ClassName, StringComparison.OrdinalIgnoreCase)))
                    .GroupBy(s => s.StudentId)
                    .Select(g => g.First())
                    .OrderBy(s => s.SectionName)
                    .ThenBy(s => s.RollNumber, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(s => s.FullName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (students.Count == 0)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = $"No active students found in {classLabel}.";
                    return;
                }

                var issued = new List<AdmitCardDto>();
                var blocked = new List<(StudentDto Student, decimal Dues)>();
                var errors = new List<string>();

                foreach (var s in students)
                {
                    // Due / not cleared → skip; cleared (incl. Exam Clearance verify) → print.
                    ExamClearanceDto clearance;
                    try
                    {
                        clearance = await Exams.GetAdmitPrintEligibilityAsync(s.StudentId, termId.Value);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"{s.RegistrationNo} {s.FullName}: {ex.Message}");
                        continue;
                    }

                    if (!clearance.IsCleared)
                    {
                        blocked.Add((s, clearance.DuesAtClearance));
                        continue;
                    }

                    try
                    {
                        var card = await Exams.IssueAdmitCardAsync(
                            s.StudentId, termId.Value, AppSession.Current?.UserId ?? 0);
                        issued.Add(card);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"{s.RegistrationNo} {s.FullName}: {ex.Message}");
                    }
                }

                var summary = new StringBuilder();
                summary.AppendLine($"{classLabel} · Term selected");
                summary.AppendLine($"Total in class: {students.Count}");
                summary.AppendLine($"Printable (cleared): {issued.Count}");
                summary.AppendLine($"Skipped (dues): {blocked.Count}");
                if (errors.Count > 0)
                    summary.AppendLine($"Errors: {errors.Count}");
                summary.AppendLine();

                if (blocked.Count > 0)
                {
                    summary.AppendLine("Due / not cleared (SMS):");
                    foreach (var (student, dues) in blocked.Take(20))
                        summary.AppendLine("• " + BuildDueSms(student, dues));
                    if (blocked.Count > 20)
                        summary.AppendLine($"… and {blocked.Count - 20} more.");
                    summary.AppendLine();
                }

                if (errors.Count > 0)
                {
                    summary.AppendLine("Errors:");
                    foreach (var err in errors.Take(10))
                        summary.AppendLine("• " + err);
                    summary.AppendLine();
                }

                if (blocked.Count > 0 || errors.Count > 0 || issued.Count > 0)
                {
                    MessageBox.Show(
                        summary.ToString(),
                        $"Bulk — {classLabel}",
                        MessageBoxButtons.OK,
                        issued.Count > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }

                if (issued.Count == 0)
                {
                    lblStatus.ForeColor = UITheme.Warning;
                    lblStatus.Text = $"{classLabel}: 0 cards. Cleared must match this Exam Term. Skipped dues: {blocked.Count}, errors: {errors.Count}.";
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
                    lblStatus.Text = $"{classLabel} bulk printed: {issued.Count} card(s)" +
                                     (blocked.Count > 0 ? $", {blocked.Count} due skipped." : ".") +
                                     $" PDF: {preview.SavedPdfPath}";
                }
                else
                {
                    lblStatus.ForeColor = UITheme.TextMuted;
                    lblStatus.Text = $"{classLabel}: {issued.Count} card(s) ready. Preview closed without printing." +
                                     (blocked.Count > 0 ? $" {blocked.Count} due skipped." : "");
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
