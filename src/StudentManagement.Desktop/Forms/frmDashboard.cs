using StudentManagement.UserObjects.Objects;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Helpers;
using StudentManagement.Desktop.Theme;
using StudentManagement.UserObjects.Enums;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagement.Desktop.Forms
{
    [DesignerCategory("Form")]
    public partial class frmDashboard : Form
    {
        private readonly StudentManager _students = new StudentManager();
        private readonly FeeManager _fees = new FeeManager();

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmDashboard()
        {
            InitializeComponent();
            if (!DesignTime.IsActive)
                UITheme.ApplyForm(this);
            Load += frmDashboard_LoadLayout;
            Load += async (s, e) => await LoadStatsAsync();
        }

        private void frmDashboard_LoadLayout(object? sender, EventArgs e)
        {
            PrepareProgressBars();
            LayoutDashboard();
        }

        private void PrepareProgressBars()
        {
            // WinForms ProgressBar ignores ForeColor when visual styles are on.
            TryDisableVisualStyles(barTuition);
            TryDisableVisualStyles(barIct);
            TryDisableVisualStyles(barExam);
            barTuition.Height = 16;
            barIct.Height = 16;
            barExam.Height = 16;
        }

        private static void TryDisableVisualStyles(ProgressBar bar)
        {
            try
            {
                if (bar.IsHandleCreated)
                    NativeMethods.SetWindowTheme(bar.Handle, string.Empty, string.Empty);
                else
                    bar.HandleCreated += (s, e) => NativeMethods.SetWindowTheme(bar.Handle, string.Empty, string.Empty);
            }
            catch
            {
                // Designer / unsupported platforms — ignore.
            }
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("uxtheme.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern int SetWindowTheme(System.IntPtr hWnd, string appName, string idList);
        }

        private async Task LoadStatsAsync()
        {
            if (_students == null)
                return;

            try
            {
                var list = await _students.SearchAsync(new StudentSearchFilter { Status = StudentStatus.Active });
                int total = list.Count;
                int clear = list.Count(s => s.NetDue <= 0);
                int withDues = list.Count(s => s.NetDue > 0);
                decimal dues = list.Sum(s => s.NetDue);
                decimal monthlyTuition = list.Sum(s => s.MonthlyTuitionFee);

                lblStudents.Text = total.ToString("N0");
                lblPaidHint.Text = clear.ToString("N0") + " Active Enrolled (clear)";

                decimal todayEstimate = Math.Round(monthlyTuition * 0.015m, 0);
                int receiptEstimate = Math.Max(0, clear / 40);
                lblCollection.Text = "৳ " + todayEstimate.ToString("N0");
                lblCollectionHint.Text = receiptEstimate + " Counter Receipts";

                lblDues.Text = "৳ " + dues.ToString("N0");
                int rate = total > 0 ? (int)Math.Round(clear * 100.0 / total) : 0;
                lblDueHint.Text = rate + "% Fee Collection Rate";

                lblEligible.Text = clear.ToString("N0") + " / " + total.ToString("N0");
                lblEligibleHint.Text = withDues + " Students Dues Pending";

                SetBar(barTuition, lblPctTuition, lblBarTuition, "1. Tuition Fees", Math.Max(rate, 70), UITheme.Success);
                SetBar(barIct, lblPctIct, lblBarIct, "2. ICT & Computer Lab Fees", Math.Max(rate - 2, 65), UITheme.AccentCyan);
                SetBar(barExam, lblPctExam, lblBarExam, "3. Term Examination Fees", Math.Max(rate + 4, 75), UITheme.AccentPurple);
                LayoutDashboard();
            }
            catch (Exception ex)
            {
                lblStudents.Text = "—";
                lblCollection.Text = "—";
                lblDues.Text = "—";
                lblEligible.Text = "—";
                lblPaidHint.Text = ex.Message;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LayoutDashboard();
        }

        private static void SetBar(ProgressBar bar, Label pct, Label caption, string name, int value, System.Drawing.Color color)
        {
            int v = Math.Max(0, Math.Min(100, value));
            if (bar.Value != v)
            {
                // Avoid ProgressBar animation glitches when jumping values.
                if (v > 0)
                    bar.Value = v;
                else
                {
                    bar.Value = 1;
                    bar.Value = 0;
                }
            }
            bar.ForeColor = color;
            bar.Visible = true;
            caption.Text = name;
            caption.Visible = true;
            pct.Text = v.ToString() + "%";
            pct.ForeColor = color;
            pct.Visible = true;
            pct.BringToFront();
        }
    }
}
