using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Chroniq.Models;

namespace Chroniq.Rendering
{
    /// <summary>
    /// High-precision Vector GDI+ rendering engine for Luxury Analog Clocks.
    /// Handles continuous trigonometric 60 FPS sweep, markings, numerals, and date badges.
    /// </summary>
    public static class AnalogClockRenderer
    {
        public static void Render(Graphics g, ClockConfig config, int w, int h, float driftX, float driftY, DateTime now, bool previewMode)
        {
            float scale = previewMode ? 0.76f : config.ClockScale;
            float cx = (w / 2f) + driftX;
            float cy = (h / 2f) + driftY;
            float radius = (Math.Min(w, h) / 2f) * scale;
            if (radius < 10) return;

            // 1. Dial Face
            Color dialColor = ColorHelper.ParseColor(config.DialColor, Color.FromArgb(17, 24, 39));
            Color borderColor = ColorHelper.ParseColor(config.BorderColor, Color.FromArgb(31, 41, 55));

            if (config.ShowDialBorder)
            {
                float borderWidth = Math.Max(1.5f, radius * 0.025f);
                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    g.DrawEllipse(borderPen, cx - radius, cy - radius, radius * 2, radius * 2);
                }
            }

            using (Brush dialBrush = new SolidBrush(dialColor))
            {
                g.FillEllipse(dialBrush, cx - radius, cy - radius, radius * 2, radius * 2);
            }

            // 2. Markers
            Color hourMarkerColor = ColorHelper.ParseColor(config.HourMarkersColor, Color.White);
            Color minMarkerColor = ColorHelper.ParseColor(config.MinuteMarkersColor, Color.Gray);
            Color numColor = ColorHelper.ParseColor(config.NumeralsColor, Color.White);

            float hourTickLen = radius * (config.Style == "bauhaus" ? 0.14f : (config.Style == "minimal" ? 0.06f : 0.09f));
            float minTickLen = radius * (config.Style == "bauhaus" ? 0.05f : (config.Style == "minimal" ? 0.025f : 0.04f));

            float hourTickW = Math.Max(1.5f, radius * (config.Style == "bauhaus" ? 0.04f : 0.022f));
            float minTickW = Math.Max(1f, radius * 0.008f);

            for (int i = 0; i < 60; i++)
            {
                double angleDeg = (i * 6.0) - 90.0;
                double rad = angleDeg * Math.PI / 180.0;
                float cosA = (float)Math.Cos(rad);
                float sinA = (float)Math.Sin(rad);

                bool isHour = (i % 5 == 0);
                if (isHour)
                {
                    float rOuter = radius * 0.94f;
                    float rInner = rOuter - hourTickLen;

                    if (config.NumeralType == "dots")
                    {
                        float dotR = Math.Max(1.5f, radius * 0.022f);
                        using (Brush dotBrush = new SolidBrush(hourMarkerColor))
                        {
                            g.FillEllipse(dotBrush, cx + rInner * cosA - dotR, cy + rInner * sinA - dotR, dotR * 2, dotR * 2);
                        }
                    }
                    else
                    {
                        using (Pen p = new Pen(hourMarkerColor, hourTickW))
                        {
                            p.StartCap = LineCap.Round;
                            p.EndCap = LineCap.Round;
                            g.DrawLine(p, cx + rInner * cosA, cy + rInner * sinA, cx + rOuter * cosA, cy + rOuter * sinA);
                        }
                    }
                }
                else
                {
                    if (config.NumeralType != "dots" && config.NumeralType != "lines" || config.Style == "bauhaus" || config.Style == "classic" || config.Style == "modern")
                    {
                        float rOuter = radius * 0.94f;
                        float rInner = rOuter - minTickLen;
                        using (Pen p = new Pen(minMarkerColor, minTickW))
                        {
                            p.StartCap = LineCap.Round;
                            p.EndCap = LineCap.Round;
                            g.DrawLine(p, cx + rInner * cosA, cy + rInner * sinA, cx + rOuter * cosA, cy + rOuter * sinA);
                        }
                    }
                }
            }

            // 3. Numerals (Arabic or Roman)
            if (config.NumeralType == "arabic" || config.NumeralType == "roman")
            {
                bool isRoman = config.NumeralType == "roman";
                string[] romans = { "XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
                string[] arabics = { "12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
                string[] list = isRoman ? romans : arabics;

                float fontSize = Math.Max(6f, radius * (isRoman ? 0.105f : 0.125f));
                string fontName = isRoman || config.Style == "classic" ? "Georgia" : "Segoe UI";
                FontStyle fs = isRoman ? FontStyle.Regular : FontStyle.Bold;

                using (Font font = new Font(fontName, fontSize, fs))
                using (Brush numBrush = new SolidBrush(numColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    float numDist = radius * (isRoman ? 0.76f : 0.74f);
                    for (int idx = 0; idx < 12; idx++)
                    {
                        double angleDeg = (idx * 30.0) - 90.0;
                        double rad = angleDeg * Math.PI / 180.0;
                        float nx = cx + (float)(numDist * Math.Cos(rad));
                        float ny = cy + (float)(numDist * Math.Sin(rad));
                        g.DrawString(list[idx], font, numBrush, nx, ny, sf);
                    }
                }
            }

            // 4. Date Badge (Positioned with optimal clearance from numerals)
            if (config.ShowDate)
            {
                string dateStr = ColorHelper.GetFormattedDate(now, config.DateFormatLanguage);
                bool isRoman = config.NumeralType == "roman";
                float dateFontSize = Math.Max(4.5f, radius * (previewMode ? 0.046f : (isRoman ? 0.050f : 0.054f)));

                using (Font dateFont = new Font("Segoe UI", dateFontSize, FontStyle.Bold))
                using (Brush dateTextBrush = new SolidBrush(ColorHelper.ParseColor(config.DateTextColor, Color.Gray)))
                using (Brush dateBgBrush = new SolidBrush(ColorHelper.ParseColor(config.DateBadgeBgColor, Color.FromArgb(31, 41, 55))))
                using (Pen dateBorderPen = new Pen(borderColor, 1f))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    SizeF sz = g.MeasureString(dateStr, dateFont);
                    float bw = sz.Width + radius * (previewMode ? 0.05f : 0.06f);
                    float bh = sz.Height + radius * 0.015f;
                    float dateY = cy + radius * (isRoman ? 0.32f : 0.36f);
                    float bx = cx - bw / 2f;
                    float by = dateY - bh / 2f;

                    GraphicsPath path = ColorHelper.RoundedRect(new RectangleF(bx, by, bw, bh), Math.Max(2f, radius * 0.015f));
                    g.FillPath(dateBgBrush, path);
                    g.DrawPath(dateBorderPen, path);
                    g.DrawString(dateStr, dateFont, dateTextBrush, cx, dateY, sf);
                }
            }

            // 5. Hand Angles Calculation
            double ms = config.SmoothSweep ? now.Millisecond : 0;
            double secFrac = now.Second + (ms / 1000.0);
            double minFrac = now.Minute + (secFrac / 60.0);
            double hrFrac = (now.Hour % 12) + (minFrac / 60.0);

            double secAngle = (secFrac * 6.0) - 90.0;
            double minAngle = (minFrac * 6.0) - 90.0;
            double hrAngle = (hrFrac * 30.0) - 90.0;

            Color hrHandColor = ColorHelper.ParseColor(config.HourHandColor, Color.White);
            Color minHandColor = ColorHelper.ParseColor(config.MinuteHandColor, Color.LightGray);
            Color secHandColor = ColorHelper.ParseColor(config.SecondHandColor, Color.Red);
            Color accentColor = ColorHelper.ParseColor(config.AccentCenterColor, Color.Red);

            // 6. Draw Hands
            if (config.ShowHourHand)
            {
                DrawHand(g, cx, cy, hrAngle, radius * 0.50f, radius * 0.12f, radius * 0.036f, hrHandColor, config.Style);
            }

            if (config.ShowMinuteHand)
            {
                DrawHand(g, cx, cy, minAngle, radius * 0.78f, radius * 0.14f, radius * 0.024f, minHandColor, config.Style);
            }

            if (config.ShowSecondHand)
            {
                DrawSecondHand(g, cx, cy, secAngle, radius * 0.85f, radius * 0.18f, radius, secHandColor, config.Style);
            }

            // 7. Center Pin / Cap
            float capR = Math.Max(3f, radius * 0.032f);
            using (Brush capBrush = new SolidBrush(accentColor))
            {
                g.FillEllipse(capBrush, cx - capR, cy - capR, capR * 2, capR * 2);
            }
        }

        private static void DrawHand(Graphics g, float cx, float cy, double angleDeg, float length, float counterLen, float width, Color color, string style)
        {
            double rad = angleDeg * Math.PI / 180.0;
            float cosA = (float)Math.Cos(rad);
            float sinA = (float)Math.Sin(rad);

            float nx = -sinA * (width / 2f);
            float ny = cosA * (width / 2f);

            PointF tip = new PointF(cx + length * cosA, cy + length * sinA);
            PointF tail = new PointF(cx - counterLen * cosA, cy - counterLen * sinA);

            PointF p1 = new PointF(tail.X + nx * 0.7f, tail.Y + ny * 0.7f);
            PointF p2 = new PointF(tip.X + nx * 0.3f, tip.Y + ny * 0.3f);
            PointF p3 = tip;
            PointF p4 = new PointF(tip.X - nx * 0.3f, tip.Y - ny * 0.3f);
            PointF p5 = new PointF(tail.X - nx * 0.7f, tail.Y - ny * 0.7f);

            using (Brush brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, new PointF[] { p1, p2, p3, p4, p5 });
            }
        }

        private static void DrawSecondHand(Graphics g, float cx, float cy, double angleDeg, float length, float counterLen, float radius, Color color, string style)
        {
            double rad = angleDeg * Math.PI / 180.0;
            float cosA = (float)Math.Cos(rad);
            float sinA = (float)Math.Sin(rad);

            PointF tip = new PointF(cx + length * cosA, cy + length * sinA);
            PointF tail = new PointF(cx - counterLen * cosA, cy - counterLen * sinA);

            float lineW = Math.Max(1.5f, radius * 0.009f);
            using (Pen pen = new Pen(color, lineW))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, tail, tip);
            }

            if (style == "bauhaus")
            {
                float discDist = length * 0.78f;
                float discX = cx + discDist * cosA;
                float discY = cy + discDist * sinA;
                float discR = Math.Max(4f, radius * 0.05f);
                using (Brush discBrush = new SolidBrush(color))
                {
                    g.FillEllipse(discBrush, discX - discR, discY - discR, discR * 2, discR * 2);
                }
            }
            else
            {
                float cwX = cx - counterLen * 0.65f * cosA;
                float cwY = cy - counterLen * 0.65f * sinA;
                float cwR = Math.Max(2f, radius * 0.022f);
                using (Brush cwBrush = new SolidBrush(color))
                {
                    g.FillEllipse(cwBrush, cwX - cwR, cwY - cwR, cwR * 2, cwR * 2);
                }
            }
        }
    }
}
