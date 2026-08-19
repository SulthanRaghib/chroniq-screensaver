using System;
using System.IO;
using System.Windows.Forms;
using Chroniq.Native;
using Chroniq.UI;

namespace Chroniq
{
    /// <summary>
    /// Application entry point routing Windows screensaver command-line flags (/s, /c, /p)
    /// and smart self-installation prompt when executed outside System32.
    /// </summary>
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    Win32Interop.SetProcessDPIAware();
                }
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArg = args[0].ToLower().Trim();
                string secondArg = args.Length > 1 ? args[1] : null;

                if (firstArg.Contains("uninstall") || firstArg.Contains("remove"))
                {
                    // Direct uninstallation command
                    InstallerHelper.UninstallFromWindows();
                    return;
                }
                else if (firstArg.Contains("install") || firstArg.Contains("setup"))
                {
                    // Direct installation command
                    InstallerHelper.InstallToWindows();
                    return;
                }
                else if (firstArg.StartsWith("/c") || firstArg.Contains("settings") || firstArg.Contains("config"))
                {
                    // Open Native Settings Dialog
                    Application.Run(new SettingsForm());
                    return;
                }
                else if (firstArg.StartsWith("/p"))
                {
                    // Preview mode in screensaver settings mini-display
                    string hwndStr = secondArg;
                    if (string.IsNullOrEmpty(hwndStr) && firstArg.Contains(":"))
                    {
                        string[] parts = firstArg.Split(':');
                        if (parts.Length > 1) hwndStr = parts[1];
                    }

                    if (!string.IsNullOrEmpty(hwndStr))
                    {
                        try
                        {
                            IntPtr previewHandle = new IntPtr(long.Parse(hwndStr));
                            ScreenSaverForm form = new ScreenSaverForm(previewHandle);
                            Win32Interop.ShowWindow(form.Handle, Win32Interop.SW_SHOW);
                            Application.Run(form);
                            return;
                        }
                        catch { }
                    }
                }
                else if (firstArg.StartsWith("/s"))
                {
                    // Fullscreen request. If user double-clicked .SCR from outside System32, offer to install:
                    if (!IsRunningFromSystem32())
                    {
                        DialogResult res = MessageBox.Show(
                            "Apakah Anda ingin memasang Chroniq Screensaver ke sistem Windows sekarang?\n\n" +
                            "• Klik [Yes] untuk memasang ke Windows (Muncul permanen di dropdown)\n" +
                            "• Klik [No] untuk langsung mencoba layar penuh (Preview)",
                            "Chroniq Screensaver Setup",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question
                        );

                        if (res == DialogResult.Yes)
                        {
                            InstallerHelper.InstallToWindows();
                            return;
                        }
                        else if (res == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    RunFullScreen();
                    return;
                }
            }

            // Default execution without flags (e.g. user double-clicked .exe from Downloads or custom folder)
            if (!IsRunningFromSystem32())
            {
                DialogResult res = MessageBox.Show(
                    "Apakah Anda ingin memasang Chroniq Screensaver ke sistem Windows sekarang?\n\n" +
                    "• Klik [Yes] untuk memasang ke Windows (Muncul permanen di dropdown)\n" +
                    "• Klik [No] untuk langsung mencoba layar penuh (Preview)",
                    "Chroniq Screensaver Setup",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (res == DialogResult.Yes)
                {
                    InstallerHelper.InstallToWindows();
                    return;
                }
                else if (res == DialogResult.Cancel)
                {
                    return;
                }
            }

            RunFullScreen();
        }

        public static bool IsRunningFromSystem32()
        {
            try
            {
                string currentDir = Path.GetDirectoryName(Application.ExecutablePath).ToLower();
                string sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower();
                return currentDir.StartsWith(sysDir) || currentDir.Contains("system32") || currentDir.Contains("syswow64");
            }
            catch
            {
                return false;
            }
        }

        private static void RunFullScreen()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                ScreenSaverForm form = new ScreenSaverForm(screen.Bounds);
                form.Show();
            }
            Application.Run();
        }
    }
}
