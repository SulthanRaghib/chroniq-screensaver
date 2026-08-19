"""
Configuration Manager for Analog Clock Screensaver.
Handles persistent JSON settings, validation, and theme presets.
"""

from __future__ import annotations

import json
import os
import sys
from copy import deepcopy
from dataclasses import asdict, dataclass, field
from pathlib import Path
from typing import Any, Dict


@dataclass
class ClockColors:
    background: str = "#0F1117"
    dial_face: str = "#1A1D27"
    dial_border: str = "#2A2E3D"
    hour_markers: str = "#E2E8F0"
    minute_markers: str = "#64748B"
    numerals: str = "#F8FAFC"
    hour_hand: str = "#F8FAFC"
    minute_hand: str = "#E2E8F0"
    second_hand: str = "#EF4444"
    accent_center: str = "#EF4444"
    date_text: str = "#94A3B8"
    date_badge_bg: str = "#242938"


@dataclass
class ClockConfig:
    # Mode: "analog" | "digital"
    clock_mode: str = "analog"

    # Preset & General
    preset_name: str = "Modern Dark"
    style: str = "modern"  # 'modern', 'classic', 'bauhaus', 'sport', 'minimal'
    numeral_type: str = "arabic"  # 'arabic', 'roman', 'dots', 'lines', 'none'
    
    # Hand Toggles (Analog)
    show_hour_hand: bool = True
    show_minute_hand: bool = True
    show_second_hand: bool = True
    
    # Digital Options
    digital_style: str = "flip"  # 'flip', 'minimal'
    use_24_hour: bool = True
    show_digital_seconds: bool = True

    # Animation & Behavior
    smooth_sweep: bool = True
    show_date: bool = True
    show_dial_border: bool = True
    anti_burn_in: bool = True
    
    # Geometry & Sizing
    clock_scale: float = 0.72  # 0.40 to 0.90 of screen height
    fps_target: int = 60
    date_format_lang: str = "system"  # 'system', 'id', 'en', 'full_id', 'full_en', 'numeric'
    
    # Colors
    colors: ClockColors = field(default_factory=ClockColors)

    def to_dict(self) -> Dict[str, Any]:
        return asdict(self)

    @classmethod
    def from_dict(cls, data: Dict[str, Any]) -> ClockConfig:
        if not isinstance(data, dict):
            return cls()
        
        colors_data = data.get("colors", {})
        colors = ClockColors(
            background=colors_data.get("background", "#0F1117"),
            dial_face=colors_data.get("dial_face", "#1A1D27"),
            dial_border=colors_data.get("dial_border", "#2A2E3D"),
            hour_markers=colors_data.get("hour_markers", "#E2E8F0"),
            minute_markers=colors_data.get("minute_markers", "#64748B"),
            numerals=colors_data.get("numerals", "#F8FAFC"),
            hour_hand=colors_data.get("hour_hand", "#F8FAFC"),
            minute_hand=colors_data.get("minute_hand", "#E2E8F0"),
            second_hand=colors_data.get("second_hand", "#EF4444"),
            accent_center=colors_data.get("accent_center", "#EF4444"),
            date_text=colors_data.get("date_text", "#94A3B8"),
            date_badge_bg=colors_data.get("date_badge_bg", "#242938"),
        )
        
        return cls(
            clock_mode=data.get("clock_mode", "analog"),
            preset_name=data.get("preset_name", "Modern Dark"),
            style=data.get("style", "modern"),
            numeral_type=data.get("numeral_type", "arabic"),
            show_hour_hand=data.get("show_hour_hand", True),
            show_minute_hand=data.get("show_minute_hand", True),
            show_second_hand=data.get("show_second_hand", True),
            digital_style=data.get("digital_style", "flip"),
            use_24_hour=data.get("use_24_hour", True),
            show_digital_seconds=data.get("show_digital_seconds", True),
            smooth_sweep=data.get("smooth_sweep", True),
            show_date=data.get("show_date", True),
            show_dial_border=data.get("show_dial_border", True),
            anti_burn_in=data.get("anti_burn_in", True),
            date_format_lang=data.get("date_format_lang", "system"),
            clock_scale=float(data.get("clock_scale", 0.72)),
            fps_target=int(data.get("fps_target", 60)),
            colors=colors,
        )


PRESETS: Dict[str, Dict[str, Any]] = {
    "Modern Dark": {
        "style": "modern",
        "numeral_type": "arabic",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": True,
        "show_date": True,
        "show_dial_border": True,
        "colors": {
            "background": "#0B0F19",
            "dial_face": "#111827",
            "dial_border": "#1F2937",
            "hour_markers": "#F3F4F6",
            "minute_markers": "#4B5563",
            "numerals": "#F9FAFB",
            "hour_hand": "#F9FAFB",
            "minute_hand": "#E5E7EB",
            "second_hand": "#EF4444",
            "accent_center": "#EF4444",
            "date_text": "#9CA3AF",
            "date_badge_bg": "#1F2937",
        },
    },
    "Fliqlo Monochrome": {
        "style": "modern",
        "numeral_type": "arabic",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": True,
        "show_date": False,
        "show_dial_border": False,
        "colors": {
            "background": "#0D0D0D",
            "dial_face": "#181818",
            "dial_border": "#282828",
            "hour_markers": "#E0E0E0",
            "minute_markers": "#505050",
            "numerals": "#FFFFFF",
            "hour_hand": "#FFFFFF",
            "minute_hand": "#D0D0D0",
            "second_hand": "#E5A93C",
            "accent_center": "#E5A93C",
            "date_text": "#888888",
            "date_badge_bg": "#222222",
        },
    },
    "Classic Vintage Roman": {
        "style": "classic",
        "numeral_type": "roman",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": False,
        "show_date": True,
        "show_dial_border": True,
        "colors": {
            "background": "#121110",
            "dial_face": "#F7F3E9",
            "dial_border": "#C5A059",
            "hour_markers": "#2C2A29",
            "minute_markers": "#736F6D",
            "numerals": "#1E1C1B",
            "hour_hand": "#1E1C1B",
            "minute_hand": "#2C2A29",
            "second_hand": "#8B1E1E",
            "accent_center": "#C5A059",
            "date_text": "#3D3937",
            "date_badge_bg": "#E8E2D2",
        },
    },
    "Swiss Railway (Bauhaus)": {
        "style": "bauhaus",
        "numeral_type": "none",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": True,
        "show_date": False,
        "show_dial_border": True,
        "colors": {
            "background": "#18181B",
            "dial_face": "#FFFFFF",
            "dial_border": "#E4E4E7",
            "hour_markers": "#09090B",
            "minute_markers": "#71717A",
            "numerals": "#09090B",
            "hour_hand": "#09090B",
            "minute_hand": "#09090B",
            "second_hand": "#DC2626",
            "accent_center": "#DC2626",
            "date_text": "#52525B",
            "date_badge_bg": "#F4F4F5",
        },
    },
    "Midnight Sapphire": {
        "style": "modern",
        "numeral_type": "dots",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": True,
        "show_date": True,
        "show_dial_border": True,
        "colors": {
            "background": "#030712",
            "dial_face": "#0B1528",
            "dial_border": "#1E3A8A",
            "hour_markers": "#60A5FA",
            "minute_markers": "#1E40AF",
            "numerals": "#93C5FD",
            "hour_hand": "#F0F9FF",
            "minute_hand": "#BAE6FD",
            "second_hand": "#38BDF8",
            "accent_center": "#38BDF8",
            "date_text": "#7DD3FC",
            "date_badge_bg": "#0F2445",
        },
    },
    "Cyberpunk Neon": {
        "style": "sport",
        "numeral_type": "arabic",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": True,
        "show_date": True,
        "show_dial_border": True,
        "colors": {
            "background": "#050508",
            "dial_face": "#0D0D14",
            "dial_border": "#06B6D4",
            "hour_markers": "#00F0FF",
            "minute_markers": "#4338CA",
            "numerals": "#00F0FF",
            "hour_hand": "#F43F5E",
            "minute_hand": "#FB7185",
            "second_hand": "#00F0FF",
            "accent_center": "#F43F5E",
            "date_text": "#00F0FF",
            "date_badge_bg": "#1E1B4B",
        },
    },
    "Minimal Slate": {
        "style": "minimal",
        "numeral_type": "lines",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": False,
        "smooth_sweep": True,
        "show_date": False,
        "show_dial_border": False,
        "colors": {
            "background": "#0F172A",
            "dial_face": "#1E293B",
            "dial_border": "#334155",
            "hour_markers": "#94A3B8",
            "minute_markers": "#475569",
            "numerals": "#CBD5E1",
            "hour_hand": "#F8FAFC",
            "minute_hand": "#94A3B8",
            "second_hand": "#38BDF8",
            "accent_center": "#F8FAFC",
            "date_text": "#64748B",
            "date_badge_bg": "#334155",
        },
    },
    "Emerald Luxury": {
        "style": "classic",
        "numeral_type": "roman",
        "show_hour_hand": True,
        "show_minute_hand": True,
        "show_second_hand": True,
        "smooth_sweep": True,
        "show_date": True,
        "show_dial_border": True,
        "colors": {
            "background": "#04110B",
            "dial_face": "#062317",
            "dial_border": "#D4AF37",
            "hour_markers": "#F5E6BE",
            "minute_markers": "#1B4D3E",
            "numerals": "#F5E6BE",
            "hour_hand": "#D4AF37",
            "minute_hand": "#F3E5AB",
            "second_hand": "#E63946",
            "accent_center": "#D4AF37",
            "date_text": "#D4AF37",
            "date_badge_bg": "#0B3826",
        },
    },
}


class ConfigManager:
    """Manages clock configuration storage and retrieval."""

    CONFIG_FILENAME = "clock_config.json"

    @classmethod
    def get_config_dir(cls) -> Path:
        """Determines appropriate config directory with APPDATA fallback."""
        try:
            # 1. Prefer application directory if writable
            if getattr(sys, "frozen", False):
                app_dir = Path(sys.executable).parent
            else:
                app_dir = Path(__file__).resolve().parent
            
            test_file = app_dir / ".write_test"
            try:
                test_file.touch()
                test_file.unlink()
                return app_dir
            except (PermissionError, OSError):
                pass
        except Exception:
            pass

        # 2. Fallback to %APPDATA%/AnalogScreensaver
        appdata = os.environ.get("APPDATA")
        if appdata:
            path = Path(appdata) / "AnalogClockScreensaver"
            path.mkdir(parents=True, exist_ok=True)
            return path
        
        # 3. Fallback to home directory
        path = Path.home() / ".analog_screensaver"
        path.mkdir(parents=True, exist_ok=True)
        return path

    @classmethod
    def get_config_path(cls) -> Path:
        return cls.get_config_dir() / cls.CONFIG_FILENAME

    @classmethod
    def load(cls) -> ClockConfig:
        """Loads configuration from JSON file or returns defaults."""
        path = cls.get_config_path()
        if path.exists():
            try:
                with open(path, "r", encoding="utf-8") as f:
                    data = json.load(f)
                    return ClockConfig.from_dict(data)
            except Exception as err:
                print(f"[ConfigManager] Error reading config from {path}: {err}. Using defaults.")
        
        # Default config
        config = ClockConfig()
        cls.save(config)
        return config

    @classmethod
    def save(cls, config: ClockConfig) -> bool:
        """Saves configuration to JSON file."""
        path = cls.get_config_path()
        try:
            with open(path, "w", encoding="utf-8") as f:
                json.dump(config.to_dict(), f, indent=4)
            return True
        except Exception as err:
            print(f"[ConfigManager] Error saving config to {path}: {err}")
            return False

    @classmethod
    def apply_preset(cls, preset_name: str, config: ClockConfig) -> ClockConfig:
        """Applies a built-in preset to the given config object."""
        if preset_name not in PRESETS:
            return config
        
        preset = deepcopy(PRESETS[preset_name])
        config.preset_name = preset_name
        config.style = preset.get("style", config.style)
        config.numeral_type = preset.get("numeral_type", config.numeral_type)
        config.show_hour_hand = preset.get("show_hour_hand", config.show_hour_hand)
        config.show_minute_hand = preset.get("show_minute_hand", config.show_minute_hand)
        config.show_second_hand = preset.get("show_second_hand", config.show_second_hand)
        config.smooth_sweep = preset.get("smooth_sweep", config.smooth_sweep)
        config.show_date = preset.get("show_date", config.show_date)
        config.show_dial_border = preset.get("show_dial_border", config.show_dial_border)
        
        colors = preset.get("colors", {})
        for key, val in colors.items():
            if hasattr(config.colors, key):
                setattr(config.colors, key, val)
                
        return config
