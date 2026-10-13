using System;
using System.ComponentModel;
using System.Diagnostics;

namespace StudentManagement.Desktop.Helpers
{
    /// <summary>
    /// Detects Visual Studio WinForms design-time so constructors can skip runtime-only setup.
    /// .NET WinForms designer often runs in DesignToolsServer — check both LicenseManager and process.
    /// </summary>
    public static class DesignTime
    {
        public static bool IsActive
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;

                try
                {
                    var name = Process.GetCurrentProcess().ProcessName;
                    return name.Equals("devenv", StringComparison.OrdinalIgnoreCase)
                           || name.Equals("DesignToolsServer", StringComparison.OrdinalIgnoreCase)
                           || name.Equals("XDesProc", StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
