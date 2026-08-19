"""
Settings & Customization Graphical User Interface for Analog Clock Screensaver.
Provides color pickers, presets, toggles, and live previews using Tkinter.
"""

from __future__ import annotations

import subprocess
import sys
import tkinter as tk
from tkinter import colorchooser, messagebox, ttk
from typing import Callable, Dict

from config_manager import PRESETS, ClockConfig, ConfigManager


class SettingsGUI:
    """Windows Settings Configuration Window for Analog Clock Screensaver."""

    def __init__(self, on_save_callback: Callable[[], None] | None = None) -> None:
        self.config: ClockConfig = ConfigManager.load()
        self.on_save_callback = on_save_callback

        self.root = tk.Tk()
        self.root.title("Pengaturan Screensaver Jam Analog")
        self.root.geometry("640x740")
        self.root.minsize(580, 680)

        # Style configuration
        self._setup_styles()

        # Variable bindings
        self.preset_var = tk.StringVar(value=self.config.preset_name)
        self.style_var = tk.StringVar(value=self.config.style)
        self.numeral_var = tk.StringVar(value=self.config.numeral_type)

        self.show_hour_var = tk.BooleanVar(value=self.config.show_hour_hand)
        self.show_min_var = tk.BooleanVar(value=self.config.show_minute_hand)
        self.show_sec_var = tk.BooleanVar(value=self.config.show_second_hand)

        self.smooth_sweep_var = tk.BooleanVar(value=self.config.smooth_sweep)
        self.show_date_var = tk.BooleanVar(value=self.config.show_date)
        self.show_border_var = tk.BooleanVar(value=self.config.show_dial_border)
        self.anti_burnin_var = tk.BooleanVar(value=self.config.anti_burn_in)
        self.scale_var = tk.DoubleVar(value=self.config.clock_scale * 100.0)

        # Color storage dict {attribute_name: (hex_string, button_widget)}
        self.color_vars: Dict[str, str] = {}
        self.color_buttons: Dict[str, tk.Button] = {}

        self._build_ui()
        self._load_config_to_ui()

    def _setup_styles(self) -> None:
        """Applies clean, modern ttk styles."""
        style = ttk.Style(self.root)
        try:
            style.theme_use("clam")
        except Exception:
            pass

        self.root.configure(bg="#F3F4F6")

    def _build_ui(self) -> None:
        """Builds all tabs and widgets."""
        # Top Header
        header_frame = tk.Frame(self.root, bg="#1E293B", padx=16, pady=12)
        header_frame.pack(fill=tk.X)

        title_lbl = tk.Label(
            header_frame,
            text="⚙️ Pengaturan Jam Analog Screensaver",
            font=("Segoe UI", 13, "bold"),
            fg="#F8FAFC",
            bg="#1E293B",
        )
        title_lbl.pack(anchor="w")

        subtitle_lbl = tk.Label(
            header_frame,
            text="Sesuaikan gaya jam, jarum, warna, dan fitur screensaver Anda secara bebas.",
            font=("Segoe UI", 9),
            fg="#94A3B8",
            bg="#1E293B",
        )
        subtitle_lbl.pack(anchor="w", pady=(2, 0))

        # Notebook (Tabs) for organized layout
        notebook_frame = tk.Frame(self.root, bg="#F3F4F6", padx=12, pady=10)
        notebook_frame.pack(fill=tk.BOTH, expand=True)

        notebook = ttk.Notebook(notebook_frame)
        notebook.pack(fill=tk.BOTH, expand=True)

        tab_general = tk.Frame(notebook, bg="#FFFFFF", padx=16, pady=14)
        tab_colors = tk.Frame(notebook, bg="#FFFFFF", padx=16, pady=14)

        notebook.add(tab_general, text="  Gaya & Jarum  ")
        notebook.add(tab_colors, text="  Palet Warna Kustom  ")

        self._build_general_tab(tab_general)
        self._build_colors_tab(tab_colors)

        # Bottom Action Bar
        action_bar = tk.Frame(self.root, bg="#E2E8F0", padx=16, pady=12)
        action_bar.pack(fill=tk.X, side=tk.BOTTOM)

        test_btn = tk.Button(
            action_bar,
            text="👁️ Test Preview",
            font=("Segoe UI", 9, "bold"),
            bg="#3B82F6",
            fg="#FFFFFF",
            activebackground="#2563EB",
            activeforeground="#FFFFFF",
            relief=tk.FLAT,
            padx=14,
            pady=6,
            cursor="hand2",
            command=self._on_test_preview,
        )
        test_btn.pack(side=tk.LEFT)

        save_btn = tk.Button(
            action_bar,
            text="💾 Simpan & Terapkan",
            font=("Segoe UI", 9, "bold"),
            bg="#10B981",
            fg="#FFFFFF",
            activebackground="#059669",
            activeforeground="#FFFFFF",
            relief=tk.FLAT,
            padx=16,
            pady=6,
            cursor="hand2",
            command=self._on_save_and_close,
        )
        save_btn.pack(side=tk.RIGHT, padx=(8, 0))

        cancel_btn = tk.Button(
            action_bar,
            text="Batal",
            font=("Segoe UI", 9),
            bg="#94A3B8",
            fg="#FFFFFF",
            activebackground="#64748B",
            activeforeground="#FFFFFF",
            relief=tk.FLAT,
            padx=12,
            pady=6,
            cursor="hand2",
            command=self.root.destroy,
        )
        cancel_btn.pack(side=tk.RIGHT)

    def _build_general_tab(self, parent: tk.Frame) -> None:
        """Builds General & Hands settings tab."""
        # 1. Preset Selector Frame
        preset_lf = tk.LabelFrame(parent, text=" 🎨 Tema & Preset Siap Pakai ", font=("Segoe UI", 9, "bold"), bg="#FFFFFF", padx=12, pady=10)
        preset_lf.pack(fill=tk.X, pady=(0, 10))

        p_inner = tk.Frame(preset_lf, bg="#FFFFFF")
        p_inner.pack(fill=tk.X)

        tk.Label(p_inner, text="Pilih Preset:", font=("Segoe UI", 9), bg="#FFFFFF").pack(side=tk.LEFT, padx=(0, 8))

        preset_cb = ttk.Combobox(
            p_inner,
            textvariable=self.preset_var,
            values=list(PRESETS.keys()),
            state="readonly",
            width=26,
            font=("Segoe UI", 9),
        )
        preset_cb.pack(side=tk.LEFT)
        preset_cb.bind("<<ComboboxSelected>>", self._on_preset_changed)

        # 2. Styles & Numerals Frame
        style_lf = tk.LabelFrame(parent, text=" 🕰️ Desain Jam & Penanda ", font=("Segoe UI", 9, "bold"), bg="#FFFFFF", padx=12, pady=10)
        style_lf.pack(fill=tk.X, pady=(0, 10))

        s_grid = tk.Frame(style_lf, bg="#FFFFFF")
        s_grid.pack(fill=tk.X)

        tk.Label(s_grid, text="Gaya Desain (Style):", font=("Segoe UI", 9), bg="#FFFFFF").grid(row=0, column=0, sticky="w", pady=4)
        style_cb = ttk.Combobox(
            s_grid,
            textvariable=self.style_var,
            values=["modern", "classic", "bauhaus", "sport", "minimal"],
            state="readonly",
            width=18,
            font=("Segoe UI", 9),
        )
        style_cb.grid(row=0, column=1, sticky="w", padx=(10, 0), pady=4)

        tk.Label(s_grid, text="Tipe Angka/Penanda:", font=("Segoe UI", 9), bg="#FFFFFF").grid(row=1, column=0, sticky="w", pady=4)
        numeral_cb = ttk.Combobox(
            s_grid,
            textvariable=self.numeral_var,
            values=["arabic", "roman", "dots", "lines", "none"],
            state="readonly",
            width=18,
            font=("Segoe UI", 9),
        )
        numeral_cb.grid(row=1, column=1, sticky="w", padx=(10, 0), pady=4)

        # 3. Hand Toggles & Motion Frame
        hands_lf = tk.LabelFrame(parent, text=" ⏱️ Opsi Jarum Jam & Animasi ", font=("Segoe UI", 9, "bold"), bg="#FFFFFF", padx=12, pady=10)
        hands_lf.pack(fill=tk.X, pady=(0, 10))

        h_grid = tk.Frame(hands_lf, bg="#FFFFFF")
        h_grid.pack(fill=tk.X)

        tk.Checkbutton(
            h_grid, text="Tampilkan Jarum Jam (Hour Hand)", variable=self.show_hour_var,
            font=("Segoe UI", 9), bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=0, column=0, sticky="w", pady=2)

        tk.Checkbutton(
            h_grid, text="Tampilkan Jarum Menit (Minute Hand)", variable=self.show_min_var,
            font=("Segoe UI", 9), bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=1, column=0, sticky="w", pady=2)

        tk.Checkbutton(
            h_grid, text="Tampilkan Jarum Detik (Second Hand)", variable=self.show_sec_var,
            font=("Segoe UI", 9), bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=2, column=0, sticky="w", pady=2)

        tk.Checkbutton(
            h_grid, text="Gerakan Mulus 60 FPS (Smooth Sweep Motion)", variable=self.smooth_sweep_var,
            font=("Segoe UI", 9, "bold"), fg="#2563EB", bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=3, column=0, sticky="w", pady=(6, 2))

        # 4. Extra Display & Scale
        extra_lf = tk.LabelFrame(parent, text=" 📐 Fitur Layar & Ukuran ", font=("Segoe UI", 9, "bold"), bg="#FFFFFF", padx=12, pady=10)
        extra_lf.pack(fill=tk.X)

        e_grid = tk.Frame(extra_lf, bg="#FFFFFF")
        e_grid.pack(fill=tk.X)

        tk.Checkbutton(
            e_grid, text="Tampilkan Tanggal & Hari", variable=self.show_date_var,
            font=("Segoe UI", 9), bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=0, column=0, sticky="w", pady=2)

        tk.Checkbutton(
            e_grid, text="Tampilkan Garis Lingkaran Tepi Dial", variable=self.show_border_var,
            font=("Segoe UI", 9), bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=1, column=0, sticky="w", pady=2)

        tk.Checkbutton(
            e_grid, text="Anti-Burn-In Protection (Pergeseran Mikro Layar OLED)", variable=self.anti_burnin_var,
            font=("Segoe UI", 9), bg="#FFFFFF", activebackground="#FFFFFF"
        ).grid(row=2, column=0, sticky="w", pady=2)

        # Date Language
        lang_row = tk.Frame(extra_lf, bg="#FFFFFF")
        lang_row.pack(fill=tk.X, pady=(4, 2))
        tk.Label(lang_row, text="Bahasa & Format Tanggal:", font=("Segoe UI", 9), bg="#FFFFFF").pack(side=tk.LEFT)
        self.date_lang_var = tk.StringVar(value=getattr(self.config, "date_format_lang", "system"))
        date_lang_cb = ttk.Combobox(
            lang_row,
            textvariable=self.date_lang_var,
            values=["system", "id", "en", "full_id", "full_en", "numeric"],
            state="readonly",
            width=18,
            font=("Segoe UI", 9),
        )
        date_lang_cb.pack(side=tk.LEFT, padx=(10, 0))

        scale_row = tk.Frame(extra_lf, bg="#FFFFFF")
        scale_row.pack(fill=tk.X, pady=(6, 0))

        tk.Label(scale_row, text="Ukuran Jam (% Tinggi Layar):", font=("Segoe UI", 9), bg="#FFFFFF").pack(side=tk.LEFT)
        scale_slider = tk.Scale(
            scale_row,
            from_=40.0,
            to=90.0,
            orient=tk.HORIZONTAL,
            variable=self.scale_var,
            resolution=1.0,
            bg="#FFFFFF",
            highlightthickness=0,
            length=180,
        )
        scale_slider.pack(side=tk.LEFT, padx=(10, 0))

    def _build_colors_tab(self, parent: tk.Frame) -> None:
        """Builds custom color palette configurator."""
        # Scrollable container for color pickers
        canvas = tk.Canvas(parent, bg="#FFFFFF", highlightthickness=0)
        scrollbar = ttk.Scrollbar(parent, orient="vertical", command=canvas.yview)
        scroll_frame = tk.Frame(canvas, bg="#FFFFFF")

        scroll_frame.bind(
            "<Configure>",
            lambda e: canvas.configure(scrollregion=canvas.bbox("all")),
        )
        canvas.create_window((0, 0), window=scroll_frame, anchor="nw")
        canvas.configure(yscrollcommand=scrollbar.set)

        canvas.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)

        color_definitions = [
            ("background", "Latar Belakang Layar (Background)"),
            ("dial_face", "Permukaan Piringan Jam (Dial Face)"),
            ("dial_border", "Garis Batas Piringan (Dial Border)"),
            ("hour_markers", "Garis Penanda Jam (Hour Markers)"),
            ("minute_markers", "Garis Penanda Menit (Minute Markers)"),
            ("numerals", "Teks Angka Jam (Numerals)"),
            ("hour_hand", "Jarum Jam (Hour Hand)"),
            ("minute_hand", "Jarum Menit (Minute Hand)"),
            ("second_hand", "Jarum Detik (Second Hand)"),
            ("accent_center", "Titik Poros Tengah (Center Accent Pin)"),
            ("date_badge_bg", "Kotak Latar Tanggal (Date Box BG)"),
            ("date_text", "Teks Tanggal (Date Text)"),
        ]

        for attr, label_text in color_definitions:
            row = tk.Frame(scroll_frame, bg="#FFFFFF", pady=4)
            row.pack(fill=tk.X, expand=True)

            lbl = tk.Label(row, text=label_text, font=("Segoe UI", 9), bg="#FFFFFF", anchor="w", width=36)
            lbl.pack(side=tk.LEFT)

            # Color preview button
            btn = tk.Button(
                row,
                text="#000000",
                font=("Consolas", 9, "bold"),
                width=12,
                relief=tk.RIDGE,
                cursor="hand2",
                command=lambda a=attr: self._pick_color(a),
            )
            btn.pack(side=tk.LEFT, padx=(8, 0))
            self.color_buttons[attr] = btn

    def _load_config_to_ui(self) -> None:
        """Populates UI elements from current self.config."""
        self.preset_var.set(self.config.preset_name)
        self.style_var.set(self.config.style)
        self.numeral_var.set(self.config.numeral_type)

        self.show_hour_var.set(self.config.show_hour_hand)
        self.show_min_var.set(self.config.show_minute_hand)
        self.show_sec_var.set(self.config.show_second_hand)

        self.smooth_sweep_var.set(self.config.smooth_sweep)
        self.show_date_var.set(self.config.show_date)
        self.show_border_var.set(self.config.show_dial_border)
        self.anti_burnin_var.set(self.config.anti_burn_in)
        self.date_lang_var.set(getattr(self.config, "date_format_lang", "system"))
        self.scale_var.set(self.config.clock_scale * 100.0)

        # Load color values
        colors_dict = self.config.colors.__dict__
        for attr, val in colors_dict.items():
            self.color_vars[attr] = val
            if attr in self.color_buttons:
                self._update_color_button_ui(attr, val)

    def _update_color_button_ui(self, attr: str, hex_color: str) -> None:
        """Updates text and background color of a color button."""
        btn = self.color_buttons.get(attr)
        if not btn:
            return

        # Determine contrasting text color
        hex_clean = hex_color.lstrip("#")
        try:
            r, g, b = [int(hex_clean[i : i + 2], 16) for i in (0, 2, 4)]
            luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255
            fg_color = "#000000" if luminance > 0.5 else "#FFFFFF"
        except Exception:
            fg_color = "#FFFFFF"

        btn.configure(text=hex_color.upper(), bg=hex_color, fg=fg_color)

    def _pick_color(self, attr: str) -> None:
        """Opens native color chooser dialog."""
        current_hex = self.color_vars.get(attr, "#FFFFFF")
        chosen = colorchooser.askcolor(color=current_hex, title=f"Pilih Warna untuk {attr}")
        if chosen and chosen[1]:
            hex_val = chosen[1].upper()
            self.color_vars[attr] = hex_val
            self._update_color_button_ui(attr, hex_val)
            self.preset_var.set("Custom")

    def _on_preset_changed(self, event: tk.Event | None = None) -> None:
        """Handles selecting a theme preset from dropdown."""
        chosen_preset = self.preset_var.get()
        if chosen_preset in PRESETS:
            ConfigManager.apply_preset(chosen_preset, self.config)
            self._load_config_to_ui()

    def _sync_ui_to_config(self) -> None:
        """Gathers values from UI into self.config."""
        self.config.preset_name = self.preset_var.get()
        self.config.style = self.style_var.get()
        self.config.numeral_type = self.numeral_var.get()

        self.config.show_hour_hand = self.show_hour_var.get()
        self.config.show_minute_hand = self.show_min_var.get()
        self.config.show_second_hand = self.show_sec_var.get()

        self.config.smooth_sweep = self.smooth_sweep_var.get()
        self.config.show_date = self.show_date_var.get()
        self.config.show_dial_border = self.show_border_var.get()
        self.config.anti_burn_in = self.anti_burnin_var.get()
        self.config.date_format_lang = self.date_lang_var.get()
        self.config.clock_scale = self.scale_var.get() / 100.0

        for attr, hex_val in self.color_vars.items():
            if hasattr(self.config.colors, attr):
                setattr(self.config.colors, attr, hex_val)

    def _on_test_preview(self) -> None:
        """Saves temporary config and runs preview."""
        self._sync_ui_to_config()
        ConfigManager.save(self.config)

        # Launch main screensaver process in preview/test mode
        if getattr(sys, "frozen", False):
            subprocess.Popen([sys.executable, "/t"])
        else:
            subprocess.Popen([sys.executable, "main.py", "/t"])

    def _on_save_and_close(self) -> None:
        """Saves config and closes dialog."""
        self._sync_ui_to_config()
        success = ConfigManager.save(self.config)
        if success:
            if self.on_save_callback:
                self.on_save_callback()
            messagebox.showinfo("Berhasil", "Pengaturan jam analog screensaver berhasil disimpan!")
            self.root.destroy()
        else:
            messagebox.showerror("Error", "Gagal menyimpan konfigurasi.")

    def run(self) -> None:
        """Starts Tkinter main loop."""
        self.root.mainloop()


def open_settings_dialog() -> None:
    """Helper entry point to open settings."""
    app = SettingsGUI()
    app.run()


if __name__ == "__main__":
    open_settings_dialog()
