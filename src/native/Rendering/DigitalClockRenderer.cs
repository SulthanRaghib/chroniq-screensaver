using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Chroniq.Models;

namespace Chroniq.Rendering
{
    /// <summary>
    /// Vector GDI+ rendering engine for Retro-Modern Digital Flip Clocks (Fliqlo aesthetic).
    /// Features true 3D mechanical split-flap folding animations on second, minute, and hour transitions,
    /// dynamic lighting shadows, center crease lines, side hinges, and date badges.
    /// </summary>
    public static class DigitalClockRenderer
    {
        private class CardFlipState
        {
            public string Prev = null;
            public string Curr = null;
            public long StartMs = 0;
            public int DurationMs = 420;
        }

        private static readonly Stopwatch _sw = Stopwatch.StartNew();
        private static readonly CardFlipState _hourState = new CardFlipState { DurationMs = 450 };
        private static readonly CardFlipState _minState = new CardFlipState { DurationMs = 450 };
        private static readonly CardFlipState _secState = new CardFlipState { DurationMs = 380 };

        public static void Render(Graphics g, ClockConfig config, int w, int h, float driftX, float driftY, DateTime now, bool previewMode)
        {
            float scale = previewMode ? 0.72f : config.ClockScale;
            float cx = (w / 2f) + driftX;
            float cy = (h / 2f) + driftY;

            // Compute hour, minute, and second strings
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
            DrawFlipCard(g, hrRect, _hourState, hrStr, digitColor, cardBgColor, cardBorderColor, isFlip, config.Use24Hour ? null : ampmStr, previewMode);

            // 2. Draw Minute Card
            RectangleF minRect = new RectangleF(startX + cardW + gap, startY, cardW, cardH);
            DrawFlipCard(g, minRect, _minState, minStr, digitColor, cardBgColor, cardBorderColor, isFlip, null, previewMode);

            // 3. Draw Seconds Card (if enabled)
            if (showSec)
            {
                RectangleF secRect = new RectangleF(startX + cardW * 2f + gap * 2f, startY + (cardH - secCardH), secCardW, secCardH);
                DrawFlipCard(g, secRect, _secState, secStr, secColor, cardBgColor, cardBorderColor, isFlip, null, previewMode);
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

        private static void DrawFlipCard(Graphics g, RectangleF rect, CardFlipState state, string newText, Color textColor, Color bgColor, Color borderColor, bool isFlip, string badgeText, bool previewMode)
        {
            long nowMs = _sw.ElapsedMilliseconds;

            if (state.Curr != newText)
            {
                if (state.Curr != null)
                {
                    state.Prev = state.Curr;
                    state.StartMs = nowMs;
                }
                else
                {
                    state.Prev = newText;
                }
                state.Curr = newText;
            }

            long elapsed = nowMs - state.StartMs;
            float progress = state.DurationMs > 0 ? (float)elapsed / state.DurationMs : 1f;
            if (progress > 1f) progress = 1f;

            if (!isFlip || progress >= 1f || state.Prev == state.Curr)
            {
                // Static Card
                DrawHalfCard(g, rect, state.Curr, textColor, bgColor, borderColor, badgeText, true, 1f, 0f, isFlip, previewMode);
                DrawHalfCard(g, rect, state.Curr, textColor, bgColor, borderColor, null, false, 1f, 0f, isFlip, previewMode);
            }
            else
            {
                // 3D Mechanical Flip Transition
                float p = EaseFlip(progress);

                if (p <= 0.5f)
                {
                    float scaleY = (float)Math.Cos(p * Math.PI);
                    float shadow = p * 1.1f;

                    // 1. Static Bottom (shows previous number with shadow darkening)
                    DrawHalfCard(g, rect, state.Prev, textColor, bgColor, borderColor, null, false, 1f, p * 0.45f, isFlip, previewMode);

                    // 2. Static Top Behind (reveals next number)
                    DrawHalfCard(g, rect, state.Curr, textColor, bgColor, borderColor, badgeText, true, 1f, 0f, isFlip, previewMode);

                    // 3. Flipping Top Flap (folds down showing previous number)
                    DrawHalfCard(g, rect, state.Prev, textColor, bgColor, borderColor, badgeText, true, scaleY, shadow, isFlip, previewMode);
                }
                else
                {
                    float scaleY = -(float)Math.Cos(p * Math.PI);
                    float shadow = (1f - p) * 1.1f;

                    // 1. Static Bottom Behind (shows previous number)
                    DrawHalfCard(g, rect, state.Prev, textColor, bgColor, borderColor, null, false, 1f, 0f, isFlip, previewMode);

                    // 2. Static Top (shows next number)
                    DrawHalfCard(g, rect, state.Curr, textColor, bgColor, borderColor, badgeText, true, 1f, 0f, isFlip, previewMode);

                    // 3. Flipping Bottom Flap (drops down showing next number)
                    DrawHalfCard(g, rect, state.Curr, textColor, bgColor, borderColor, null, false, scaleY, shadow, isFlip, previewMode);
                }
            }

            if (isFlip)
            {
                DrawCreaseAndHinges(g, rect);
            }
        }

        private static void DrawHalfCard(Graphics g, RectangleF rect, string text, Color textColor, Color bgColor, Color borderColor, string badgeText, bool isTop, float scaleY, float shadowAlpha, bool isFlip, bool previewMode)
        {
            float cornerR = Math.Max(4f, rect.Height * 0.09f);
            float midY = rect.Y + (rect.Height / 2f);
            float cx = rect.X + (rect.Width / 2f);

            GraphicsState state = g.Save();

            if (scaleY < 0.999f)
            {
                Matrix m = g.Transform;
                m.Translate(cx, midY);
                m.Scale(1f, Math.Max(0.001f, scaleY));
                m.Translate(-cx, -midY);
                g.Transform = m;
            }

            using (GraphicsPath path = isTop ? RoundedTopHalf(rect, cornerR) : RoundedBottomHalf(rect, cornerR))
            {
                g.SetClip(path);

                // Fill card background
                using (Brush bgBrush = new SolidBrush(isFlip ? bgColor : Color.FromArgb(40, bgColor)))
                {
                    g.FillPath(bgBrush, path);
                }

                // Draw border
                using (Pen borderPen = new Pen(isFlip ? borderColor : Color.FromArgb(60, borderColor), Math.Max(1.2f, rect.Height * 0.014f)))
                {
                    g.DrawPath(borderPen, path);
                }

                // Draw digits
                float fontSize = Math.Max(8f, rect.Height * (previewMode ? 0.48f : 0.51f));
                using (Font font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(textColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(text, font, textBrush, cx, rect.Y + rect.Height / 2f, sf);
                }

                // Draw AM/PM badge (top-left pill indicator, 100% clear from digits)
                if (isTop && !string.IsNullOrEmpty(badgeText))
                {
                    float badgeFontSize = Math.Max(4.5f, rect.Height * (previewMode ? 0.070f : 0.078f));
                    using (Font badgeFont = new Font("Segoe UI", badgeFontSize, FontStyle.Bold))
                    using (Brush badgeBrush = new SolidBrush(Color.FromArgb(215, textColor)))
                    using (Brush pillBg = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        SizeF bsz = g.MeasureString(badgeText, badgeFont);
                        float px = rect.X + rect.Width * 0.055f;
                        float py = rect.Y + rect.Height * 0.055f;
                        float pw = bsz.Width + rect.Width * 0.025f;
                        float ph = bsz.Height * 0.88f;

                        using (GraphicsPath pillPath = ColorHelper.RoundedRect(new RectangleF(px, py, pw, ph), Math.Max(2f, rect.Height * 0.015f)))
                        {
                            g.FillPath(pillBg, pillPath);
                        }

                        g.DrawString(badgeText, badgeFont, badgeBrush, px + pw / 2f, py + ph / 2f, sf);
                    }
                }

                // 3D Shadow shading overlay
                if (shadowAlpha > 0.01f)
                {
                    int alpha = (int)Math.Min(215, shadowAlpha * 255);
                    using (Brush shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                    {
                        g.FillPath(shadowBrush, path);
                    }
                }
            }

            g.Restore(state);
        }

        private static void DrawCreaseAndHinges(Graphics g, RectangleF rect)
        {
            float midY = rect.Y + (rect.Height / 2f);

            // Center Flip Crease / Divider Line
            using (Pen darkPen = new Pen(Color.FromArgb(160, 0, 0, 0), Math.Max(1.5f, rect.Height * 0.018f)))
            using (Pen lightPen = new Pen(Color.FromArgb(45, 255, 255, 255), 1f))
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

        private static GraphicsPath RoundedTopHalf(RectangleF rect, float r)
        {
            float midY = rect.Y + (rect.Height / 2f);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, r * 2f, r * 2f, 180, 90);
            path.AddArc(rect.Right - r * 2f, rect.Y, r * 2f, r * 2f, 270, 90);
            path.AddLine(rect.Right, rect.Y + r, rect.Right, midY);
            path.AddLine(rect.Right, midY, rect.X, midY);
            path.AddLine(rect.X, midY, rect.X, rect.Y + r);
            path.CloseFigure();
            return path;
        }

        private static GraphicsPath RoundedBottomHalf(RectangleF rect, float r)
        {
            float midY = rect.Y + (rect.Height / 2f);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(rect.X, midY, rect.Right, midY);
            path.AddLine(rect.Right, midY, rect.Right, rect.Bottom - r);
            path.AddArc(rect.Right - r * 2f, rect.Bottom - r * 2f, r * 2f, r * 2f, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r * 2f, r * 2f, r * 2f, 90, 90);
            path.AddLine(rect.X, rect.Bottom - r, rect.X, midY);
            path.CloseFigure();
            return path;
        }

        private static float EaseFlip(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
        }
    }
}
