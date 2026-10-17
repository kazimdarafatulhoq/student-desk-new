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
        private StudentManager? _students;
        private StudentManager Students => _students ??= new StudentManager();
        private FeeManager? _fees;
        private FeeManager Fees => _fees ??= new FeeManager();

        /// <summary>Parameterless constructor required by the WinForms designer.</summary>
        public frmDashboard()
        {
            InitializeComponent();
            if (DesignTime.IsActive)
                return;
            UITheme.ApplyForm(this);
            lblSubtitle.Text = "Institutional overview for " + AppSession.InstitutionName + ".";
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
            if (DesignTime.IsActive)
                return;

            try
            {
                // Live KPIs from usp_GetDashboardStats (FeePayments / ledger / students).
                var stats = await Fees.GetDashboardStatsAsync();

                lblStudents.Text = stats.ActiveStudents.ToString("N0");
                lblPaidHint.Text = stats.ClearStudents.ToString("N0") + " Active Enrolled (clear)";

                lblCollection.Text = "৳ " + stats.CollectionsToday.ToString("N0");
                lblCollectionHint.Text = stats.ReceiptsToday.ToString("N0") + " Counter Receipts";

                lblDues.Text = "৳ " + stats.TotalReceivables.ToString("N0");
                lblDueHint.Text = stats.FeeCollectionRatePct + "% Fee Collection Rate";

                lblEligible.Text = stats.ClearStudents.ToString("N0") + " / " + stats.ActiveStudents.ToString("N0");
                lblEligibleHint.Text = stats.StudentsWithDues + " Students Dues Pending";

                decimal categoryTotal = stats.TuitionInvoiced + stats.IctInvoiced + stats.ExamInvoiced;
                SetBar(barTuition, lblPctTuition, lblBarTuition, "1. Tuition Fees",
                    SharePct(stats.TuitionInvoiced, categoryTotal), UITheme.Success);
                SetBar(barIct, lblPctIct, lblBarIct, "2. ICT & Computer Lab Fees",
                    SharePct(stats.IctInvoiced, categoryTotal), UITheme.AccentCyan);
                SetBar(barExam, lblPctExam, lblBarExam, "3. Term Examination Fees",
                    SharePct(stats.ExamInvoiced, categoryTotal), UITheme.AccentPurple);
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

        private static int SharePct(decimal part, decimal total)
        {
            if (total <= 0m) return 0;
            return (int)Math.Round(part * 100m / total);
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
