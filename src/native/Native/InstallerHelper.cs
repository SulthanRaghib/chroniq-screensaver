using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace Chroniq.Native
{
    /// <summary>
    /// Helper to install and uninstall Chroniq Screensaver directly into C:\Windows\System32 with seamless UAC elevation.
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
                    // Elevate via cmd.exe which is 100% guaranteed to support the "runas" verb on all Windows systems
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd.exe";
                    psi.Arguments = string.Format(
                        "/c taskkill /f /im Chroniq.scr 2>nul & taskkill /f /im Chroniq.exe 2>nul & del /f /q \"%SystemRoot%\\System32\\PChroniq.scr\" 2>nul & del /f /q \"%SystemRoot%\\SysWOW64\\PChroniq.scr\" 2>nul & copy /y \"{0}\" \"%SystemRoot%\\System32\\Chroniq.scr\" >nul & if exist \"%SystemRoot%\\SysWOW64\" copy /y \"{0}\" \"%SystemRoot%\\SysWOW64\\Chroniq.scr\" >nul & reg add \"HKCU\\Control Panel\\Desktop\" /v SCRNSAVE.EXE /t REG_SZ /d \"%SystemRoot%\\System32\\Chroniq.scr\" /f >nul & reg add \"HKCU\\Control Panel\\Desktop\" /v ScreenSaveActive /t REG_SZ /d 1 /f >nul & start rundll32.exe desk.cpl,InstallScreenSaver \"%SystemRoot%\\System32\\Chroniq.scr\"",
                        currentExe
                    );
                    psi.Verb = "runas";
                    psi.UseShellExecute = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;

                    try
                    {
                        Process p = Process.Start(psi);
                        if (p != null)
                        {
                            p.WaitForExit(5000);
                        }

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
                    catch (Exception)
                    {
                        // User cancelled UAC prompt
                        return false;
                    }
                }

                // Running directly as Administrator:
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

                File.Copy(currentExe, targetScr, true);

                string sysRoot = Environment.GetEnvironmentVariable("SystemRoot");
                if (!string.IsNullOrEmpty(sysRoot))
                {
                    string sysWow64 = Path.Combine(sysRoot, "SysWOW64");
                    if (Directory.Exists(sysWow64))
                    {
                        try { File.Copy(currentExe, Path.Combine(sysWow64, "Chroniq.scr"), true); } catch { }
                    }
                }

                try
                {
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "SCRNSAVE.EXE", targetScr);
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "ScreenSaveActive", "1");
                }
                catch { }

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

        public static bool UninstallFromWindows(bool showSuccessMessage = true)
        {
            try
            {
                if (!IsAdministrator())
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd.exe";
                    psi.Arguments = "/c taskkill /f /im Chroniq.scr 2>nul & taskkill /f /im Chroniq.exe 2>nul & if exist \"%SystemRoot%\\System32\\Chroniq.scr\" del /f /q \"%SystemRoot%\\System32\\Chroniq.scr\" >nul & if exist \"%SystemRoot%\\SysWOW64\\Chroniq.scr\" del /f /q \"%SystemRoot%\\SysWOW64\\Chroniq.scr\" >nul & reg add \"HKCU\\Control Panel\\Desktop\" /v SCRNSAVE.EXE /t REG_SZ /d \"\" /f >nul & reg add \"HKCU\\Control Panel\\Desktop\" /v ScreenSaveActive /t REG_SZ /d 0 /f >nul";
                    psi.Verb = "runas";
                    psi.UseShellExecute = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;

                    try
                    {
                        Process p = Process.Start(psi);
                        if (p != null)
                        {
                            p.WaitForExit(5000);
                        }

                        if (showSuccessMessage)
                        {
                            MessageBox.Show(
                                "Chroniq Screensaver telah berhasil di-uninstall dan dihapus dari sistem Windows.",
                                "Chroniq Uninstaller",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                // Running directly as Administrator:
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

                string systemDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string targetScr = Path.Combine(systemDir, "Chroniq.scr");
                if (File.Exists(targetScr))
                {
                    try { File.Delete(targetScr); } catch { }
                }

                string sysRoot = Environment.GetEnvironmentVariable("SystemRoot");
                if (!string.IsNullOrEmpty(sysRoot))
                {
                    string sysWow64 = Path.Combine(sysRoot, "SysWOW64");
                    string wow64Scr = Path.Combine(sysWow64, "Chroniq.scr");
                    if (File.Exists(wow64Scr))
                    {
                        try { File.Delete(wow64Scr); } catch { }
                    }
                }

                try
                {
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "SCRNSAVE.EXE", "");
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "ScreenSaveActive", "0");
                }
                catch { }

                if (showSuccessMessage)
                {
                    MessageBox.Show(
                        "Chroniq Screensaver telah berhasil di-uninstall dan dihapus dari sistem Windows.",
                        "Chroniq Uninstaller",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencopot screensaver: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
