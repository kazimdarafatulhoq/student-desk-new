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
    public partial class frmFeeCollection : Form
    {
        private StudentManager? _students;
        private StudentManager Students => _students ??= new StudentManager();
        private FeeManager? _fees;
        private FeeManager Fees => _fees ??= new FeeManager();
        private StudentDto? _student;
        private List<CheckBox> _monthChecks = new();

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmFeeCollection()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            BuildMonthMatrix();
            WireOptionalFeeEvents();
        }

        private void WireOptionalFeeEvents()
        {
            foreach (TextBox box in GetOptionalFeeBoxes())
                box.TextChanged += (s, e) => Recalc();
        }

        private IEnumerable<TextBox> GetOptionalFeeBoxes()
        {
            yield return txtRegistrationFee;
            yield return txtAdmissionFee;
            yield return txtTransportFee;
            yield return txtExaminationFee;
            yield return txtTranscriptFee;
            yield return txtTransferCertificateFee;
            yield return txtHostelFoodCharges;
            yield return txtMiscellaneous;
        }

        private void BuildMonthMatrix()
        {
            flpMonths.Controls.Clear();
            _monthChecks.Clear();
            foreach (var month in FeeManager.BuildMonthMatrix(DateTime.Now.Year))
            {
                var chk = new CheckBox
                {
                    Text = month,
                    AutoSize = true,
                    ForeColor = UITheme.TextPrimary,
                    Margin = new Padding(8, 6, 8, 6)
                };
                chk.CheckedChanged += (_, _) => Recalc();
                _monthChecks.Add(chk);
                flpMonths.Controls.Add(chk);
            }
        }

        private List<NamedFeeAmount> ReadOptionalFees()
        {
            var list = new List<NamedFeeAmount>();
            // Names match frmFeeCollection designer labels and FeeCategories seed.
            AddIfPositive(list, "Registration Fee", txtRegistrationFee);
            AddIfPositive(list, "New Admission / Re-admission", txtAdmissionFee);
            AddIfPositive(list, "Monthly Transport Fee", txtTransportFee);
            AddIfPositive(list, "Examination Fee (Term / Annual)", txtExaminationFee);
            AddIfPositive(list, "Transcript / Testimonial / Certificate", txtTranscriptFee);
            AddIfPositive(list, "Transfer Certificate / Letter", txtTransferCertificateFee);
            AddIfPositive(list, "Hostel Food Charges", txtHostelFoodCharges);
            AddIfPositive(list, "Miscellaneous", txtMiscellaneous);
            return list;
        }

        private static void AddIfPositive(List<NamedFeeAmount> list, string name, TextBox box)
        {
            if (decimal.TryParse(box.Text, out var amount) && amount > 0)
                list.Add(new NamedFeeAmount { Name = name, Amount = amount });
        }

        /// <summary>Enter in the search box runs Quick Search (same as the button).</summary>
        private void txtSearch_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;
            e.Handled = true;
            btnFind.PerformClick();
        }

        private async void btnFind_Click(object? sender, EventArgs e)
        {
            if (DesignTime.IsActive)
                return;

            try
            {
                _student = await Students.FindQuickAsync(txtSearch.Text);
                if (_student is null)
                {
                    ClearBio();
                    MessageBox.Show("Student not found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblBioName.Text = _student.FullName;
                lblBioReg.Text = _student.RegistrationNo;
                lblBioClass.Text = $"{_student.ClassName} / {_student.SectionName}  ·  Roll {_student.RollNumber}";
                lblBioDue.Text = $"Net Due: ৳ {_student.NetDue:N2}";
                lblBioDue.ForeColor = _student.NetDue > 0 ? UITheme.Danger : UITheme.Success;
                lblBioPhone.Text = _student.GuardianPhone;
                txtTuitionUnit.Text = _student.MonthlyTuitionFee.ToString("0.##");
                Recalc();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearBio()
        {
            _student = null;
            lblBioName.Text = "—";
            lblBioReg.Text = "—";
            lblBioClass.Text = "—";
            lblBioDue.Text = "—";
            lblBioPhone.Text = "—";
        }

        private void Recalc()
        {
            if (_student is null)
            {
                lblNetPayable.Text = "৳ 0.00";
                lblTuitionTotal.Text = "Tuition: ৳ 0.00";
                return;
            }

            var months = _monthChecks.Count(c => c.Checked);
            _ = decimal.TryParse(txtTuitionUnit.Text, out var unit);
            _ = decimal.TryParse(txtFine.Text, out var fine);
            _ = decimal.TryParse(txtWaiver.Text, out var waiver);
            var otherFees = ReadOptionalFees();
            var otherTotal = otherFees.Sum(f => f.Amount);
            var tuition = unit * months;
            var net = FeeManager.CalculateNetPayable(tuition, fine, waiver, otherTotal);
            lblTuitionTotal.Text = otherTotal > 0
                ? $"Tuition: ৳ {tuition:N2}  ·  Other: ৳ {otherTotal:N2}"
                : $"Tuition: ৳ {tuition:N2}";
            lblNetPayable.Text = $"৳ {net:N2}";
            lblNetPayable.ForeColor = net > 0 ? UITheme.Success : UITheme.TextMuted;
        }

        private async void btnCollect_Click(object? sender, EventArgs e)
        {
            if (_student is null)
            {
                MessageBox.Show("Find a student first.", "Fee Collection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DesignTime.IsActive)
                return;

            try
            {
                btnCollect.Enabled = false;
                var periods = _monthChecks.Where(c => c.Checked).Select(c => c.Text).ToList();
                _ = decimal.TryParse(txtTuitionUnit.Text, out var unit);
                _ = decimal.TryParse(txtFine.Text, out var fine);
                _ = decimal.TryParse(txtWaiver.Text, out var waiver);
                var otherFees = ReadOptionalFees();

                var result = await Fees.CollectAsync(new FeeCollectionRequest
                {
                    StudentId = _student.StudentId,
                    FeePeriods = periods,
                    TuitionAmount = unit * periods.Count,
                    FineAmount = fine,
                    WaiverAmount = waiver,
                    OtherFees = otherFees,
                    PaymentMethod = (PaymentMethod)cboPaymentMethod.SelectedItem!,
                    TransactionRef = txtTxnRef.Text.Trim(),
                    Remarks = txtRemarks.Text.Trim(),
                    CollectedByUserId = AppSession.Current?.UserId ?? 0
                });

                // Preview first (admit-card style), then Print Copy from the dialog.
                using (var preview = new frmReceiptPreview(
                    result,
                    AppSession.InstitutionName,
                    AppSession.InstitutionCampus))
                {
                    preview.ShowDialog(FindForm());
                }

                _student = await Students.GetByIdAsync(_student.StudentId);
                if (_student is not null)
                {
                    lblBioDue.Text = $"Net Due: ৳ {_student.NetDue:N2}";
                    lblBioDue.ForeColor = _student.NetDue > 0 ? UITheme.Danger : UITheme.Success;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Collection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCollect.Enabled = true;
            }
        }
    }
}
