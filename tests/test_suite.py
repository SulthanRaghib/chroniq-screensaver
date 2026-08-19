"""
Automated Test Suite for Analog Clock Screensaver.
Validates configuration integrity, presets, and rendering robustness across styles.
"""

from __future__ import annotations

import os
import sys
from datetime import datetime
from pathlib import Path

# Add src/python to sys.path
python_src_dir = Path(__file__).resolve().parent.parent / "src" / "python"
sys.path.insert(0, str(python_src_dir))

import pygame

from clock_renderer import ClockRenderer
from config_manager import PRESETS, ClockConfig, ConfigManager


def run_tests() -> None:
    print("=== STARTING CLOCK ENGINE TESTS ===")

    # Initialize headless Pygame
    os.environ["SDL_VIDEODRIVER"] = "dummy"
    pygame.init()
    pygame.font.init()

    surface = pygame.Surface((1920, 1080))

    # 1. Test Config Loading & Saving
    config = ConfigManager.load()
    assert config is not None, "Failed to load config"
    print("[OK] Config loading test passed")

    # 2. Test Presets and Styles Rendering
    renderer = ClockRenderer(config)
    test_times = [
        datetime(2026, 8, 19, 10, 10, 30, 500000),
        datetime(2026, 1, 1, 0, 0, 0, 0),
        datetime(2026, 12, 31, 23, 59, 59, 999999),
        datetime(2026, 6, 15, 6, 30, 15, 250000),
    ]

    for preset_name in PRESETS:
        print(f"Testing preset: {preset_name}...")
        ConfigManager.apply_preset(preset_name, config)
        renderer.set_config(config)

        for t in test_times:
            renderer.render(surface, t, current_ticks=1000)
    print("[OK] All preset rendering tests passed")

    # 3. Test All Styles & Numeral Combinations
    styles = ["modern", "classic", "bauhaus", "sport", "minimal"]
    numerals = ["arabic", "roman", "dots", "lines", "none"]

    for st in styles:
        for num in numerals:
            config.style = st
            config.numeral_type = num
            for show_h in [True, False]:
                for show_m in [True, False]:
                    for show_s in [True, False]:
                        config.show_hour_hand = show_h
                        config.show_minute_hand = show_m
                        config.show_second_hand = show_s
                        renderer.render(surface, test_times[0], current_ticks=5000)

    print("[OK] All style, numeral, and hand combinations rendered without errors")

    # 4. Clean up
    pygame.quit()
    print("=== ALL TESTS COMPLETED SUCCESSFULLY ===")


if __name__ == "__main__":
    run_tests()
