"""
Main Entry Point for Analog Clock Windows Screensaver.
Handles Windows Screensaver flags (/s, /c, /p, /t), DPI scaling, and input events.
"""

from __future__ import annotations

import ctypes
import math
import sys
from datetime import datetime
from typing import Tuple

import pygame

from clock_renderer import ClockRenderer
from config_manager import ConfigManager
from settings_gui import open_settings_dialog


def set_windows_dpi_awareness() -> None:
    """Enables per-monitor DPI awareness on Windows for crisp high-res rendering."""
    try:
        # Try Per-Monitor V2 DPI awareness (Windows 10/11)
        ctypes.windll.shcore.SetProcessDpiAwareness(2)
    except Exception:
        try:
            # Fallback to system DPI awareness
            ctypes.windll.user32.SetProcessDPIAware()
        except Exception:
            pass


def run_screensaver(fullscreen: bool = True) -> None:
    """Runs the main screensaver visual loop with seamless zero-flash transition."""
    import os
    os.environ["SDL_VIDEO_WINDOW_POS"] = "0,0"
    os.environ["SDL_WINDOWS_DPI_AWARENESS"] = "permonitorv2"

    set_windows_dpi_awareness()
    pygame.init()
    pygame.font.init()

    # Load persistent configuration
    config = ConfigManager.load()
    renderer = ClockRenderer(config)

    # Display Setup
    display_info = pygame.display.Info()
    screen_w, screen_h = display_info.current_w, display_info.current_h

    if fullscreen:
        # Start completely HIDDEN so Windows DWM never renders an unpainted gray window
        flags = pygame.FULLSCREEN | pygame.DOUBLEBUF | pygame.HWSURFACE | pygame.NOFRAME | pygame.HIDDEN
        surface = pygame.display.set_mode((screen_w, screen_h), flags)
        pygame.mouse.set_visible(False)
    else:
        # Windowed test mode
        test_w = min(1200, int(screen_w * 0.8))
        test_h = min(800, int(screen_h * 0.8))
        flags = pygame.DOUBLEBUF | pygame.RESIZABLE | pygame.HIDDEN
        surface = pygame.display.set_mode((test_w, test_h), flags)
        pygame.display.set_caption("Analog Clock Screensaver (Test Mode)")
        pygame.mouse.set_visible(True)

    # Pre-render clock into both front and back buffers while window is still hidden
    now = datetime.now()
    ticks = pygame.time.get_ticks()
    renderer.render(surface, now, ticks)
    pygame.display.flip()
    renderer.render(surface, now, ticks)
    pygame.display.flip()

    # Now that the frame buffer is 100% rendered with the clock, reveal the window instantly!
    try:
        wm_info = pygame.display.get_wm_info()
        hwnd = wm_info.get("window")
        if hwnd:
            # SW_SHOW = 5, SW_MAXIMIZE = 3
            ctypes.windll.user32.ShowWindow(hwnd, 5)
            ctypes.windll.user32.SetForegroundWindow(hwnd)
            ctypes.windll.user32.SetFocus(hwnd)
    except Exception:
        pass

    clock = pygame.time.Clock()
    running = True

    # Mouse jitter filter: screensaver should not exit on sub-pixel sensor vibration
    initial_mouse_pos: Tuple[int, int] | None = None
    mouse_threshold = 20.0  # pixels

    target_fps = 60 if config.smooth_sweep else 30

    while running:
        current_ticks = pygame.time.get_ticks()
        now = datetime.now()

        # Handle Events
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False

            elif event.type == pygame.KEYDOWN:
                # Any key press exits screensaver
                running = False

            elif event.type == pygame.MOUSEBUTTONDOWN:
                # Any mouse click exits screensaver
                running = False

            elif event.type == pygame.MOUSEMOTION:
                if fullscreen:
                    mx, my = event.pos
                    if initial_mouse_pos is None:
                        initial_mouse_pos = (mx, my)
                    else:
                        dx = mx - initial_mouse_pos[0]
                        dy = my - initial_mouse_pos[1]
                        dist = math.hypot(dx, dy)
                        if dist > mouse_threshold:
                            running = False

            elif event.type == pygame.VIDEORESIZE:
                surface = pygame.display.set_mode((event.w, event.h), pygame.DOUBLEBUF | pygame.RESIZABLE)

        # Render Analog Clock Frame
        renderer.render(surface, now, current_ticks)
        pygame.display.flip()

        clock.tick(target_fps)

    pygame.quit()


def handle_preview_mode(hwnd_str: str | None = None) -> None:
    """Handles Windows mini-preview pane (/p <hwnd>). Exits cleanly if unsupported."""
    # Standard fallback for preview without native parent embedding
    sys.exit(0)


def main() -> None:
    """Parses command line arguments according to Windows Screensaver specs."""
    args = sys.argv[1:]

    if not args:
        # Default run: fullscreen screensaver
        run_screensaver(fullscreen=True)
        return

    first_arg = args[0].lower()

    if first_arg.startswith("/c"):
        # Configuration / Settings Dialog
        open_settings_dialog()

    elif first_arg.startswith("/s"):
        # Screensaver Fullscreen Mode
        run_screensaver(fullscreen=True)

    elif first_arg.startswith("/p"):
        # Preview Mode in Screen Saver control panel
        hwnd = args[1] if len(args) > 1 else None
        handle_preview_mode(hwnd)

    elif first_arg in ["/t", "-t", "--test", "/w", "--windowed"]:
        # Windowed test mode
        run_screensaver(fullscreen=False)

    else:
        # Any other argument defaults to fullscreen
        run_screensaver(fullscreen=True)


if __name__ == "__main__":
    main()
