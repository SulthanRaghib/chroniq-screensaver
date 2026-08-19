using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Chroniq.Models;

namespace Chroniq.Rendering
{
    /// <summary>
    /// Vector GDI+ rendering engine for Retro-Modern Digital Flip Clocks (Fliqlo aesthetic).
    /// Handles wide flip cards, crisp creases, generous gaps, side hinges, AM/PM indicators, and date badges.
    /// </summary>
    public static class DigitalClockRenderer
    {
        public static void Render(Graphics g, ClockConfig config, int w, int h, int driftX, int driftY, DateTime now, bool previewMode)
        {
            float scale = previewMode ? 0.72f : config.ClockScale;
            float cx = (w / 2f) + driftX;
            float cy = (h / 2f) + driftY;

            // Compute hour and minute strings
            int hourVal = config.Use24Hour ? now.Hour : (now.Hour % 12 == 0 ? 12 : now.Hour % 12);
            string hrStr = hourVal.ToString("00");
            string minStr = now.Minute.ToString("00");
            string secStr = now.Second.ToString("00");
            string ampmStr = now.Hour >= 12 ? "PM" : "AM";

            bool isFlip = config.DigitalStyle == "flip";
            bool showSec = config.ShowDigitalSeconds;

            Color cardBgColor = ColorHelper.ParseColor(config.DialColor, Color.FromArgb(26, 30, 40));
            Color cardBorderColor = ColorHelper.ParseColor(config.BorderColor, Color.FromArgb(48, 56, 70));
            Color digitColor = ColorHelper.ParseColor(config.NumeralsColor, Color.White);
            Color secColor = ColorHelper.ParseColor(config.SecondHandColor, Color.FromArgb(229, 169, 60));

            // Proportions: Wide rectangular cards with generous breathing room
            float baseSize = Math.Min(w, h) * scale;
            float cardH = baseSize * 0.44f;
            float cardW = cardH * 1.15f; // Generous 1.15x width ratio
            float secCardW = cardW * 0.58f;
            float secCardH = cardH * 0.58f;
            float gap = baseSize * 0.055f; // Clean 5.5% separation gap

            float totalW = showSec ? (cardW * 2f + secCardW + gap * 2f) : (cardW * 2f + gap);
            float startX = cx - (totalW / 2f);
            float startY = cy - (cardH / 2f) - (config.ShowDate ? baseSize * 0.04f : 0);

            // 1. Draw Hour Card
            RectangleF hrRect = new RectangleF(startX, startY, cardW, cardH);
            DrawDigitalCard(g, hrRect, hrStr, digitColor, cardBgColor, cardBorderColor, isFlip, config.Use24Hour ? null : ampmStr, previewMode);

            // 2. Draw Minute Card
            RectangleF minRect = new RectangleF(startX + cardW + gap, startY, cardW, cardH);
            DrawDigitalCard(g, minRect, minStr, digitColor, cardBgColor, cardBorderColor, isFlip, null, previewMode);

            // 3. Draw Seconds Card (if enabled)
            if (showSec)
            {
                RectangleF secRect = new RectangleF(startX + cardW * 2f + gap * 2f, startY + (cardH - secCardH), secCardW, secCardH);
                DrawDigitalCard(g, secRect, secStr, secColor, cardBgColor, cardBorderColor, isFlip, null, previewMode);
            }

            // 4. Date Badge (if enabled)
            if (config.ShowDate)
            {
                string dateStr = ColorHelper.GetFormattedDate(now, config.DateFormatLanguage);
                float dateFontSize = Math.Max(4.5f, baseSize * (previewMode ? 0.044f : 0.050f));

                using (Font dateFont = new Font("Segoe UI", dateFontSize, FontStyle.Bold))
                using (Brush dateTextBrush = new SolidBrush(ColorHelper.ParseColor(config.DateTextColor, Color.FromArgb(160, 174, 192))))
                using (Brush dateBgBrush = new SolidBrush(ColorHelper.ParseColor(config.DateBadgeBgColor, Color.FromArgb(26, 32, 44))))
                using (Pen dateBorderPen = new Pen(cardBorderColor, 1f))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    SizeF sz = g.MeasureString(dateStr, dateFont);
                    float bw = sz.Width + baseSize * (previewMode ? 0.06f : 0.09f);
                    float bh = sz.Height + baseSize * 0.025f;
                    float bx = cx - bw / 2f;
                    float by = startY + cardH + baseSize * (previewMode ? 0.045f : 0.065f);

                    GraphicsPath path = ColorHelper.RoundedRect(new RectangleF(bx, by, bw, bh), Math.Max(3f, baseSize * 0.016f));
                    g.FillPath(dateBgBrush, path);
                    g.DrawPath(dateBorderPen, path);
                    g.DrawString(dateStr, dateFont, dateTextBrush, cx, by + bh / 2f, sf);
                }
            }
        }

        private static void DrawDigitalCard(Graphics g, RectangleF rect, string text, Color textColor, Color bgColor, Color borderColor, bool isFlip, string badgeText, bool previewMode)
        {
            float cornerR = Math.Max(4f, rect.Height * 0.09f);

            if (isFlip)
            {
                // Rounded Card Background
                using (GraphicsPath path = ColorHelper.RoundedRect(rect, cornerR))
                using (Brush bgBrush = new SolidBrush(bgColor))
                using (Pen borderPen = new Pen(borderColor, Math.Max(1.2f, rect.Height * 0.014f)))
                {
                    g.FillPath(bgBrush, path);
                    g.DrawPath(borderPen, path);
                }

                // Center Flip Crease / Divider Line
                float midY = rect.Y + (rect.Height / 2f);
                using (Pen darkPen = new Pen(Color.FromArgb(140, 0, 0, 0), Math.Max(1.5f, rect.Height * 0.018f)))
                using (Pen lightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f))
                {
                    g.DrawLine(darkPen, rect.X + 2, midY, rect.Right - 2, midY);
                    g.DrawLine(lightPen, rect.X + 2, midY + 1.2f, rect.Right - 2, midY + 1.2f);
                }

                // Side Hinge Notches
                float notchW = rect.Width * 0.035f;
                float notchH = rect.Height * 0.050f;
                using (Brush notchBrush = new SolidBrush(Color.FromArgb(11, 15, 25)))
                {
                    g.FillRectangle(notchBrush, rect.X - 1, midY - notchH / 2f, notchW, notchH);
                    g.FillRectangle(notchBrush, rect.Right - notchW + 1, midY - notchH / 2f, notchW, notchH);
                }
            }
            else
            {
                // Clean Minimalist Style with translucent card backing and subtle border
                using (GraphicsPath path = ColorHelper.RoundedRect(rect, cornerR))
                using (Brush bgBrush = new SolidBrush(Color.FromArgb(40, bgColor)))
                using (Pen borderPen = new Pen(Color.FromArgb(60, borderColor), 1f))
                {
                    g.FillPath(bgBrush, path);
                    g.DrawPath(borderPen, path);
                }
            }

            // Draw Digits with optimal internal padding
            float fontSize = Math.Max(8f, rect.Height * (previewMode ? 0.52f : 0.54f));
            using (Font font = new Font("Segoe UI", fontSize, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(textColor))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, textBrush, rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f, sf);
            }

            // Draw AM/PM Badge if applicable
            if (!string.IsNullOrEmpty(badgeText))
            {
                float badgeFontSize = Math.Max(5f, rect.Height * (previewMode ? 0.10f : 0.12f));
                using (Font badgeFont = new Font("Segoe UI", badgeFontSize, FontStyle.Bold))
                using (Brush badgeBrush = new SolidBrush(textColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near })
                {
                    g.DrawString(badgeText, badgeFont, badgeBrush, rect.X + rect.Width * 0.08f, rect.Y + rect.Height * 0.07f, sf);
                }
            }
        }
    }
}
