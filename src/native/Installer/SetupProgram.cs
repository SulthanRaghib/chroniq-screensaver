using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Chroniq.Installer
{
    public static class SetupProgram
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string arg = args[0].ToLower().Trim();
                if (arg.Contains("uninstall") || arg.Contains("remove"))
                {
                    PerformUninstall(silent: false);
                    return;
                }
                else if (arg.Contains("silent") || arg.Contains("quiet"))
                {
                    PerformInstall(silent: true);
                    return;
                }
            }

            // Show Sleek GUI Installer Dialog
            Application.Run(new SetupForm());
        }

        public static bool PerformInstall(bool silent = false)
        {
            try
            {
                // 1. Force kill all running screensaver instances and preview processes
                ProcessStartInfo psiKill = new ProcessStartInfo();
                psiKill.FileName = "cmd.exe";
                psiKill.Arguments = "/c taskkill /f /im Chroniq.scr 2>nul & taskkill /f /im Chroniq.exe 2>nul & taskkill /f /im rundll32.exe 2>nul & del /f /q \"%SystemRoot%\\System32\\PChroniq.scr\" 2>nul & del /f /q \"%SystemRoot%\\SysWOW64\\PChroniq.scr\" 2>nul";
                psiKill.UseShellExecute = false;
                psiKill.CreateNoWindow = true;
                psiKill.WindowStyle = ProcessWindowStyle.Hidden;

                Process pKill = Process.Start(psiKill);
                if (pKill != null) pKill.WaitForExit(3000);

                // 2. Extract embedded Chroniq.scr or copy companion
                byte[] scrBytes = null;
                Assembly asm = Assembly.GetExecutingAssembly();
                using (Stream stream = asm.GetManifestResourceStream("Chroniq.scr"))
                {
                    if (stream != null)
                    {
                        scrBytes = new byte[stream.Length];
                        stream.Read(scrBytes, 0, scrBytes.Length);
                    }
                }

                string sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string targetScr = Path.Combine(sysDir, "Chroniq.scr");

                if (scrBytes != null && scrBytes.Length > 0)
                {
                    File.WriteAllBytes(targetScr, scrBytes);
                }
                else
                {
                    string localScr = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Chroniq.scr");
                    if (File.Exists(localScr))
                    {
                        File.Copy(localScr, targetScr, true);
                    }
                }

                // Copy to SysWOW64 if 64-bit Windows
                string sysRoot = Environment.GetEnvironmentVariable("SystemRoot");
                if (!string.IsNullOrEmpty(sysRoot))
                {
                    string sysWow64 = Path.Combine(sysRoot, "SysWOW64");
                    if (Directory.Exists(sysWow64))
                    {
                        string wow64Target = Path.Combine(sysWow64, "Chroniq.scr");
                        if (scrBytes != null && scrBytes.Length > 0)
                        {
                            try { File.WriteAllBytes(wow64Target, scrBytes); } catch { }
                        }
                    }
                }

                // 3. Register in Windows Registry
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "SCRNSAVE.EXE", targetScr);
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "ScreenSaveActive", "1");

                // 4. Open Screen Saver Settings dialog from System32 working directory
                try
                {
                    ProcessStartInfo psiOpen = new ProcessStartInfo();
                    psiOpen.FileName = "cmd.exe";
                    psiOpen.Arguments = string.Format("/c start \"\" /d \"%SystemRoot%\\System32\" rundll32.exe desk.cpl,InstallScreenSaver \"{0}\"", targetScr);
                    psiOpen.UseShellExecute = false;
                    psiOpen.CreateNoWindow = true;
                    Process.Start(psiOpen);
                }
                catch { }

                if (!silent)
                {
                    MessageBox.Show(
                        "Chroniq Screensaver telah berhasil dipasang ke sistem Windows!\n\nNama 'Chroniq' kini aktif dan muncul permanen di menu screensaver Windows.",
                        "Instalasi Berhasil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show("Gagal memasang screensaver: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public static bool PerformUninstall(bool silent = false)
        {
            try
            {
                // 1. Force kill all running screensaver instances, preview windows, and stale dialogs
                ProcessStartInfo psiKill = new ProcessStartInfo();
                psiKill.FileName = "cmd.exe";
                psiKill.Arguments = "/c taskkill /f /im Chroniq.scr 2>nul & taskkill /f /im Chroniq.exe 2>nul & taskkill /f /im rundll32.exe 2>nul & del /f /q \"%SystemRoot%\\System32\\Chroniq.scr\" 2>nul & del /f /q \"%SystemRoot%\\SysWOW64\\Chroniq.scr\" 2>nul & del /f /q \"%SystemRoot%\\System32\\PChroniq.scr\" 2>nul & del /f /q \"%SystemRoot%\\SysWOW64\\PChroniq.scr\" 2>nul & del /f /q \"%SystemRoot%\\System32\\AnalogClock.scr\" 2>nul & del /f /q \"%SystemRoot%\\SysWOW64\\AnalogClock.scr\" 2>nul & reg add \"HKCU\\Control Panel\\Desktop\" /v SCRNSAVE.EXE /t REG_SZ /d \"\" /f >nul & reg add \"HKCU\\Control Panel\\Desktop\" /v ScreenSaveActive /t REG_SZ /d 0 /f >nul";
                psiKill.UseShellExecute = false;
                psiKill.CreateNoWindow = true;
                psiKill.WindowStyle = ProcessWindowStyle.Hidden;

                Process p = Process.Start(psiKill);
                if (p != null)
                {
                    p.WaitForExit(4000);
                }

                // 2. Open fresh Screen Saver Settings dialog from System32
                try
                {
                    ProcessStartInfo psiOpen = new ProcessStartInfo();
                    psiOpen.FileName = "cmd.exe";
                    psiOpen.Arguments = "/c start \"\" /d \"%SystemRoot%\\System32\" rundll32.exe desk.cpl,InstallScreenSaver";
                    psiOpen.UseShellExecute = false;
                    psiOpen.CreateNoWindow = true;
                    Process.Start(psiOpen);
                }
                catch { }

                if (!silent)
                {
                    MessageBox.Show(
                        "Chroniq Screensaver telah berhasil di-uninstall dan dihapus bersih dari sistem Windows.\n\nPilihan screensaver kini kembali ke (None).",
                        "Uninstall Berhasil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show("Gagal mencopot screensaver: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }
    }

    public class SetupForm : Form
    {
        public SetupForm()
        {
            this.Text = "Chroniq Screensaver — Windows Setup";
            this.Size = new Size(480, 360);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(11, 15, 25);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);

            // Title Label
            Label lblTitle = new Label();
            lblTitle.Text = "CHRONIQ SCREENSAVER";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 240, 255);
            lblTitle.Location = new Point(20, 25);
            lblTitle.Size = new Size(440, 35);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Subtitle
            Label lblSub = new Label();
            lblSub.Text = "Dual-Engine: Luxury Analog & Retro-Modern Digital Flip Clock";
            lblSub.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblSub.ForeColor = Color.FromArgb(156, 163, 175);
            lblSub.Location = new Point(20, 65);
            lblSub.Size = new Size(440, 25);
            lblSub.TextAlign = ContentAlignment.MiddleCenter;

            // Info Box Panel
            Panel infoPanel = new Panel();
            infoPanel.Location = new Point(30, 100);
            infoPanel.Size = new Size(405, 90);
            infoPanel.BackColor = Color.FromArgb(18, 24, 38);
            infoPanel.BorderStyle = BorderStyle.FixedSingle;

            Label lblInfo = new Label();
            lblInfo.Text = "• 1-Klik Pemasangan ke sistem Windows (System32)\n• Nama resmi 'Chroniq' muncul permanen di menu screensaver\n• Mendukung kustomisasi tema, warna, dan proteksi Anti-Burn-In";
            lblInfo.Font = new Font("Segoe UI", 8.5F);
            lblInfo.ForeColor = Color.FromArgb(209, 213, 219);
            lblInfo.Location = new Point(12, 12);
            lblInfo.Size = new Size(380, 65);
            infoPanel.Controls.Add(lblInfo);

            // Install Button (Primary)
            Button btnInstall = new Button();
            btnInstall.Text = "📥 Pasang Screensaver (1-Click Install)";
            btnInstall.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnInstall.BackColor = Color.FromArgb(37, 99, 235);
            btnInstall.ForeColor = Color.White;
            btnInstall.FlatStyle = FlatStyle.Flat;
            btnInstall.FlatAppearance.BorderSize = 0;
            btnInstall.Location = new Point(30, 210);
            btnInstall.Size = new Size(405, 45);
            btnInstall.Cursor = Cursors.Hand;
            btnInstall.Click += (s, e) => {
                if (SetupProgram.PerformInstall(silent: false))
                {
                    this.Close();
                }
            };

            // Uninstall Button (Secondary)
            Button btnUninstall = new Button();
            btnUninstall.Text = "🗑️ Copot (Uninstall)";
            btnUninstall.Font = new Font("Segoe UI", 8.5F);
            btnUninstall.BackColor = Color.FromArgb(31, 41, 55);
            btnUninstall.ForeColor = Color.FromArgb(239, 68, 68);
            btnUninstall.FlatStyle = FlatStyle.Flat;
            btnUninstall.FlatAppearance.BorderSize = 0;
            btnUninstall.Location = new Point(30, 268);
            btnUninstall.Size = new Size(195, 34);
            btnUninstall.Cursor = Cursors.Hand;
            btnUninstall.Click += (s, e) => {
                SetupProgram.PerformUninstall(silent: false);
            };

            // Close Button
            Button btnClose = new Button();
            btnClose.Text = "Tutup";
            btnClose.Font = new Font("Segoe UI", 8.5F);
            btnClose.BackColor = Color.FromArgb(31, 41, 55);
            btnClose.ForeColor = Color.FromArgb(156, 163, 175);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Location = new Point(240, 268);
            btnClose.Size = new Size(195, 34);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSub);
            this.Controls.Add(infoPanel);
            this.Controls.Add(btnInstall);
            this.Controls.Add(btnUninstall);
            this.Controls.Add(btnClose);
        }
    }
}
