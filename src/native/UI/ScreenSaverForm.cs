using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Chroniq.Models;
using Chroniq.Native;
using Chroniq.Rendering;

namespace Chroniq.UI
{
    /// <summary>
    /// ScreenSaver Form handling full-screen rendering, mouse movement detection,
    /// mini-preview embedding, and anti-burn-in subpixel orbital drift.
    /// </summary>
    public class ScreenSaverForm : Form
    {
        private Point mouseLocation;
        private bool previewMode = false;
        private bool isTestPreview = false;
        private IntPtr previewParentHwnd = IntPtr.Zero;
        private ClockConfig config;
        private Timer timer;
        private DateTime startTime;

        private DateTime lastConfigCheck = DateTime.MinValue;
        private DateTime lastConfigWriteTime = DateTime.MinValue;

        public ScreenSaverForm(Rectangle bounds, ClockConfig customConfig = null, bool testPreview = false)
        {
            this.isTestPreview = testPreview;
            config = customConfig != null ? customConfig : ClockConfig.Load();
            startTime = DateTime.Now;

            this.BackColor = ColorHelper.ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = bounds;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            Cursor.Hide();
            SetupTimer();
        }

        public ScreenSaverForm(IntPtr previewHandle)
        {
            previewMode = true;
            previewParentHwnd = previewHandle;
            config = ClockConfig.Load();
            startTime = DateTime.Now;

            try
            {
                string path = ClockConfig.GetConfigPath();
                if (File.Exists(path)) lastConfigWriteTime = File.GetLastWriteTimeUtc(path);
            }
            catch { }

            this.BackColor = ColorHelper.ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;

            Win32Interop.SetParent(this.Handle, previewHandle);
            Win32Interop.SetWindowLong(this.Handle, Win32Interop.GWL_STYLE, Win32Interop.GetWindowLong(this.Handle, Win32Interop.GWL_STYLE) | Win32Interop.WS_CHILD | Win32Interop.WS_VISIBLE);

            Rectangle parentRect;
            Win32Interop.GetClientRect(previewHandle, out parentRect);
            this.Bounds = new Rectangle(0, 0, parentRect.Width, parentRect.Height);
            Win32Interop.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, parentRect.Width, parentRect.Height, Win32Interop.SWP_SHOWWINDOW);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);
            this.UpdateStyles();

            SetupTimer();
        }

        private void SetupTimer()
        {
            timer = new Timer();
            timer.Interval = previewMode ? 40 : ((config.ClockMode == "analog" && config.SmoothSweep) ? 16 : 40);
            timer.Tick += (s, e) => {
                if (previewMode)
                {
                    // Check for config updates every 500ms
                    if ((DateTime.Now - lastConfigCheck).TotalMilliseconds > 500)
                    {
                        lastConfigCheck = DateTime.Now;
                        try
                        {
                            string path = ClockConfig.GetConfigPath();
                            if (File.Exists(path))
                            {
                                DateTime wt = File.GetLastWriteTimeUtc(path);
                                if (wt > lastConfigWriteTime)
                                {
                                    lastConfigWriteTime = wt;
                                    this.config = ClockConfig.Load();
                                    this.BackColor = ColorHelper.ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
                                }
                            }
                        }
                        catch { }
                    }
                }
                this.Invalidate();
            };
            timer.Start();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Opaque painting overrides background erase to eliminate flicker completely
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color bgColor = ColorHelper.ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
            g.Clear(bgColor);

            DateTime now = DateTime.Now;
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            if (w <= 0 || h <= 0) return;

            int driftX = 0, driftY = 0;
            if (config.AntiBurnIn && !previewMode)
            {
                double elapsedSec = (now - startTime).TotalSeconds;
                driftX = (int)(Math.Sin(elapsedSec / 120.0 * 2 * Math.PI) * 16.0);
                driftY = (int)(Math.Cos(elapsedSec / 160.0 * 2 * Math.PI) * 14.0);
            }

            if (config.ClockMode == "digital")
            {
                DigitalClockRenderer.Render(g, config, w, h, driftX, driftY, now, previewMode);
            }
            else
            {
                AnalogClockRenderer.Render(g, config, w, h, driftX, driftY, now, previewMode);
            }
        }

        private void ExitScreensaver()
        {
            if (previewMode) return;

            if (isTestPreview)
            {
                Cursor.Show();
                this.Close();
            }
            else
            {
                Cursor.Show();
                Environment.Exit(0);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (previewMode) return;
            if (!mouseLocation.IsEmpty)
            {
                int dx = Math.Abs(mouseLocation.X - e.X);
                int dy = Math.Abs(mouseLocation.Y - e.Y);
                if (dx > 15 || dy > 15)
                {
                    ExitScreensaver();
                }
            }
            mouseLocation = e.Location;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (previewMode) return;
            ExitScreensaver();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (previewMode) return;
            ExitScreensaver();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Cursor.Show();
            base.OnFormClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            Cursor.Show();
            if (disposing)
            {
                if (timer != null) timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
