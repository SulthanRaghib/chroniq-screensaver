using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Chroniq.Rendering
{
    /// <summary>
    /// Utility methods for hex color parsing, graphics paths, and localized date strings.
    /// </summary>
    public static class ColorHelper
    {
        public static Color ParseColor(string hex, Color fallback)
        {
            try
            {
                if (string.IsNullOrEmpty(hex)) return fallback;
                if (hex.StartsWith("#")) hex = hex.Substring(1);
                if (hex.Length == 6)
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    return Color.FromArgb(r, g, b);
                }
            }
            catch { }
            return fallback;
        }

        public static string ColorToHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        public static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float diameter = radius * 2f;
            SizeF size = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static string GetFormattedDate(DateTime now, string lang)
        {
            if (lang == "id")
            {
                string[] days = { "SEN", "SEL", "RAB", "KAM", "JUM", "SAB", "MIN" };
                string[] months = { "JAN", "FEB", "MAR", "APR", "MEI", "JUN", "JUL", "AGU", "SEP", "OKT", "NOV", "DES" };
                int dayIdx = (int)now.DayOfWeek;
                dayIdx = (dayIdx == 0) ? 6 : dayIdx - 1;
                return string.Format("{0} {1} {2}", days[dayIdx], now.Day, months[now.Month - 1]);
            }
            else if (lang == "en")
            {
                string[] days = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
                string[] months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
                int dayIdx = (int)now.DayOfWeek;
                dayIdx = (dayIdx == 0) ? 6 : dayIdx - 1;
                return string.Format("{0} {1} {2}", days[dayIdx], now.Day, months[now.Month - 1]);
            }
            else if (lang == "full_id")
            {
                string[] days = { "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu", "Minggu" };
                string[] months = { "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
                int dayIdx = (int)now.DayOfWeek;
                dayIdx = (dayIdx == 0) ? 6 : dayIdx - 1;
                return string.Format("{0}, {1} {2}", days[dayIdx], now.Day, months[now.Month - 1]);
            }
            else if (lang == "full_en")
            {
                string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                int dayIdx = (int)now.DayOfWeek;
                dayIdx = (dayIdx == 0) ? 6 : dayIdx - 1;
                return string.Format("{0}, {1} {2}", days[dayIdx], now.Day, months[now.Month - 1]);
            }
            else if (lang == "numeric")
            {
                return now.ToString("dd/MM/yyyy");
            }
            else
            {
                // Default System Culture
                try
                {
                    CultureInfo ci = CultureInfo.CurrentCulture;
                    string dayAbbr = ci.DateTimeFormat.GetAbbreviatedDayName(now.DayOfWeek).ToUpper();
                    string monthAbbr = ci.DateTimeFormat.GetAbbreviatedMonthName(now.Month).ToUpper().TrimEnd('.');
                    return string.Format("{0} {1} {2}", dayAbbr, now.Day, monthAbbr);
                }
                catch
                {
                    return now.ToString("ddd d MMM").ToUpper();
                }
            }
        }
    }
}
