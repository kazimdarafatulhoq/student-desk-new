using System;
using System.Configuration;
using System.Windows.Forms;
using StudentManagement.BusinessLayer;
using StudentManagement.Desktop.Forms;
using StudentManagement.Desktop.Helpers;
using StudentManagement.DataAccessLayer;

namespace StudentManagement.Desktop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // Same pattern as DyeERP: App.config drives connection / offline.
            AppSession.OfflineMode = DbConfig.OfflineMode;
            AppSession.InstitutionName =
                ConfigurationManager.AppSettings["InstitutionName"]
                ?? AppSession.InstitutionName;

            if (!AppSession.OfflineMode)
            {
                try
                {
                    new AuthManager().EnsureDemoPasswords();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message +
                        "\n\nOr set OfflineMode=true in App.config to design the UI without SQL Server.",
                        "Database Setup Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            // Form → new Manager() style (no DI container).
            using var login = new frmLogin();
            if (login.ShowDialog() == DialogResult.OK)
            {
                System.Windows.Forms.Application.Run(new frmMainMenu());
            }
        }
    }
}
