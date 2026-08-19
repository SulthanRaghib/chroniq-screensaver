using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace Chroniq.Native
{
    /// <summary>
    /// Helper to install Chroniq Screensaver directly into C:\Windows\System32 with seamless UAC elevation.
    /// </summary>
    public static class InstallerHelper
    {
        public static bool IsAdministrator()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool InstallToWindows(bool showSuccessMessage = true)
        {
            try
            {
                string currentExe = Application.ExecutablePath;
                string systemDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string targetScr = Path.Combine(systemDir, "Chroniq.scr");

                if (!IsAdministrator())
                {
                    // Relaunch self with UAC elevation
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = currentExe;
                    psi.Arguments = "/install";
                    psi.Verb = "runas";
                    psi.UseShellExecute = true;

                    try
                    {
                        Process.Start(psi);
                        return true;
                    }
                    catch (Exception)
                    {
                        // User cancelled UAC prompt
                        return false;
                    }
                }

                // Running as Administrator:
                // 1. Terminate running screensavers
                try
                {
                    foreach (var proc in Process.GetProcessesByName("Chroniq"))
                    {
                        if (proc.Id != Process.GetCurrentProcess().Id)
                        {
                            try { proc.Kill(); } catch { }
                        }
                    }
                }
                catch { }

                // 2. Copy to System32
                File.Copy(currentExe, targetScr, true);

                // 3. Copy to SysWOW64 if present on 64-bit Windows
                string sysRoot = Environment.GetEnvironmentVariable("SystemRoot");
                if (!string.IsNullOrEmpty(sysRoot))
                {
                    string sysWow64 = Path.Combine(sysRoot, "SysWOW64");
                    if (Directory.Exists(sysWow64))
                    {
                        try { File.Copy(currentExe, Path.Combine(sysWow64, "Chroniq.scr"), true); } catch { }
                    }
                }

                // 4. Register in Registry
                try
                {
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "SCRNSAVE.EXE", targetScr);
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "ScreenSaveActive", "1");
                }
                catch { }

                // 5. Open Windows Screen Saver Settings
                try
                {
                    Process.Start("rundll32.exe", "desk.cpl,InstallScreenSaver \"" + targetScr + "\"");
                }
                catch { }

                if (showSuccessMessage)
                {
                    MessageBox.Show(
                        "Chroniq Screensaver berhasil dipasang ke sistem Windows!\n\n" +
                        "Nama 'Chroniq' sekarang muncul permanen di menu dropdown Screen Saver Windows.",
                        "Chroniq Installer",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memasang screensaver: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
