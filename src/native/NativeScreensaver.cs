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
        public string PresetName = "Modern Dark";
        public string Style = "modern"; // modern, classic, bauhaus, sport, minimal
        public string NumeralType = "arabic"; // arabic, roman, dots, lines, none
        public bool ShowHourHand = true;
        public bool ShowMinuteHand = true;
        public bool ShowSecondHand = true;
        public bool SmoothSweep = true;
        public bool ShowDate = true;
        public bool ShowDialBorder = true;
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
            string dir = Path.Combine(appData, "AnalogClockScreensaver");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, "clock_config.json");
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
                string path = GetConfigPath();
                string json = ToJson();
                File.WriteAllText(path, json);
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

        private ComboBox cbPreset;
        private ComboBox cbStyle;
        private ComboBox cbNumeral;
        private CheckBox chkHour;
        private CheckBox chkMin;
        private CheckBox chkSec;
        private CheckBox chkSweep;
        private CheckBox chkDate;
        private CheckBox chkBorder;
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
            this.Text = "Pengaturan Screensaver Jam Analog";
            this.Size = new Size(620, 720);
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

            TabPage tabGeneral = new TabPage("  Gaya & Jarum  ");
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
            bottomPanel.Height = 58;
            bottomPanel.BackColor = Color.FromArgb(226, 232, 240);

            // Test Preview Button
            Button btnPreview = new Button();
            btnPreview.Text = "👁️ Test Preview";
            btnPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPreview.BackColor = Color.FromArgb(59, 130, 246);
            btnPreview.ForeColor = Color.White;
            btnPreview.FlatStyle = FlatStyle.Flat;
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Size = new Size(130, 36);
            btnPreview.Location = new Point(16, 11);
            btnPreview.Cursor = Cursors.Hand;
            btnPreview.Click += (s, e) => {
                SyncUIToConfig();
                config.Save();
                using (ScreenSaverForm previewForm = new ScreenSaverForm(Screen.PrimaryScreen.Bounds))
                {
                    previewForm.ShowDialog();
                }
                Cursor.Show(); // Always guarantee cursor is shown upon returning
            };

            Button btnSave = new Button();
            btnSave.Text = "Simpan & Terapkan";
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(16, 185, 129);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Size = new Size(145, 36);
            btnSave.Location = new Point(445, 11);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += (s, e) => SaveAndClose();

            Button btnCancel = new Button();
            btnCancel.Text = "Batal";
            btnCancel.BackColor = Color.FromArgb(148, 163, 184);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Size = new Size(80, 36);
            btnCancel.Location = new Point(355, 11);
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
            int y = 14;

            // 1. Preset Group
            GroupBox gbPreset = new GroupBox();
            gbPreset.Text = " 🎨 Tema & Preset Siap Pakai ";
            gbPreset.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbPreset.Location = new Point(14, y);
            gbPreset.Size = new Size(560, 68);

            Label lblP = new Label();
            lblP.Text = "Pilih Preset:";
            lblP.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblP.Location = new Point(16, 28);
            lblP.AutoSize = true;

            cbPreset = new ComboBox();
            cbPreset.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPreset.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            cbPreset.Items.AddRange(new object[] {
                "Modern Dark", "Fliqlo Monochrome", "Classic Vintage Roman",
                "Swiss Railway (Bauhaus)", "Midnight Sapphire", "Cyberpunk Neon",
                "Minimal Slate", "Emerald Luxury", "Custom"
            });
            cbPreset.Location = new Point(110, 25);
            cbPreset.Size = new Size(260, 25);
            cbPreset.SelectedIndexChanged += (s, e) => {
                if (isUpdatingUI) return; // Prevent overwriting loaded settings!
                if (cbPreset.SelectedItem != null && cbPreset.SelectedItem.ToString() != "Custom")
                {
                    config.ApplyPreset(cbPreset.SelectedItem.ToString());
                    LoadConfigToUI();
                }
            };

            gbPreset.Controls.Add(lblP);
            gbPreset.Controls.Add(cbPreset);
            page.Controls.Add(gbPreset);
            y += 78;

            // 2. Style & Numerals Group
            GroupBox gbStyle = new GroupBox();
            gbStyle.Text = " 🕰️ Desain Jam & Penanda ";
            gbStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbStyle.Location = new Point(14, y);
            gbStyle.Size = new Size(560, 95);

            Label lblSt = new Label { Text = "Gaya Desain:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 26), AutoSize = true };
            cbStyle = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(150, 23), Size = new Size(200, 25) };
            cbStyle.Items.AddRange(new object[] { "modern", "classic", "bauhaus", "sport", "minimal" });

            Label lblNum = new Label { Text = "Tipe Angka:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 58), AutoSize = true };
            cbNumeral = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(150, 55), Size = new Size(200, 25) };
            cbNumeral.Items.AddRange(new object[] { "arabic", "roman", "dots", "lines", "none" });

            gbStyle.Controls.Add(lblSt); gbStyle.Controls.Add(cbStyle);
            gbStyle.Controls.Add(lblNum); gbStyle.Controls.Add(cbNumeral);
            page.Controls.Add(gbStyle);
            y += 105;

            // 3. Hands & Motion Group
            GroupBox gbHands = new GroupBox();
            gbHands.Text = " ⏱️ Opsi Jarum Jam & Animasi ";
            gbHands.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbHands.Location = new Point(14, y);
            gbHands.Size = new Size(560, 130);

            chkHour = new CheckBox { Text = "Tampilkan Jarum Jam (Hour Hand)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 24), AutoSize = true };
            chkMin = new CheckBox { Text = "Tampilkan Jarum Menit (Minute Hand)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 48), AutoSize = true };
            chkSec = new CheckBox { Text = "Tampilkan Jarum Detik (Second Hand)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 72), AutoSize = true };
            chkSweep = new CheckBox { Text = "Gerakan Mulus 60 FPS (Smooth Sweep Motion)", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), Location = new Point(16, 98), AutoSize = true };

            gbHands.Controls.Add(chkHour); gbHands.Controls.Add(chkMin); gbHands.Controls.Add(chkSec); gbHands.Controls.Add(chkSweep);
            page.Controls.Add(gbHands);
            y += 140;

            // 4. Display, Language & Scale Group
            GroupBox gbDisp = new GroupBox();
            gbDisp.Text = " 📐 Fitur Layar & Bahasa Tanggal ";
            gbDisp.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gbDisp.Location = new Point(14, y);
            gbDisp.Size = new Size(560, 180);

            chkDate = new CheckBox { Text = "Tampilkan Tanggal & Hari", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 24), AutoSize = true };
            chkBorder = new CheckBox { Text = "Tampilkan Garis Tepi Dial", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 48), AutoSize = true };
            chkAntiBurn = new CheckBox { Text = "Anti-Burn-In Protection (Pergeseran Mikro OLED)", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 72), AutoSize = true };

            // Date language dropdown
            Label lblDateLang = new Label { Text = "Bahasa & Format Tanggal:", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 102), AutoSize = true };
            cbDateLang = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(180, 99), Size = new Size(260, 25) };
            cbDateLang.Items.AddRange(new object[] {
                "Default Sistem (Otomatis)",
                "Bahasa Indonesia (RAB 19 AGU)",
                "English (WED 19 AUG)",
                "Indonesia Lengkap (Rabu, 19 Agustus)",
                "English Full (Wednesday, 19 August)",
                "Format Angka (19/08/2026)"
            });

            Label lblSc = new Label { Text = "Ukuran Jam (% Layar):", Font = new Font("Segoe UI", 9F, FontStyle.Regular), Location = new Point(16, 138), AutoSize = true };
            tbScale = new TrackBar { Minimum = 40, Maximum = 90, Value = 72, TickFrequency = 5, Location = new Point(180, 134), Size = new Size(200, 30) };
            lblScaleVal = new Label { Text = "72%", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(390, 138), AutoSize = true };
            tbScale.ValueChanged += (s, e) => { lblScaleVal.Text = tbScale.Value + "%"; };

            gbDisp.Controls.Add(chkDate); gbDisp.Controls.Add(chkBorder); gbDisp.Controls.Add(chkAntiBurn);
            gbDisp.Controls.Add(lblDateLang); gbDisp.Controls.Add(cbDateLang);
            gbDisp.Controls.Add(lblSc); gbDisp.Controls.Add(tbScale); gbDisp.Controls.Add(lblScaleVal);
            page.Controls.Add(gbDisp);
        }

        private void BuildColorsTab(TabPage page)
        {
            int y = 14;
            btnBg = CreateColorRow(page, "Latar Belakang Layar (Background):", ref y);
            btnDial = CreateColorRow(page, "Permukaan Piringan Jam (Dial Face):", ref y);
            btnBorder = CreateColorRow(page, "Garis Batas Piringan (Dial Border):", ref y);
            btnHourM = CreateColorRow(page, "Garis Penanda Jam (Hour Markers):", ref y);
            btnMinM = CreateColorRow(page, "Garis Penanda Menit (Minute Markers):", ref y);
            btnNum = CreateColorRow(page, "Teks Angka Jam (Numerals):", ref y);
            btnHourH = CreateColorRow(page, "Jarum Jam (Hour Hand):", ref y);
            btnMinH = CreateColorRow(page, "Jarum Menit (Minute Hand):", ref y);
            btnSecH = CreateColorRow(page, "Jarum Detik (Second Hand):", ref y);
            btnAccent = CreateColorRow(page, "Titik Poros Tengah (Accent Pin):", ref y);
            btnDateBg = CreateColorRow(page, "Kotak Latar Tanggal (Date Box BG):", ref y);
            btnDateText = CreateColorRow(page, "Teks Tanggal (Date Text):", ref y);
        }

        private Button CreateColorRow(TabPage page, string label, ref int y)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(16, y + 4);
            lbl.Size = new Size(280, 20);
            page.Controls.Add(lbl);

            Button btn = new Button();
            btn.Size = new Size(130, 26);
            btn.Location = new Point(310, y);
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
            }
            finally
            {
                isUpdatingUI = false;
            }
        }

        private void SyncUIToConfig()
        {
            config.PresetName = cbPreset.SelectedItem != null ? cbPreset.SelectedItem.ToString() : "Custom";
            config.Style = cbStyle.SelectedItem != null ? cbStyle.SelectedItem.ToString() : "modern";
            config.NumeralType = cbNumeral.SelectedItem != null ? cbNumeral.SelectedItem.ToString() : "arabic";

            config.ShowHourHand = chkHour.Checked;
            config.ShowMinuteHand = chkMin.Checked;
            config.ShowSecondHand = chkSec.Checked;
            config.SmoothSweep = chkSweep.Checked;
            config.ShowDate = chkDate.Checked;
            config.ShowDialBorder = chkBorder.Checked;
            config.AntiBurnIn = chkAntiBurn.Checked;
            config.ClockScale = tbScale.Value / 100.0f;

            // Date Lang Sync
            int dIdx = cbDateLang.SelectedIndex;
            if (dIdx == 1) config.DateFormatLanguage = "id";
            else if (dIdx == 2) config.DateFormatLanguage = "en";
            else if (dIdx == 3) config.DateFormatLanguage = "full_id";
            else if (dIdx == 4) config.DateFormatLanguage = "full_en";
            else if (dIdx == 5) config.DateFormatLanguage = "numeric";
            else config.DateFormatLanguage = "system";

            config.BgColor = btnBg.Text;
            config.DialColor = btnDial.Text;
            config.BorderColor = btnBorder.Text;
            config.HourMarkersColor = btnHourM.Text;
            config.MinuteMarkersColor = btnMinM.Text;
            config.NumeralsColor = btnNum.Text;
            config.HourHandColor = btnHourH.Text;
            config.MinuteHandColor = btnMinH.Text;
            config.SecondHandColor = btnSecH.Text;
            config.AccentCenterColor = btnAccent.Text;
            config.DateBadgeBgColor = btnDateBg.Text;
            config.DateTextColor = btnDateText.Text;
        }

        private void SaveAndClose()
        {
            SyncUIToConfig();
            config.Save();
            MessageBox.Show("Pengaturan jam analog berhasil disimpan!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private Point mouseLocation;
        private bool previewMode = false;
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

        public ScreenSaverForm(Rectangle bounds)
        {
            config = ClockConfig.Load();
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

        public ScreenSaverForm(IntPtr previewHandle)
        {
            previewMode = true;
            previewParentHwnd = previewHandle;
            config = ClockConfig.Load();
            startTime = DateTime.Now;

            this.BackColor = ParseColor(config.BgColor, Color.FromArgb(11, 15, 25));
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            SetParent(this.Handle, previewHandle);
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000)); // WS_CHILD

            Rectangle parentRect;
            GetClientRect(previewHandle, out parentRect);
            this.Size = parentRect.Size;
            this.Location = new Point(0, 0);

            SetupTimer();
        }

        private void SetupTimer()
        {
            timer = new Timer();
            timer.Interval = config.SmoothSweep ? 16 : 50; // ~60 FPS
            timer.Tick += (s, e) => this.Invalidate();
            timer.Start();
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

            // In preview mode, always resize to fit parent exactly
            if (previewMode && previewParentHwnd != IntPtr.Zero)
            {
                Rectangle parentRect;
                if (GetClientRect(previewParentHwnd, out parentRect))
                {
                    w = parentRect.Width;
                    h = parentRect.Height;
                }
            }

            int driftX = 0, driftY = 0;
            if (config.AntiBurnIn && !previewMode)
            {
                double elapsedSec = (now - startTime).TotalSeconds;
                driftX = (int)(Math.Sin(elapsedSec / 120.0 * 2 * Math.PI) * 16.0);
                driftY = (int)(Math.Cos(elapsedSec / 160.0 * 2 * Math.PI) * 14.0);
            }

            float cx = (w / 2f) + driftX;
            float cy = (h / 2f) + driftY;
            float radius = (Math.Min(w, h) / 2f) * (previewMode ? 0.85f : config.ClockScale);
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
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

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

                float dateFontSize = Math.Max(5.5f, radius * 0.058f);
                using (Font dateFont = new Font("Segoe UI", dateFontSize, FontStyle.Bold))
                using (Brush dateTextBrush = new SolidBrush(ParseColor(config.DateTextColor, Color.Gray)))
                using (Brush dateBgBrush = new SolidBrush(ParseColor(config.DateBadgeBgColor, Color.FromArgb(31, 41, 55))))
                using (Pen dateBorderPen = new Pen(borderColor, 1f))
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    SizeF sz = g.MeasureString(dateStr, dateFont);
                    float bw = sz.Width + radius * 0.08f;
                    float bh = sz.Height + radius * 0.03f;
                    float bx = cx - bw / 2f;
                    float by = cy + radius * 0.40f - bh / 2f;

                    GraphicsPath path = RoundedRect(new RectangleF(bx, by, bw, bh), Math.Max(2f, radius * 0.02f));
                    g.FillPath(dateBgBrush, path);
                    g.DrawPath(dateBorderPen, path);
                    g.DrawString(dateStr, dateFont, dateTextBrush, cx, cy + radius * 0.40f, sf);
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
            Environment.Exit(0);
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

        [STAThread]
        static void Main(string[] args)
        {
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
                    if (secondArg != null)
                    {
                        try
                        {
                            IntPtr previewHandle = new IntPtr(long.Parse(secondArg));
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
