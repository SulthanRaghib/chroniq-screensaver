using System;
using System.Drawing;
using System.Windows.Forms;
using Chroniq.Models;
using Chroniq.Native;
using Chroniq.Rendering;

namespace Chroniq.UI
{
    /// <summary>
    /// Configuration Dialog for Chroniq Screensaver.
    /// Provides interactive settings for mode selection, theme presets, custom color pickers, and live test preview.
    /// </summary>
    public class SettingsForm : Form
    {
        private ClockConfig config;
        private bool isUpdatingUI = false;

        // Mode radio buttons
        private RadioButton rbAnalog;
        private RadioButton rbDigital;

        // Preset and style selectors
        private ComboBox cbPreset;
        private ComboBox cbStyle;
        private ComboBox cbNumeral;

        // Digital controls
        private GroupBox gbDigital;
        private ComboBox cbDigitalStyle;
        private CheckBox chk24Hour;
        private CheckBox chkDigitalSec;

        // Analog controls
        private GroupBox gbStyle;
        private GroupBox gbHands;
        private CheckBox chkHour;
        private CheckBox chkMin;
        private CheckBox chkSec;
        private CheckBox chkSweep;
        private CheckBox chkBorder;

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

            // Bottom Panel for Action Buttons
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
            btnPreview.Size = new Size(130, 38);
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

            Button btnCancel = new Button();
            btnCancel.Text = "Batal";
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.BackColor = Color.FromArgb(148, 163, 184);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Size = new Size(85, 38);
            btnCancel.Location = new Point(365, 11);
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();

            Button btnSave = new Button();
            btnSave.Text = "💾 Simpan & Terapkan";
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(16, 185, 129);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Size = new Size(170, 38);
            btnSave.Location = new Point(460, 11);
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += (s, e) => SaveAndClose();

            bottomPanel.Controls.Add(btnPreview);
            bottomPanel.Controls.Add(btnCancel);
            bottomPanel.Controls.Add(btnSave);

            this.Controls.Add(tabs);
            this.Controls.Add(bottomPanel);

            // Build Tab Content
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

            // 1. Preset Group
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
                    config.ClockMode = currentMode;
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
                        SetColorBtn(btn, ColorHelper.ColorToHex(cd.Color));
                        cbPreset.SelectedItem = "Custom";
                    }
                }
            };

            page.Controls.Add(btn);
            y += 34;
            return btn;
        }

        private void SetColorBtn(Button btn, string hex)
        {
            Color c = ColorHelper.ParseColor(hex, Color.Black);
            btn.BackColor = c;
            btn.Text = "#" + (hex.StartsWith("#") ? hex.Substring(1) : hex).ToUpper();
            btn.ForeColor = (c.R * 0.299 + c.G * 0.587 + c.B * 0.114 > 128) ? Color.Black : Color.White;
        }

        private void LoadConfigToUI()
        {
            isUpdatingUI = true;
            try
            {
                if (config.ClockMode == "digital")
                {
                    rbDigital.Checked = true;
                    rbAnalog.Checked = false;
                }
                else
                {
                    rbAnalog.Checked = true;
                    rbDigital.Checked = false;
                }

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

        private void SaveAndClose(bool suppressMsg = false)
        {
            SyncUIToConfig();
            config.Save();
            if (!suppressMsg)
            {
                MessageBox.Show("Pengaturan jam screensaver berhasil disimpan!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Close();
        }
    }
}
