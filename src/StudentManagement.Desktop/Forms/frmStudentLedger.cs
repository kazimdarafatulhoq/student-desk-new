using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.Reporting.Generators;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmStudentLedger : Form
    {
        private StudentManager? _students;
        private StudentManager Students => _students ??= new StudentManager();
        private FeeManager? _fees;
        private FeeManager Fees => _fees ??= new FeeManager();
        private List<StudentDto> _lookup = new();
        private LedgerSummaryDto? _summary;
        private IReadOnlyList<LedgerEntryDto> _entries = Array.Empty<LedgerEntryDto>();

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmStudentLedger()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            Load += async (s, e) => await LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            if (DesignTime.IsActive)
                return;

            _lookup = (await Students.GetLookupAsync()).ToList();
            cboStudent.DisplayMember = "Display";
            cboStudent.ValueMember = nameof(StudentDto.StudentId);
            cboStudent.DataSource = _lookup.Select(s => new
            {
                s.StudentId,
                Display = $"{s.RegistrationNo} — {s.FullName}"
            }).ToList();
        }

        private async void cboStudent_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboStudent.SelectedValue is not int studentId)
                return;
            await LoadLedgerAsync(studentId);
        }

        private async Task LoadLedgerAsync(int studentId)
        {
            if (DesignTime.IsActive)
                return;

            _summary = await Fees.GetSummaryAsync(studentId);
            _entries = await Fees.GetLedgerDetailedAsync(studentId);

            lblInvoiced.Text = $"৳ {_summary.TotalInvoiced:N2}";
            lblPaid.Text = $"৳ {_summary.TotalPaid:N2}";
            lblAdvance.Text = $"৳ {_summary.AdvanceBalance:N2}";
            lblDue.Text = $"৳ {_summary.NetDue:N2}";

            dgvLedger.Rows.Clear();
            foreach (var e in _entries)
            {
                dgvLedger.Rows.Add(
                    e.TransactionDate.ToString("dd-MMM-yyyy"),
                    e.VoucherNo,
                    e.Particulars,
                    e.FeePeriod,
                    e.DebitAmount > 0 ? e.DebitAmount.ToString("N2") : "",
                    e.CreditAmount > 0 ? e.CreditAmount.ToString("N2") : "",
                    e.RunningBalance.ToString("N2"),
                    e.Status.ToString());
            }
        }

        private void dgvLedger_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == 4 && e.Value is string d && !string.IsNullOrEmpty(d))
                e.CellStyle!.ForeColor = UITheme.Danger;
            if (e.ColumnIndex == 5 && e.Value is string c && !string.IsNullOrEmpty(c))
                e.CellStyle!.ForeColor = UITheme.Success;
        }

        private void btnPrint_Click(object? sender, EventArgs e)
        {
            if (_summary is null)
            {
                MessageBox.Show("Select a student first to print the ledger statement.",
                    "Print Statement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnPrint.Enabled = false;
                var path = LedgerStatementPrinter.GeneratePdf(
                    _summary,
                    _entries,
                    AppSession.InstitutionName,
                    AppSession.ReportsPath);

                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // PDF is saved even if the default viewer cannot open.
                }

                MessageBox.Show(
                    $"Ledger statement ready.\n\nSaved to:\n{path}",
                    "Print Statement",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Print Statement Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPrint.Enabled = true;
            }
        }

        private void btnExport_Click(object? sender, EventArgs e)
        {
            if (_summary is null)
            {
                MessageBox.Show("Select a student first to export the ledger.",
                    "Export CSV / Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnExport.Enabled = false;
                var path = LedgerStatementPrinter.ExportCsv(_entries, _summary.RegistrationNo, AppSession.ReportsPath);
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                catch
                {
                }

                MessageBox.Show($"Exported:\n{path}", "Export CSV / Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExport.Enabled = true;
            }
        }
    }
}
