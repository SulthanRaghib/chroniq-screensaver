using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AnalogClockScreensaver
{
    public class ClockConfig
    {
        // Mode: "analog" | "digital"
        public string ClockMode = "analog";

        // Analog Options
        public string PresetName = "Modern Dark";
        public string Style = "modern"; // modern, classic, bauhaus, sport, minimal
        public string NumeralType = "arabic"; // arabic, roman, dots, lines, none
        public bool ShowHourHand = true;
        public bool ShowMinuteHand = true;
        public bool ShowSecondHand = true;
        public bool SmoothSweep = true;
        public bool ShowDialBorder = true;

        // Digital Options
        public string DigitalStyle = "flip"; // "flip", "minimal"
        public bool Use24Hour = true;
        public bool ShowDigitalSeconds = true;

        // Common Display Options
        public bool ShowDate = true;
        public bool AntiBurnIn = true;
        public float ClockScale = 0.72f;
        public string DateFormatLanguage = "system"; // system, id, en, full_id, full_en, numeric

        // Colors
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
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = false; ShowDialBorder = false;
                BgColor = "#0D0D0D"; DialColor = "#181818"; BorderColor = "#282828";
                HourMarkersColor = "#E0E0E0"; MinuteMarkersColor = "#505050"; NumeralsColor = "#FFFFFF";
                HourHandColor = "#FFFFFF"; MinuteHandColor = "#D0D0D0"; SecondHandColor = "#E5A93C";
                AccentCenterColor = "#E5A93C"; DateTextColor = "#888888"; DateBadgeBgColor = "#222222";
            }
            else if (preset == "Classic Vintage Roman")
            {
                Style = "classic"; NumeralType = "roman";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = false; ShowDate = true; ShowDialBorder = true;
                BgColor = "#121110"; DialColor = "#F7F3E9"; BorderColor = "#C5A059";
                HourMarkersColor = "#2C2A29"; MinuteMarkersColor = "#736F6D"; NumeralsColor = "#1E1C1B";
                HourHandColor = "#1E1C1B"; MinuteHandColor = "#2C2A29"; SecondHandColor = "#8B1E1E";
                AccentCenterColor = "#C5A059"; DateTextColor = "#3D3937"; DateBadgeBgColor = "#E8E2D2";
            }
            else if (preset == "Swiss Railway (Bauhaus)")
            {
                Style = "bauhaus"; NumeralType = "none";
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = true; SmoothSweep = true; ShowDate = false; ShowDialBorder = true;
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
                ShowHourHand = true; ShowMinuteHand = true; ShowSecondHand = false; SmoothSweep = true; ShowDate = false; ShowDialBorder = false;
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

        private static ClockConfig ParseJson(string json)
        {
            ClockConfig c = new ClockConfig();
            c.ClockMode = GetJsonVal(json, "clock_mode", "analog");
            c.PresetName = GetJsonVal(json, "preset_name", c.PresetName);
            c.Style = GetJsonVal(json, "style", c.Style);
            c.NumeralType = GetJsonVal(json, "numeral_type", c.NumeralType);
            c.ShowHourHand = GetJsonVal(json, "show_hour_hand", "true").ToLower() == "true";
            c.ShowMinuteHand = GetJsonVal(json, "show_minute_hand", "true").ToLower() == "true";
            c.ShowSecondHand = GetJsonVal(json, "show_second_hand", "true").ToLower() == "true";
            c.SmoothSweep = GetJsonVal(json, "smooth_sweep", "true").ToLower() == "true";
            c.ShowDate = GetJsonVal(json, "show_date", "true").ToLower() == "true";
            c.ShowDialBorder = GetJsonVal(json, "show_dial_border", "true").ToLower() == "true";
            c.AntiBurnIn = GetJsonVal(json, "anti_burn_in", "true").ToLower() == "true";
            c.DateFormatLanguage = GetJsonVal(json, "date_format_lang", "system");

            // Digital parsing
            c.DigitalStyle = GetJsonVal(json, "digital_style", "flip");
            c.Use24Hour = GetJsonVal(json, "use_24_hour", "true").ToLower() == "true";
            c.ShowDigitalSeconds = GetJsonVal(json, "show_digital_seconds", "true").ToLower() == "true";

            float sc;
            if (float.TryParse(GetJsonVal(json, "clock_scale", "0.72"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out sc)) c.ClockScale = sc;

            c.BgColor = GetJsonVal(json, "background", c.BgColor);
            c.DialColor = GetJsonVal(json, "dial_face", c.DialColor);
            c.BorderColor = GetJsonVal(json, "dial_border", c.BorderColor);
            c.HourMarkersColor = GetJsonVal(json, "hour_markers", c.HourMarkersColor);
            c.MinuteMarkersColor = GetJsonVal(json, "minute_markers", c.MinuteMarkersColor);
            c.NumeralsColor = GetJsonVal(json, "numerals", c.NumeralsColor);
            c.HourHandColor = GetJsonVal(json, "hour_hand", c.HourHandColor);
            c.MinuteHandColor = GetJsonVal(json, "minute_hand", c.MinuteHandColor);
            c.SecondHandColor = GetJsonVal(json, "second_hand", c.SecondHandColor);
            c.AccentCenterColor = GetJsonVal(json, "accent_center", c.AccentCenterColor);
            c.DateTextColor = GetJsonVal(json, "date_text", c.DateTextColor);
            c.DateBadgeBgColor = GetJsonVal(json, "date_badge_bg", c.DateBadgeBgColor);
            return c;
        }

        public string ToJson()
        {
            return "{\n" +
                "  \"clock_mode\": \"" + ClockMode + "\",\n" +
                "  \"preset_name\": \"" + PresetName + "\",\n" +
                "  \"style\": \"" + Style + "\",\n" +
                "  \"numeral_type\": \"" + NumeralType + "\",\n" +
                "  \"show_hour_hand\": " + (ShowHourHand ? "true" : "false") + ",\n" +
                "  \"show_minute_hand\": " + (ShowMinuteHand ? "true" : "false") + ",\n" +
                "  \"show_second_hand\": " + (ShowSecondHand ? "true" : "false") + ",\n" +
                "  \"smooth_sweep\": " + (SmoothSweep ? "true" : "false") + ",\n" +
                "  \"show_date\": " + (ShowDate ? "true" : "false") + ",\n" +
                "  \"show_dial_border\": " + (ShowDialBorder ? "true" : "false") + ",\n" +
                "  \"anti_burn_in\": " + (AntiBurnIn ? "true" : "false") + ",\n" +
                "  \"date_format_lang\": \"" + DateFormatLanguage + "\",\n" +
                "  \"digital_style\": \"" + DigitalStyle + "\",\n" +
                "  \"use_24_hour\": " + (Use24Hour ? "true" : "false") + ",\n" +
                "  \"show_digital_seconds\": " + (ShowDigitalSeconds ? "true" : "false") + ",\n" +
                "  \"clock_scale\": " + ClockScale.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",\n" +
                "  \"colors\": {\n" +
                "    \"background\": \"" + BgColor + "\",\n" +
                "    \"dial_face\": \"" + DialColor + "\",\n" +
                "    \"dial_border\": \"" + BorderColor + "\",\n" +
                "    \"hour_markers\": \"" + HourMarkersColor + "\",\n" +
                "    \"minute_markers\": \"" + MinuteMarkersColor + "\",\n" +
                "    \"numerals\": \"" + NumeralsColor + "\",\n" +
                "    \"hour_hand\": \"" + HourHandColor + "\",\n" +
                "    \"minute_hand\": \"" + MinuteHandColor + "\",\n" +
                "    \"second_hand\": \"" + SecondHandColor + "\",\n" +
                "    \"accent_center\": \"" + AccentCenterColor + "\",\n" +
                "    \"date_text\": \"" + DateTextColor + "\",\n" +
                "    \"date_badge_bg\": \"" + DateBadgeBgColor + "\"\n" +
                "  }\n" +
                "}";
        }
    }

    public class SettingsForm : Form
    {
        private ClockConfig config;
        private bool isUpdatingUI = false;

        // Mode
        private RadioButton rbAnalog;
        private RadioButton rbDigital;

        // Analog options
        private GroupBox gbStyle;
        private GroupBox gbHands;
        private ComboBox cbPreset;
        private ComboBox cbStyle;
        private ComboBox cbNumeral;
        private CheckBox chkHour;
        private CheckBox chkMin;
        private CheckBox chkSec;
        private CheckBox chkSweep;
        private CheckBox chkBorder;

        // Digital options
        private GroupBox gbDigital;
        private ComboBox cbDigitalStyle;
        private CheckBox chk24Hour;
        private CheckBox chkDigitalSec;

        // Common options
        private CheckBox chkDate;
        private CheckBox chkAntiBurn;
        private ComboBox cbDateLang;
        private TrackBar tbScale;
        private Label lblScaleVal;

        // Color buttons
        private Button btnBg, btnDial, btnBorder, btnHourM, btnMinM, btnNum, btnHourH, btnMinH, btnSecH, btnAccent, btnDateText, btnDateBg;

        public SettingsForm()
        {
            config = ClockConfig.Load();
            InitUI();
            LoadConfigToUI();
        }

        private void InitUI()
        {
            this.Text = "Chroniq — Pengaturan Screensaver Jam (Analog & Digital)";
            this.Size = new Size(660, 840);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(243, 244, 246);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Tab Control
            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Padding = new Point(14, 6);

            TabPage tabGeneral = new TabPage("  Mode & Gaya Jam  ");
            tabGeneral.BackColor = Color.White;
            tabGeneral.AutoScroll = true;

            TabPage tabColors = new TabPage("  Palet Warna Kustom  ");
            tabColors.BackColor = Color.White;
            tabColors.AutoScroll = true;

            tabs.TabPages.Add(tabGeneral);
            tabs.TabPages.Add(tabColors);

            // Bottom Panel for Buttons
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 60;
            bottomPanel.BackColor = Color.FromArgb(226, 232, 240);

            Button btnPreview = new Button();
            btnPreview.Text = "👁️ Test Preview";
            btnPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPreview.BackColor = Color.FromArgb(59, 130, 246);
            btnPreview.ForeColor = Color.White;
            btnPreview.FlatStyle = FlatStyle.Flat;
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Size = new Size(135, 38);
            btnPreview.Location = new Point(16, 11);
            btnPreview.Cursor = Cursors.Hand;
            btnPreview.Click += (s, e) => {
                ClockConfig previewConfig = BuildCurrentUIConfig();
                using (ScreenSaverForm previewForm = new ScreenSaverForm(Screen.PrimaryScreen.Bounds, previewConfig, testPreview: true))
                {
                    previewForm.ShowDialog();
                }
                Cursor.Show();
            };

            Button btnSave = new Button();
            btnSave.Text = "Simpan & Terapkan";
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(16, 185, 129);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Size = new Size(155, 38);
            btnSave.Location = new Point(465, 11);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += (s, e) => SaveAndClose();

            Button btnCancel = new Button();
            btnCancel.Text = "Batal";
            btnCancel.BackColor = Color.FromArgb(148, 163, 184);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Size = new Size(85, 38);
            btnCancel.Location = new Point(370, 11);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(btnPreview);
            bottomPanel.Controls.Add(btnSave);
            bottomPanel.Controls.Add(btnCancel);

            this.Controls.Add(tabs);
            this.Controls.Add(bottomPanel);

            // Build General Tab
            BuildGeneralTab(tabGeneral);
            BuildColorsTab(tabColors);
        }

        private void BuildGeneralTab(TabPage page)
        {
            // 0. Mode Selector Group
            GroupBox gbMode = new GroupBox();
            gbMode.Text = " ⏱️ Pilih Mode Tampilan Screensaver ";
            gbMode.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            gbMode.ForeColor = Color.FromArgb(30, 41, 59);
            gbMode.Location = new Point(14, 14);
            gbMode.Size = new Size(595, 65);

            rbAnalog = new RadioButton { Text = "Jam Analog 🕰️", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Location = new Point(30, 25), AutoSize = true, Checked = true };
            rbDigital = new RadioButton { Text = "Jam Digital (Flip / Modern) 🔢", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Location = new Point(230, 25), AutoSize = true };

            rbAnalog.CheckedChanged += (s, e) => {
                if (isUpdatingUI) return;
                if (rbAnalog.Checked)
                {
                    config.ClockMode = "analog";
                    UpdateModeUI();
                }
            };
            rbDigital.CheckedChanged += (s, e) => {
                if (isUpdatingUI) return;
                if (rbDigital.Checked)
                {
                    config.ClockMode = "digital";
                    UpdateModeUI();
                }
            };

            gbMode.Controls.Add(rbAnalog);
            gbMode.Controls.Add(rbDigital);
            page.Controls.Add(gbMode);

            // 1. Preset Group (For Analog & Digital theme palettes)
            GroupBox gbPreset = new GroupBox();
            gbPreset.Text = " 🎨 Tema & Preset Warna ";
            gbPreset.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbPreset.Location = new Point(14, 88);
            gbPreset.Size = new Size(595, 65);

            Label lblP = new Label { Text = "Pilih Preset:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 26), AutoSize = true };
            cbPreset = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(120, 23), Size = new Size(280, 25) };
            cbPreset.Items.AddRange(new object[] {
                "Modern Dark", "Fliqlo Monochrome", "Classic Vintage Roman",
                "Swiss Railway (Bauhaus)", "Midnight Sapphire", "Cyberpunk Neon",
                "Minimal Slate", "Emerald Luxury", "Custom"
            });
            cbPreset.SelectedIndexChanged += (s, e) => {
                if (isUpdatingUI) return;
                if (cbPreset.SelectedItem != null && cbPreset.SelectedItem.ToString() != "Custom")
                {
                    string currentMode = rbDigital.Checked ? "digital" : "analog";
                    config.ApplyPreset(cbPreset.SelectedItem.ToString());
                    config.ClockMode = currentMode; // Preserve user's current clock mode!
                    LoadConfigToUI();
                }
            };

            gbPreset.Controls.Add(lblP);
            gbPreset.Controls.Add(cbPreset);
            page.Controls.Add(gbPreset);

            // 2. Digital Options Group
            gbDigital = new GroupBox();
            gbDigital.Text = " 🔢 Opsi Jam Digital ";
            gbDigital.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbDigital.Location = new Point(14, 162);
            gbDigital.Size = new Size(595, 125);

            Label lblDigSt = new Label { Text = "Gaya Tampilan Digital:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 26), AutoSize = true };
            cbDigitalStyle = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(190, 23), Size = new Size(280, 25) };
            cbDigitalStyle.Items.AddRange(new object[] { "flip (Kartu Flip ala Fliqlo)", "minimal (Teks Bersih Tanpa Kartu)" });

            chk24Hour = new CheckBox { Text = "Gunakan Format 24 Jam (Contoh: 23:50 vs 11:50 PM)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 60), AutoSize = true };
            chkDigitalSec = new CheckBox { Text = "Tampilkan Detik Digital", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 88), AutoSize = true };

            gbDigital.Controls.Add(lblDigSt);
            gbDigital.Controls.Add(cbDigitalStyle);
            gbDigital.Controls.Add(chk24Hour);
            gbDigital.Controls.Add(chkDigitalSec);
            page.Controls.Add(gbDigital);

            // 3. Style & Numerals Group (Analog)
            gbStyle = new GroupBox();
            gbStyle.Text = " 🕰️ Desain Jam & Penanda (Khusus Analog) ";
            gbStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbStyle.Location = new Point(14, 296);
            gbStyle.Size = new Size(595, 98);

            Label lblSt = new Label { Text = "Gaya Desain:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 26), AutoSize = true };
            cbStyle = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(180, 23), Size = new Size(240, 25) };
            cbStyle.Items.AddRange(new object[] { "modern", "classic", "bauhaus", "sport", "minimal" });

            Label lblNum = new Label { Text = "Tipe Angka:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 60), AutoSize = true };
            cbNumeral = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(180, 57), Size = new Size(240, 25) };
            cbNumeral.Items.AddRange(new object[] { "arabic", "roman", "dots", "lines", "none" });

            gbStyle.Controls.Add(lblSt); gbStyle.Controls.Add(cbStyle);
            gbStyle.Controls.Add(lblNum); gbStyle.Controls.Add(cbNumeral);
            page.Controls.Add(gbStyle);

            // 4. Hands & Motion Group (Analog)
            gbHands = new GroupBox();
            gbHands.Text = " ⏱️ Opsi Jarum Jam & Animasi (Khusus Analog) ";
            gbHands.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbHands.Location = new Point(14, 402);
            gbHands.Size = new Size(595, 138);

            chkHour = new CheckBox { Text = "Tampilkan Jarum Jam (Hour Hand)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 24), AutoSize = true };
            chkMin = new CheckBox { Text = "Tampilkan Jarum Menit (Minute Hand)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 48), AutoSize = true };
            chkSec = new CheckBox { Text = "Tampilkan Jarum Detik (Second Hand)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 72), AutoSize = true };
            chkSweep = new CheckBox { Text = "Gerakan Mulus 60 FPS (Smooth Sweep Motion)", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), Location = new Point(16, 100), AutoSize = true };

            gbHands.Controls.Add(chkHour); gbHands.Controls.Add(chkMin); gbHands.Controls.Add(chkSec); gbHands.Controls.Add(chkSweep);
            page.Controls.Add(gbHands);

            // 5. Display, Language & Scale Group (Common)
            GroupBox gbDisp = new GroupBox();
            gbDisp.Text = " 📐 Fitur Layar & Bahasa Tanggal ";
            gbDisp.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbDisp.Location = new Point(14, 548);
            gbDisp.Size = new Size(595, 215);

            chkDate = new CheckBox { Text = "Tampilkan Tanggal & Hari", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 24), AutoSize = true };
            chkBorder = new CheckBox { Text = "Tampilkan Garis Tepi (Dial Border / Card Border)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 50), AutoSize = true };
            chkAntiBurn = new CheckBox { Text = "Anti-Burn-In Protection (Pergeseran Mikro Layar OLED)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 76), AutoSize = true };

            Label lblDateLang = new Label { Text = "Bahasa & Format Tanggal:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 114), AutoSize = true };
            cbDateLang = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(200, 110), Size = new Size(280, 25) };
            cbDateLang.Items.AddRange(new object[] {
                "Default Sistem (Otomatis)",
                "Bahasa Indonesia (RAB 19 AGU)",
                "English (WED 19 AUG)",
                "Indonesia Lengkap (Rabu, 19 Agustus)",
                "English Full (Wednesday, 19 August)",
                "Format Angka (19/08/2026)"
            });

            Label lblSc = new Label { Text = "Ukuran Jam (% Layar):", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 158), AutoSize = true };
            tbScale = new TrackBar { Minimum = 40, Maximum = 90, Value = 72, TickFrequency = 5, Location = new Point(200, 152), Size = new Size(240, 45) };
            lblScaleVal = new Label { Text = "72%", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(450, 158), AutoSize = true };
            tbScale.ValueChanged += (s, e) => { lblScaleVal.Text = tbScale.Value + "%"; };

            gbDisp.Controls.Add(chkDate); gbDisp.Controls.Add(chkBorder); gbDisp.Controls.Add(chkAntiBurn);
            gbDisp.Controls.Add(lblDateLang); gbDisp.Controls.Add(cbDateLang);
            gbDisp.Controls.Add(lblSc); gbDisp.Controls.Add(tbScale); gbDisp.Controls.Add(lblScaleVal);
            page.Controls.Add(gbDisp);
        }

        private void UpdateModeUI()
        {
            bool isDigital = rbDigital.Checked;
            gbDigital.Enabled = isDigital;
            gbDigital.Visible = isDigital;

            // When digital mode is active, analog hands/dial groups are subtle or disabled
            gbStyle.Enabled = !isDigital;
            gbHands.Enabled = !isDigital;
        }

        private void BuildColorsTab(TabPage page)
        {
            int y = 14;
            btnBg = CreateColorRow(page, "Latar Belakang Layar (Background):", ref y);
            btnDial = CreateColorRow(page, "Permukaan Piringan / Kartu Digital (Dial / Card Face):", ref y);
            btnBorder = CreateColorRow(page, "Garis Batas Piringan / Kartu (Border):", ref y);
            btnHourM = CreateColorRow(page, "Garis Penanda Jam (Hour Markers):", ref y);
            btnMinM = CreateColorRow(page, "Garis Penanda Menit (Minute Markers):", ref y);
            btnNum = CreateColorRow(page, "Teks Angka Jam (Numerals / Digital Digits):", ref y);
            btnHourH = CreateColorRow(page, "Jarum Jam (Hour Hand):", ref y);
            btnMinH = CreateColorRow(page, "Jarum Menit (Minute Hand):", ref y);
            btnSecH = CreateColorRow(page, "Jarum Detik / Aksen Digital (Seconds / Accent):", ref y);
            btnAccent = CreateColorRow(page, "Titik Poros Tengah / Divider Lipatan (Accent):", ref y);
            btnDateBg = CreateColorRow(page, "Kotak Latar Tanggal (Date Box BG):", ref y);
            btnDateText = CreateColorRow(page, "Teks Tanggal (Date Text):", ref y);
        }

        private Button CreateColorRow(TabPage page, string label, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(16, y + 4);
            lbl.Size = new Size(300, 20);
            page.Controls.Add(lbl);

            Button btn = new Button();
            btn.Size = new Size(130, 26);
            btn.Location = new Point(330, y);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Consolas", 8.5F, FontStyle.Bold);

            btn.Click += (s, e) => {
                using (ColorDialog cd = new ColorDialog())
                {
                    cd.Color = btn.BackColor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        SetColorBtn(btn, ColorToHex(cd.Color));
                        cbPreset.SelectedItem = "Custom";
                    }
                }
            };

            page.Controls.Add(btn);
            y += 34;
            return btn;
        }

        private string ColorToHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        private void SetColorBtn(Button btn, string hex)
        {
            Color c = Color.Black;
            try
            {
                if (hex.StartsWith("#")) hex = hex.Substring(1);
                c = Color.FromArgb(Convert.ToInt32(hex.Substring(0, 2), 16), Convert.ToInt32(hex.Substring(2, 2), 16), Convert.ToInt32(hex.Substring(4, 2), 16));
            }
            catch { }
            btn.BackColor = c;
            btn.Text = "#" + hex.ToUpper();
            btn.ForeColor = (c.R * 0.299 + c.G * 0.587 + c.B * 0.114 > 128) ? Color.Black : Color.White;
        }

        private void LoadConfigToUI()
        {
            isUpdatingUI = true;
            try
            {
                if (config.ClockMode == "digital") rbDigital.Checked = true;
                else rbAnalog.Checked = true;

                if (cbPreset.Items.Contains(config.PresetName)) cbPreset.SelectedItem = config.PresetName;
                else cbPreset.SelectedItem = "Custom";

                if (cbStyle.Items.Contains(config.Style)) cbStyle.SelectedItem = config.Style;
                if (cbNumeral.Items.Contains(config.NumeralType)) cbNumeral.SelectedItem = config.NumeralType;

                chkHour.Checked = config.ShowHourHand;
                chkMin.Checked = config.ShowMinuteHand;
                chkSec.Checked = config.ShowSecondHand;
                chkSweep.Checked = config.SmoothSweep;
                chkDate.Checked = config.ShowDate;
                chkBorder.Checked = config.ShowDialBorder;
                chkAntiBurn.Checked = config.AntiBurnIn;

                // Digital
                if (config.DigitalStyle == "minimal") cbDigitalStyle.SelectedIndex = 1;
                else cbDigitalStyle.SelectedIndex = 0; // flip
                chk24Hour.Checked = config.Use24Hour;
                chkDigitalSec.Checked = config.ShowDigitalSeconds;

                // Date Lang Mapping
                if (config.DateFormatLanguage == "id") cbDateLang.SelectedIndex = 1;
                else if (config.DateFormatLanguage == "en") cbDateLang.SelectedIndex = 2;
                else if (config.DateFormatLanguage == "full_id") cbDateLang.SelectedIndex = 3;
                else if (config.DateFormatLanguage == "full_en") cbDateLang.SelectedIndex = 4;
                else if (config.DateFormatLanguage == "numeric") cbDateLang.SelectedIndex = 5;
                else cbDateLang.SelectedIndex = 0; // system

                int sc = (int)(config.ClockScale * 100);
                if (sc >= 40 && sc <= 90) tbScale.Value = sc;
                lblScaleVal.Text = tbScale.Value + "%";

                SetColorBtn(btnBg, config.BgColor);
                SetColorBtn(btnDial, config.DialColor);
                SetColorBtn(btnBorder, config.BorderColor);
                SetColorBtn(btnHourM, config.HourMarkersColor);
                SetColorBtn(btnMinM, config.MinuteMarkersColor);
                SetColorBtn(btnNum, config.NumeralsColor);
                SetColorBtn(btnHourH, config.HourHandColor);
                SetColorBtn(btnMinH, config.MinuteHandColor);
                SetColorBtn(btnSecH, config.SecondHandColor);
                SetColorBtn(btnAccent, config.AccentCenterColor);
                SetColorBtn(btnDateBg, config.DateBadgeBgColor);
                SetColorBtn(btnDateText, config.DateTextColor);

                UpdateModeUI();
            }
            finally
            {
                isUpdatingUI = false;
            }
        }

        private ClockConfig BuildCurrentUIConfig()
        {
            ClockConfig c = new ClockConfig();
            c.ClockMode = rbDigital.Checked ? "digital" : "analog";
            c.PresetName = cbPreset.SelectedItem != null ? cbPreset.SelectedItem.ToString() : "Custom";
            c.Style = cbStyle.SelectedItem != null ? cbStyle.SelectedItem.ToString() : "modern";
            c.NumeralType = cbNumeral.SelectedItem != null ? cbNumeral.SelectedItem.ToString() : "arabic";

            c.ShowHourHand = chkHour.Checked;
            c.ShowMinuteHand = chkMin.Checked;
            c.ShowSecondHand = chkSec.Checked;
            c.SmoothSweep = chkSweep.Checked;
            c.ShowDate = chkDate.Checked;
            c.ShowDialBorder = chkBorder.Checked;
            c.AntiBurnIn = chkAntiBurn.Checked;
            c.ClockScale = tbScale.Value / 100.0f;

            // Digital sync
            c.DigitalStyle = (cbDigitalStyle.SelectedIndex == 1) ? "minimal" : "flip";
            c.Use24Hour = chk24Hour.Checked;
            c.ShowDigitalSeconds = chkDigitalSec.Checked;

            // Date Lang Sync
            int dIdx = cbDateLang.SelectedIndex;
            if (dIdx == 1) c.DateFormatLanguage = "id";
            else if (dIdx == 2) c.DateFormatLanguage = "en";
            else if (dIdx == 3) c.DateFormatLanguage = "full_id";
            else if (dIdx == 4) c.DateFormatLanguage = "full_en";
            else if (dIdx == 5) c.DateFormatLanguage = "numeric";
            else c.DateFormatLanguage = "system";

            c.BgColor = btnBg.Text;
            c.DialColor = btnDial.Text;
            c.BorderColor = btnBorder.Text;
            c.HourMarkersColor = btnHourM.Text;
            c.MinuteMarkersColor = btnMinM.Text;
            c.NumeralsColor = btnNum.Text;
            c.HourHandColor = btnHourH.Text;
            c.MinuteHandColor = btnMinH.Text;
            c.SecondHandColor = btnSecH.Text;
            c.AccentCenterColor = btnAccent.Text;
            c.DateBadgeBgColor = btnDateBg.Text;
            c.DateTextColor = btnDateText.Text;
            return c;
        }

        private void SyncUIToConfig()
        {
            this.config = BuildCurrentUIConfig();
        }

        private void SaveAndClose()
        {
            SyncUIToConfig();
            config.Save();
            MessageBox.Show("Pengaturan jam screensaver berhasil disimpan!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }

    public class ScreenSaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private Point mouseLocation;
        private bool previewMode = false;
        private bool isTestPreview = false;
        private IntPtr previewParentHwnd = IntPtr.Zero;
        private ClockConfig config;
        private Timer timer;
        private DateTime startTime;

        private static Color ParseColor(string hex, Color fallback)
        {
            try
            {
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

        public ScreenSaverForm(Rectangle bounds, ClockConfig customConfig = null, bool testPreview = false)
        {
            this.isTestPreview = testPreview;
            config = customConfig != null ? customConfig : ClockConfig.Load();
            startTime = DateTime.Now;

            this.BackColor = ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
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

        private DateTime lastConfigCheck = DateTime.MinValue;
        private DateTime lastConfigWriteTime = DateTime.MinValue;

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

            this.BackColor = ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;

            SetParent(this.Handle, previewHandle);
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000 | 0x10000000)); // WS_CHILD | WS_VISIBLE

            Rectangle parentRect;
            GetClientRect(previewHandle, out parentRect);
            this.Bounds = new Rectangle(0, 0, parentRect.Width, parentRect.Height);
            SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, parentRect.Width, parentRect.Height, 0x0040); // SWP_SHOWWINDOW

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
                                    this.BackColor = ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
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
            // Do not paint background to prevent flicker
        }

        private string GetFormattedDate(DateTime now)
        {
            string lang = config.DateFormatLanguage;

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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color bgColor = ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
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
                RenderDigitalClock(g, w, h, driftX, driftY, now);
            }
            else
            {
                RenderAnalogClock(g, w, h, driftX, driftY, now);
            }
        }

        private void RenderDigitalClock(Graphics g, int w, int h, int driftX, int driftY, DateTime now)
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

            Color cardBgColor = ParseColor(config.DialColor, Color.FromArgb(24, 24, 24));
            Color cardBorderColor = ParseColor(config.BorderColor, Color.FromArgb(40, 40, 40));
            Color digitColor = ParseColor(config.NumeralsColor, Color.White);
            Color secColor = ParseColor(config.SecondHandColor, Color.FromArgb(229, 169, 60));
            Color accentColor = ParseColor(config.AccentCenterColor, Color.FromArgb(45, 45, 45));

            float baseSize = Math.Min(w, h) * scale;
            float cardH = baseSize * 0.46f;
            float cardW = cardH * 0.90f;
            float secCardW = cardW * 0.65f;
            float secCardH = cardH * 0.65f;
            float gap = baseSize * 0.035f;

            float totalW = showSec ? (cardW * 2 + secCardW + gap * 2) : (cardW * 2 + gap);
            float startX = cx - (totalW / 2f);
            float startY = cy - (cardH / 2f) - (config.ShowDate ? baseSize * 0.05f : 0);

            // 1. Draw Hour Card
            RectangleF hrRect = new RectangleF(startX, startY, cardW, cardH);
            DrawDigitalCard(g, hrRect, hrStr, digitColor, cardBgColor, cardBorderColor, isFlip, config.Use24Hour ? null : ampmStr);

            // 2. Draw Minute Card
            RectangleF minRect = new RectangleF(startX + cardW + gap, startY, cardW, cardH);
            DrawDigitalCard(g, minRect, minStr, digitColor, cardBgColor, cardBorderColor, isFlip, null);

            // 3. Draw Seconds Card (if enabled)
            if (showSec)
            {
                RectangleF secRect = new RectangleF(startX + cardW * 2 + gap * 2, startY + (cardH - secCardH), secCardW, secCardH);
                DrawDigitalCard(g, secRect, secStr, secColor, cardBgColor, cardBorderColor, isFlip, null);
            }

            // 4. Date Badge (if enabled)
            if (config.ShowDate)
            {
                string dateStr = GetFormattedDate(now);
                float dateFontSize = Math.Max(4.5f, baseSize * (previewMode ? 0.046f : 0.052f));

                using (Font dateFont = new Font("Segoe UI", dateFontSize, FontStyle.Bold))
                using (Brush dateTextBrush = new SolidBrush(ParseColor(config.DateTextColor, Color.Gray)))
                using (Brush dateBgBrush = new SolidBrush(ParseColor(config.DateBadgeBgColor, Color.FromArgb(31, 41, 55))))
                using (Pen dateBorderPen = new Pen(cardBorderColor, 1f))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    SizeF sz = g.MeasureString(dateStr, dateFont);
                    float bw = sz.Width + baseSize * (previewMode ? 0.05f : 0.08f);
                    float bh = sz.Height + baseSize * 0.02f;
                    float bx = cx - bw / 2f;
                    float by = startY + cardH + baseSize * (previewMode ? 0.035f : 0.06f);

                    GraphicsPath path = RoundedRect(new RectangleF(bx, by, bw, bh), Math.Max(2f, baseSize * 0.015f));
                    g.FillPath(dateBgBrush, path);
                    g.DrawPath(dateBorderPen, path);
                    g.DrawString(dateStr, dateFont, dateTextBrush, cx, by + bh / 2f, sf);
                }
            }
        }

        private void DrawDigitalCard(Graphics g, RectangleF rect, string text, Color textColor, Color bgColor, Color borderColor, bool isFlip, string badgeText)
        {
            float cornerR = Math.Max(4f, rect.Height * 0.08f);

            if (isFlip)
            {
                // Rounded Card Background
                using (GraphicsPath path = RoundedRect(rect, cornerR))
                using (Brush bgBrush = new SolidBrush(bgColor))
                using (Pen borderPen = new Pen(borderColor, Math.Max(1.5f, rect.Height * 0.012f)))
                {
                    g.FillPath(bgBrush, path);
                    if (config.ShowDialBorder)
                    {
                        g.DrawPath(borderPen, path);
                    }
                }

                // Center Split Line (Fliqlo horizontal crease)
                float midY = rect.Y + rect.Height / 2f;
                using (Pen splitPen = new Pen(Color.FromArgb(180, 10, 10, 15), Math.Max(1.5f, rect.Height * 0.012f)))
                using (Pen highlightPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1f))
                {
                    g.DrawLine(splitPen, rect.X, midY, rect.Right, midY);
                    g.DrawLine(highlightPen, rect.X, midY + 1.5f, rect.Right, midY + 1.5f);
                }

                // Tiny side hinge notches
                float notchW = rect.Width * 0.035f;
                float notchH = rect.Height * 0.04f;
                using (Brush notchBrush = new SolidBrush(ParseColor(config.BgColor, Color.Black)))
                {
                    g.FillRectangle(notchBrush, rect.X - 1, midY - notchH / 2f, notchW, notchH);
                    g.FillRectangle(notchBrush, rect.Right - notchW + 1, midY - notchH / 2f, notchW, notchH);
                }
            }

            // Draw Digits
            float fontSize = Math.Max(14f, rect.Height * 0.62f);
            using (Font font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Brush textBrush = new SolidBrush(textColor))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, textBrush, rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f, sf);
            }

            // Draw AM/PM Badge if applicable
            if (!string.IsNullOrEmpty(badgeText))
            {
                float badgeFontSize = Math.Max(7f, rect.Height * 0.12f);
                using (Font badgeFont = new Font("Segoe UI", badgeFontSize, FontStyle.Bold))
                using (Brush badgeBrush = new SolidBrush(textColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near })
                {
                    g.DrawString(badgeText, badgeFont, badgeBrush, rect.X + rect.Width * 0.08f, rect.Y + rect.Height * 0.07f, sf);
                }
            }
        }

        private void RenderAnalogClock(Graphics g, int w, int h, int driftX, int driftY, DateTime now)
        {
            float scale = previewMode ? 0.72f : config.ClockScale;
            float cx = (w / 2f) + driftX;
            float cy = (h / 2f) + driftY;
            float radius = (Math.Min(w, h) / 2f) * scale;
            if (radius < 10) return;

            // 1. Dial Face
            Color dialColor = ParseColor(config.DialColor, Color.FromArgb(17, 24, 39));
            Color borderColor = ParseColor(config.BorderColor, Color.FromArgb(31, 41, 55));

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
            Color hourMarkerColor = ParseColor(config.HourMarkersColor, Color.White);
            Color minMarkerColor = ParseColor(config.MinuteMarkersColor, Color.Gray);
            Color numColor = ParseColor(config.NumeralsColor, Color.White);

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
                string[] romans = { "XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
                string[] arabics = { "12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
                string[] list = config.NumeralType == "roman" ? romans : arabics;

                float fontSize = Math.Max(6f, radius * (config.NumeralType == "roman" ? 0.12f : 0.13f));
                string fontName = config.NumeralType == "roman" || config.Style == "classic" ? "Georgia" : "Segoe UI";

                using (Font font = new Font(fontName, fontSize, (config.NumeralType == "roman" ? FontStyle.Regular : FontStyle.Bold)))
                using (Brush numBrush = new SolidBrush(numColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    float numDist = radius * 0.73f;
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

            // 4. Date Badge
            if (config.ShowDate)
            {
                string dateStr = GetFormattedDate(now);
                float dateFontSize = Math.Max(4.5f, radius * (previewMode ? 0.052f : 0.058f));

                using (Font dateFont = new Font("Segoe UI", dateFontSize, FontStyle.Bold))
                using (Brush dateTextBrush = new SolidBrush(ParseColor(config.DateTextColor, Color.Gray)))
                using (Brush dateBgBrush = new SolidBrush(ParseColor(config.DateBadgeBgColor, Color.FromArgb(31, 41, 55))))
                using (Pen dateBorderPen = new Pen(borderColor, 1f))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    SizeF sz = g.MeasureString(dateStr, dateFont);
                    float bw = sz.Width + radius * (previewMode ? 0.06f : 0.08f);
                    float bh = sz.Height + radius * 0.02f;
                    float bx = cx - bw / 2f;
                    float by = cy + radius * 0.38f - bh / 2f;

                    GraphicsPath path = RoundedRect(new RectangleF(bx, by, bw, bh), Math.Max(2f, radius * 0.02f));
                    g.FillPath(dateBgBrush, path);
                    g.DrawPath(dateBorderPen, path);
                    g.DrawString(dateStr, dateFont, dateTextBrush, cx, cy + radius * 0.38f, sf);
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

            Color hrHandColor = ParseColor(config.HourHandColor, Color.White);
            Color minHandColor = ParseColor(config.MinuteHandColor, Color.LightGray);
            Color secHandColor = ParseColor(config.SecondHandColor, Color.Red);
            Color accentColor = ParseColor(config.AccentCenterColor, Color.Red);

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

            // 7. Center Cap
            float capR = Math.Max(2.5f, radius * 0.032f);
            using (Brush capBrush = new SolidBrush(accentColor))
            using (Brush innerDotBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.FillEllipse(capBrush, cx - capR, cy - capR, capR * 2, capR * 2);
                g.FillEllipse(innerDotBrush, cx - capR / 2f, cy - capR / 2f, capR, capR);
            }
        }

        private void DrawHand(Graphics g, float cx, float cy, double angleDeg, float length, float counterLen, float width, Color color, string style)
        {
            double rad = angleDeg * Math.PI / 180.0;
            float cosA = (float)Math.Cos(rad);
            float sinA = (float)Math.Sin(rad);

            float nx = -sinA * (width / 2f);
            float ny = cosA * (width / 2f);

            float tipX = cx + length * cosA;
            float tipY = cy + length * sinA;
            float tailX = cx - counterLen * cosA;
            float tailY = cy - counterLen * sinA;

            using (Brush brush = new SolidBrush(color))
            {
                if (style == "bauhaus" || style == "minimal")
                {
                    PointF[] pts = new PointF[]
                    {
                        new PointF(tailX + nx, tailY + ny),
                        new PointF(tipX + nx, tipY + ny),
                        new PointF(tipX - nx, tipY - ny),
                        new PointF(tailX - nx, tailY - ny)
                    };
                    g.FillPolygon(brush, pts);
                }
                else
                {
                    PointF[] pts = new PointF[]
                    {
                        new PointF(tailX + nx * 0.7f, tailY + ny * 0.7f),
                        new PointF(tipX + nx * 0.3f, tipY + ny * 0.3f),
                        new PointF(tipX, tipY),
                        new PointF(tipX - nx * 0.3f, tipY - ny * 0.3f),
                        new PointF(tailX - nx * 0.7f, tailY - ny * 0.7f)
                    };
                    g.FillPolygon(brush, pts);
                }
            }
        }

        private void DrawSecondHand(Graphics g, float cx, float cy, double angleDeg, float length, float counterLen, float radius, Color color, string style)
        {
            double rad = angleDeg * Math.PI / 180.0;
            float cosA = (float)Math.Cos(rad);
            float sinA = (float)Math.Sin(rad);

            float tipX = cx + length * cosA;
            float tipY = cy + length * sinA;
            float tailX = cx - counterLen * cosA;
            float tailY = cy - counterLen * sinA;

            float lineW = Math.Max(1.2f, radius * 0.009f);
            using (Pen p = new Pen(color, lineW))
            {
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;
                g.DrawLine(p, tailX, tailY, tipX, tipY);
            }

            using (Brush b = new SolidBrush(color))
            {
                if (style == "bauhaus")
                {
                    float discDist = length * 0.78f;
                    float discX = cx + discDist * cosA;
                    float discY = cy + discDist * sinA;
                    float discR = Math.Max(3f, radius * 0.05f);
                    g.FillEllipse(b, discX - discR, discY - discR, discR * 2, discR * 2);
                }
                else
                {
                    float cwX = cx - (counterLen * 0.65f) * cosA;
                    float cwY = cy - (counterLen * 0.65f) * sinA;
                    float cwR = Math.Max(1.5f, radius * 0.022f);
                    g.FillEllipse(b, cwX - cwR, cwY - cwR, cwR * 2, cwR * 2);
                }
            }
        }

        private GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void ExitScreensaver()
        {
            if (previewMode) return;
            try
            {
                Cursor.Show();
                if (timer != null) timer.Stop();
            }
            catch { }

            if (isTestPreview)
            {
                this.Close(); // Only close modal preview window and return to SettingsForm!
            }
            else
            {
                Environment.Exit(0); // Exit standalone screensaver process
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

    public static class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDPIAware();
                }
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArg = args[0].ToLower().Trim();
                string secondArg = args.Length > 1 ? args[1] : null;

                if (firstArg.StartsWith("/c"))
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
                            ShowWindow(form.Handle, 5); // SW_SHOW
                            Application.Run(form);
                            return;
                        }
                        catch { }
                    }
                }
                else if (firstArg.StartsWith("/s"))
                {
                    // Fullscreen on all screens
                    RunFullScreen();
                    return;
                }
            }

            // Default: Fullscreen
            RunFullScreen();
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
