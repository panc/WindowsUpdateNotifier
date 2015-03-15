using System;
using System.Security.Principal;

namespace WindowsUpdateNotifier
{
    public class UacHelper
    {
        public static bool IsRunningAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsRunningOnWindows7()
        {
            return Environment.OSVersion.Version.Major == 6 &&
                   Environment.OSVersion.Version.Minor == 1;
        }
    }
}