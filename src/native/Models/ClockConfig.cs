using System;
using System.IO;

namespace Chroniq.Models
{
    /// <summary>
    /// Configuration model for Chroniq Screensaver supporting dual-mode (Analog & Digital)
    /// and persistent JSON serialization with legacy fallback.
    /// </summary>
    public class ClockConfig
    {
        // Core Mode
        public string ClockMode = "analog"; // "analog" or "digital"

        // Preset & Analog Styles
        public string PresetName = "Modern Dark";
        public string Style = "modern"; // "modern", "classic", "bauhaus", "sport", "minimal"
        public string NumeralType = "arabic"; // "arabic", "roman", "dots", "lines", "none"

        // Digital Options
        public string DigitalStyle = "flip"; // "flip" (Fliqlo) or "minimal"
        public bool Use24Hour = true;
        public bool ShowDigitalSeconds = true;

        // Analog Hands
        public bool ShowHourHand = true;
        public bool ShowMinuteHand = true;
        public bool ShowSecondHand = true;
        public bool SmoothSweep = true;

        // Common Features
        public bool ShowDate = true;
        public bool ShowDialBorder = true;
        public bool AntiBurnIn = true;
        public float ClockScale = 0.72f;
        public string DateFormatLanguage = "system"; // "system", "id", "en", "full_id", "full_en", "numeric"

        // Color Palettes
        public string BgColor = "#0B0F19";
        public string DialColor = "#111827";
        public string BorderColor = "#1F2937";
        public string HourMarkersColor = "#F3F4F6";
        public string MinuteMarkersColor = "#4B5563";
        public string NumeralsColor = "#F9FAFB";
        public string HourHandColor = "#F9FAFB";
        public string MinuteHandColor = "#E5E7EB";
        public string SecondHandColor = "#EF4444";
        public string AccentCenterColor = "#EF4444";
        public string DateTextColor = "#9CA3AF";
        public string DateBadgeBgColor = "#1F2937";

        public static string GetConfigPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(appData, "ChroniqScreensaver");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string modernPath = Path.Combine(dir, "chroniq_config.json");

            // Backwards compatibility fallback if legacy file exists
            if (!File.Exists(modernPath))
            {
                string legacyPath = Path.Combine(appData, "AnalogClockScreensaver", "clock_config.json");
                if (File.Exists(legacyPath))
                {
                    try { File.Copy(legacyPath, modernPath, true); } catch { }
                }
            }
            return modernPath;
        }

        public static ClockConfig Load()
        {
            try
            {
                string path = GetConfigPath();
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    return ParseJson(json);
                }
            }
            catch { }
            return new ClockConfig();
        }

        public void Save()
        {
            try
            {
                string json = ToJson();
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // 1. Modern Chroniq path
                string dir1 = Path.Combine(appData, "ChroniqScreensaver");
                if (!Directory.Exists(dir1)) Directory.CreateDirectory(dir1);
                File.WriteAllText(Path.Combine(dir1, "chroniq_config.json"), json);

                // 2. Legacy compatibility path
                string dir2 = Path.Combine(appData, "AnalogClockScreensaver");
                if (!Directory.Exists(dir2)) Directory.CreateDirectory(dir2);
                File.WriteAllText(Path.Combine(dir2, "clock_config.json"), json);
            }
            catch { }
        }

        public void ApplyPreset(string preset)
        {
            this.PresetName = preset;
            if (preset == "Modern Dark")
            {
                Style = "modern"; NumeralType = "arabic";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = true; ShowDialBorder = true;
                BgColor = "#0B0F19"; DialColor = "#111827"; BorderColor = "#1F2937";
                HourMarkersColor = "#F3F4F6"; MinuteMarkersColor = "#4B5563"; NumeralsColor = "#F9FAFB";
                HourHandColor = "#F9FAFB"; MinuteHandColor = "#E5E7EB"; SecondHandColor = "#EF4444";
                AccentCenterColor = "#EF4444"; DateTextColor = "#9CA3AF"; DateBadgeBgColor = "#1F2937";
            }
            else if (preset == "Fliqlo Monochrome")
            {
                Style = "modern"; NumeralType = "arabic";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = false; ShowDate = true; ShowDialBorder = false;
                BgColor = "#0D0D0D"; DialColor = "#181818"; BorderColor = "#282828";
                HourMarkersColor = "#E0E0E0"; MinuteMarkersColor = "#505050"; NumeralsColor = "#FFFFFF";
                HourHandColor = "#FFFFFF"; MinuteHandColor = "#D0D0D0"; SecondHandColor = "#E5A93C";
                AccentCenterColor = "#E5A93C"; DateTextColor = "#888888"; DateBadgeBgColor = "#222222";
            }
            else if (preset == "Classic Vintage Roman")
            {
                Style = "classic"; NumeralType = "roman";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = true; ShowDialBorder = true;
                BgColor = "#121110"; DialColor = "#F7F3E9"; BorderColor = "#C5A059";
                HourMarkersColor = "#2C2A29"; MinuteMarkersColor = "#736F6D"; NumeralsColor = "#1E1C1B";
                HourHandColor = "#1E1C1B"; MinuteHandColor = "#2C2A29"; SecondHandColor = "#8B1E1E";
                AccentCenterColor = "#C5A059"; DateTextColor = "#3D3937"; DateBadgeBgColor = "#E8E2D2";
            }
            else if (preset == "Swiss Railway (Bauhaus)")
            {
                Style = "bauhaus"; NumeralType = "none";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = false; ShowDate = true; ShowDialBorder = true;
                BgColor = "#18181B"; DialColor = "#FFFFFF"; BorderColor = "#E4E4E7";
                HourMarkersColor = "#09090B"; MinuteMarkersColor = "#71717A"; NumeralsColor = "#09090B";
                HourHandColor = "#09090B"; MinuteHandColor = "#09090B"; SecondHandColor = "#DC2626";
                AccentCenterColor = "#DC2626"; DateTextColor = "#52525B"; DateBadgeBgColor = "#F4F4F5";
            }
            else if (preset == "Midnight Sapphire")
            {
                Style = "modern"; NumeralType = "dots";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = true; ShowDialBorder = true;
                BgColor = "#030712"; DialColor = "#0B1528"; BorderColor = "#1E3A8A";
                HourMarkersColor = "#60A5FA"; MinuteMarkersColor = "#1E40AF"; NumeralsColor = "#93C5FD";
                HourHandColor = "#F0F9FF"; MinuteHandColor = "#BAE6FD"; SecondHandColor = "#38BDF8";
                AccentCenterColor = "#38BDF8"; DateTextColor = "#7DD3FC"; DateBadgeBgColor = "#0F2445";
            }
            else if (preset == "Cyberpunk Neon")
            {
                Style = "sport"; NumeralType = "arabic";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = true; ShowDialBorder = true;
                BgColor = "#050508"; DialColor = "#0D0D14"; BorderColor = "#06B6D4";
                HourMarkersColor = "#00F0FF"; MinuteMarkersColor = "#4338CA"; NumeralsColor = "#00F0FF";
                HourHandColor = "#F43F5E"; MinuteHandColor = "#FB7185"; SecondHandColor = "#00F0FF";
                AccentCenterColor = "#F43F5E"; DateTextColor = "#00F0FF"; DateBadgeBgColor = "#1E1B4B";
            }
            else if (preset == "Minimal Slate")
            {
                Style = "minimal"; NumeralType = "lines";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = true; ShowDialBorder = false;
                BgColor = "#0F172A"; DialColor = "#1E293B"; BorderColor = "#334155";
                HourMarkersColor = "#94A3B8"; MinuteMarkersColor = "#475569"; NumeralsColor = "#CBD5E1";
                HourHandColor = "#F8FAFC"; MinuteHandColor = "#94A3B8"; SecondHandColor = "#38BDF8";
                AccentCenterColor = "#F8FAFC"; DateTextColor = "#64748B"; DateBadgeBgColor = "#334155";
            }
            else if (preset == "Emerald Luxury")
            {
                Style = "classic"; NumeralType = "roman";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = true; ShowDialBorder = true;
                BgColor = "#04110B"; DialColor = "#062317"; BorderColor = "#D4AF37";
                HourMarkersColor = "#F5E6BE"; MinuteMarkersColor = "#1B4D3E"; NumeralsColor = "#F5E6BE";
                HourHandColor = "#D4AF37"; MinuteHandColor = "#F3E5AB"; SecondHandColor = "#E63946";
                AccentCenterColor = "#D4AF37"; DateTextColor = "#D4AF37"; DateBadgeBgColor = "#0B3826";
            }
        }

        public string ToJson()
        {
            return "{\n" +
                "  \"clock_mode\": \"" + ClockMode + "\",\n" +
                "  \"preset_name\": \"" + PresetName + "\",\n" +
                "  \"style\": \"" + Style + "\",\n" +
                "  \"numeral_type\": \"" + NumeralType + "\",\n" +
                "  \"digital_style\": \"" + DigitalStyle + "\",\n" +
                "  \"use_24_hour\": " + Use24Hour.ToString().ToLower() + ",\n" +
                "  \"show_digital_seconds\": " + ShowDigitalSeconds.ToString().ToLower() + ",\n" +
                "  \"show_hour_hand\": " + ShowHourHand.ToString().ToLower() + ",\n" +
                "  \"show_minute_hand\": " + ShowMinuteHand.ToString().ToLower() + ",\n" +
                "  \"show_second_hand\": " + ShowSecondHand.ToString().ToLower() + ",\n" +
                "  \"smooth_sweep\": " + SmoothSweep.ToString().ToLower() + ",\n" +
                "  \"show_date\": " + ShowDate.ToString().ToLower() + ",\n" +
                "  \"show_dial_border\": " + ShowDialBorder.ToString().ToLower() + ",\n" +
                "  \"anti_burn_in\": " + AntiBurnIn.ToString().ToLower() + ",\n" +
                "  \"clock_scale\": " + ClockScale.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",\n" +
                "  \"date_format_language\": \"" + DateFormatLanguage + "\",\n" +
                "  \"bg_color\": \"" + BgColor + "\",\n" +
                "  \"dial_color\": \"" + DialColor + "\",\n" +
                "  \"border_color\": \"" + BorderColor + "\",\n" +
                "  \"hour_markers_color\": \"" + HourMarkersColor + "\",\n" +
                "  \"minute_markers_color\": \"" + MinuteMarkersColor + "\",\n" +
                "  \"numerals_color\": \"" + NumeralsColor + "\",\n" +
                "  \"hour_hand_color\": \"" + HourHandColor + "\",\n" +
                "  \"minute_hand_color\": \"" + MinuteHandColor + "\",\n" +
                "  \"second_hand_color\": \"" + SecondHandColor + "\",\n" +
                "  \"accent_center_color\": \"" + AccentCenterColor + "\",\n" +
                "  \"date_text_color\": \"" + DateTextColor + "\",\n" +
                "  \"date_badge_bg_color\": \"" + DateBadgeBgColor + "\"\n" +
                "}";
        }

        public static ClockConfig ParseJson(string json)
        {
            ClockConfig c = new ClockConfig();
            c.ClockMode = GetJsonVal(json, "clock_mode", "analog");
            c.PresetName = GetJsonVal(json, "preset_name", "Modern Dark");
            c.Style = GetJsonVal(json, "style", "modern");
            c.NumeralType = GetJsonVal(json, "numeral_type", "arabic");
            c.DigitalStyle = GetJsonVal(json, "digital_style", "flip");

            c.Use24Hour = GetJsonBool(json, "use_24_hour", true);
            c.ShowDigitalSeconds = GetJsonBool(json, "show_digital_seconds", true);

            c.ShowHourHand = GetJsonBool(json, "show_hour_hand", true);
            c.ShowMinuteHand = GetJsonBool(json, "show_minute_hand", true);
            c.ShowSecondHand = GetJsonBool(json, "show_second_hand", true);
            c.SmoothSweep = GetJsonBool(json, "smooth_sweep", true);
            c.ShowDate = GetJsonBool(json, "show_date", true);
            c.ShowDialBorder = GetJsonBool(json, "show_dial_border", true);
            c.AntiBurnIn = GetJsonBool(json, "anti_burn_in", true);
            c.ClockScale = GetJsonFloat(json, "clock_scale", 0.72f);
            c.DateFormatLanguage = GetJsonVal(json, "date_format_language", "system");

            c.BgColor = GetJsonVal(json, "bg_color", "#0B0F19");
            c.DialColor = GetJsonVal(json, "dial_color", "#111827");
            c.BorderColor = GetJsonVal(json, "border_color", "#1F2937");
            c.HourMarkersColor = GetJsonVal(json, "hour_markers_color", "#F3F4F6");
            c.MinuteMarkersColor = GetJsonVal(json, "minute_markers_color", "#4B5563");
            c.NumeralsColor = GetJsonVal(json, "numerals_color", "#F9FAFB");
            c.HourHandColor = GetJsonVal(json, "hour_hand_color", "#F9FAFB");
            c.MinuteHandColor = GetJsonVal(json, "minute_hand_color", "#E5E7EB");
            c.SecondHandColor = GetJsonVal(json, "second_hand_color", "#EF4444");
            c.AccentCenterColor = GetJsonVal(json, "accent_center_color", "#EF4444");
            c.DateTextColor = GetJsonVal(json, "date_text_color", "#9CA3AF");
            c.DateBadgeBgColor = GetJsonVal(json, "date_badge_bg_color", "#1F2937");
            return c;
        }

        private static string GetJsonVal(string json, string key, string def)
        {
            int idx = json.IndexOf("\"" + key + "\"");
            if (idx == -1) return def;
            int colon = json.IndexOf(":", idx);
            if (colon == -1) return def;
            int start = colon + 1;
            while (start < json.Length && (json[start] == ' ' || json[start] == '\"')) start++;
            int end = start;
            while (end < json.Length && json[end] != '\"' && json[end] != ',' && json[end] != '\r' && json[end] != '\n' && json[end] != '}') end++;
            if (end > start) return json.Substring(start, end - start).Trim();
            return def;
        }

        private static bool GetJsonBool(string json, string key, bool def)
        {
            string val = GetJsonVal(json, key, def.ToString().ToLower());
            return val.ToLower().Contains("true");
        }

        private static float GetJsonFloat(string json, string key, float def)
        {
            string val = GetJsonVal(json, key, def.ToString());
            float f;
            if (float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out f))
                return f;
            return def;
        }
    }
}
