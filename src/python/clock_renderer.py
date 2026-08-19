"""
High-Performance Anti-Aliased Analog Clock Rendering Engine.
Uses pygame and pygame.gfxdraw for vector-quality geometry.
"""

from __future__ import annotations

import math
from datetime import datetime
from typing import Dict, List, Tuple

import pygame
import pygame.gfxdraw

from config_manager import ClockConfig


def hex_to_rgb(hex_str: str) -> Tuple[int, int, int]:
    """Converts a hex color string (#RRGGBB) to an RGB tuple."""
    hex_str = hex_str.lstrip("#")
    if len(hex_str) == 3:
        hex_str = "".join([c * 2 for c in hex_str])
    try:
        return tuple(int(hex_str[i : i + 2], 16) for i in (0, 2, 4))
    except Exception:
        return (255, 255, 255)


def hex_to_rgba(hex_str: str, alpha: int = 255) -> Tuple[int, int, int, int]:
    """Converts a hex color string to an RGBA tuple."""
    r, g, b = hex_to_rgb(hex_str)
    return (r, g, b, max(0, min(255, alpha)))


class ClockRenderer:
    """Renders high-quality analog clock visuals based on ClockConfig."""

    ROMAN_NUMERALS = ["XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI"]
    ARABIC_NUMERALS = ["12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"]
    MONTHS = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"]
    DAYS = ["MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"]

    def __init__(self, config: ClockConfig) -> None:
        self.config = config
        self._font_cache: Dict[Tuple[str, int, bool], pygame.font.Font] = {}
        self._last_radius: int = 0
        self._burn_in_start_time: float = 0.0

    def set_config(self, config: ClockConfig) -> None:
        """Updates the configuration reference."""
        self.config = config

    def _get_font(self, font_name: str | None, size: int, bold: bool = False) -> pygame.font.Font:
        """Retrieves a cached Pygame font."""
        key = (font_name or "default", size, bold)
        if key not in self._font_cache:
            try:
                if font_name:
                    font = pygame.font.SysFont(font_name, size, bold=bold)
                else:
                    font = pygame.font.SysFont("Segoe UI, Arial, Helvetica, sans-serif", size, bold=bold)
            except Exception:
                font = pygame.font.Font(None, size)
            self._font_cache[key] = font
        return self._font_cache[key]

    @staticmethod
    def _draw_aa_circle(surface: pygame.Surface, x: int, y: int, radius: int, color: Tuple[int, int, int, int]) -> None:
        """Draws filled antialiased circle."""
        if radius <= 0:
            return
        pygame.gfxdraw.filled_circle(surface, x, y, radius, color)
        pygame.gfxdraw.aacircle(surface, x, y, radius, color)

    @staticmethod
    def _draw_aa_polygon(surface: pygame.Surface, points: List[Tuple[float, float]], color: Tuple[int, int, int, int]) -> None:
        """Draws filled antialiased polygon."""
        if len(points) < 3:
            return
        int_pts = [(int(round(px)), int(round(py))) for px, py in points]
        pygame.gfxdraw.filled_polygon(surface, int_pts, color)
        pygame.gfxdraw.aapolygon(surface, int_pts, color)

    @staticmethod
    def _draw_thick_line(
        surface: pygame.Surface,
        p1: Tuple[float, float],
        p2: Tuple[float, float],
        width: float,
        color: Tuple[int, int, int, int],
    ) -> None:
        """Draws a smooth anti-aliased thick line as a rotated rectangle with rounded caps."""
        x1, y1 = p1
        x2, y2 = p2
        dx = x2 - x1
        dy = y2 - y1
        length = math.hypot(dx, dy)
        if length < 0.001:
            return

        # Perpendicular normal vector
        nx = -dy / length * (width / 2.0)
        ny = dx / length * (width / 2.0)

        poly = [
            (x1 + nx, y1 + ny),
            (x2 + nx, y2 + ny),
            (x2 - nx, y2 - ny),
            (x1 - nx, y1 - ny),
        ]
        ClockRenderer._draw_aa_polygon(surface, poly, color)

        # End caps for extra smoothness
        cap_r = int(round(width / 2.0))
        if cap_r > 1:
            ClockRenderer._draw_aa_circle(surface, int(round(x1)), int(round(y1)), cap_r, color)
            ClockRenderer._draw_aa_circle(surface, int(round(x2)), int(round(y2)), cap_r, color)

    def _calculate_burn_in_offset(self, current_time_sec: float) -> Tuple[int, int]:
        """Calculates gentle periodic drift for OLED screensaver protection."""
        if not self.config.anti_burn_in:
            return 0, 0
        
        # Slow cycle every 12 minutes (720 seconds) with max 18px drift
        period = 720.0
        angle_x = (current_time_sec / period) * 2.0 * math.pi
        angle_y = (current_time_sec / (period * 1.3)) * 2.0 * math.pi
        
        offset_x = int(round(18.0 * math.sin(angle_x)))
        offset_y = int(round(15.0 * math.cos(angle_y)))
        return offset_x, offset_y

    def render(self, surface: pygame.Surface, now: datetime, current_ticks: int) -> None:
        """Main render loop: draws the complete clock onto the surface."""
        screen_w, screen_h = surface.get_size()
        
        # 1. Background Fill
        bg_rgb = hex_to_rgb(self.config.colors.background)
        surface.fill(bg_rgb)

        # 2. Geometric Center & Sizing
        time_sec = current_ticks / 1000.0
        drift_x, drift_y = self._calculate_burn_in_offset(time_sec)
        center_x = (screen_w // 2) + drift_x
        center_y = (screen_h // 2) + drift_y

        min_dim = min(screen_w, screen_h)
        radius = int(round((min_dim / 2.0) * self.config.clock_scale))
        if radius < 20:
            return

        if getattr(self.config, "clock_mode", "analog") == "digital":
            self._render_digital_clock(surface, center_x, center_y, min_dim, now)
            return

        # 3. Draw Dial Face & Borders
        self._render_dial_face(surface, center_x, center_y, radius)

        # 4. Draw Dial Markers & Numerals
        self._render_dial_markers_and_numerals(surface, center_x, center_y, radius)

        # 5. Draw Date Indicator (if enabled)
        if self.config.show_date:
            self._render_date_badge(surface, center_x, center_y, radius, now)

        # 6. Calculate Time Angles
        hour_angle, minute_angle, second_angle = self._calculate_hand_angles(now)

        # 7. Draw Hands (Hour, Minute, Second)
        self._render_hands(
            surface,
            center_x,
            center_y,
            radius,
            hour_angle,
            minute_angle,
            second_angle,
        )

    def _render_dial_face(self, surface: pygame.Surface, cx: int, cy: int, radius: int) -> None:
        """Renders the dial backdrop and border."""
        dial_rgba = hex_to_rgba(self.config.colors.dial_face)
        border_rgba = hex_to_rgba(self.config.colors.dial_border)

        # Outer Border Ring
        if self.config.show_dial_border:
            border_width = max(2, int(round(radius * 0.025)))
            self._draw_aa_circle(surface, cx, cy, radius + border_width, border_rgba)

        # Main Dial Circle
        self._draw_aa_circle(surface, cx, cy, radius, dial_rgba)

    def _render_dial_markers_and_numerals(self, surface: pygame.Surface, cx: int, cy: int, radius: int) -> None:
        """Renders hour ticks, minute ticks, and optional numerals."""
        hour_color = hex_to_rgba(self.config.colors.hour_markers)
        minute_color = hex_to_rgba(self.config.colors.minute_markers)
        numeral_color = hex_to_rgba(self.config.colors.numerals)

        style = self.config.style
        numeral_type = self.config.numeral_type

        # Tick marks
        minute_tick_len = radius * 0.04
        hour_tick_len = radius * 0.09
        if style == "bauhaus":
            hour_tick_len = radius * 0.14
            minute_tick_len = radius * 0.05
        elif style == "minimal":
            hour_tick_len = radius * 0.06
            minute_tick_len = radius * 0.025

        minute_tick_width = max(1.0, radius * 0.008)
        hour_tick_width = max(2.0, radius * 0.022)

        # Draw 60 minute / second ticks
        for i in range(60):
            angle = (i * 6.0) - 90.0
            rad = math.radians(angle)
            cos_a = math.cos(rad)
            sin_a = math.sin(rad)

            is_hour = (i % 5 == 0)

            if is_hour:
                r_outer = radius * 0.94
                r_inner = r_outer - hour_tick_len
                p1 = (cx + r_inner * cos_a, cy + r_inner * sin_a)
                p2 = (cx + r_outer * cos_a, cy + r_outer * sin_a)
                
                if style == "bauhaus":
                    # Bauhaus uses bold block markers for hours
                    self._draw_thick_line(surface, p1, p2, hour_tick_width * 1.8, hour_color)
                elif numeral_type == "dots":
                    dot_r = int(round(radius * 0.024))
                    self._draw_aa_circle(surface, int(round(p1[0])), int(round(p1[1])), dot_r, hour_color)
                else:
                    self._draw_thick_line(surface, p1, p2, hour_tick_width, hour_color)
            else:
                # Minute markers
                if numeral_type not in ["lines", "dots"] or style == "classic" or style == "bauhaus" or style == "modern":
                    r_outer = radius * 0.94
                    r_inner = r_outer - minute_tick_len
                    p1 = (cx + r_inner * cos_a, cy + r_inner * sin_a)
                    p2 = (cx + r_outer * cos_a, cy + r_outer * sin_a)
                    self._draw_thick_line(surface, p1, p2, minute_tick_width, minute_color)

        # Draw Numerals (if applicable)
        if numeral_type in ["arabic", "roman"]:
            font_size = max(14, int(round(radius * (0.13 if numeral_type == "roman" else 0.14))))
            is_classic = (style == "classic" or numeral_type == "roman")
            font = self._get_font("Georgia" if is_classic else "Segoe UI, Montserrat, Arial", font_size, bold=not is_classic)
            
            num_dist = radius * 0.73
            num_list = self.ROMAN_NUMERALS if numeral_type == "roman" else self.ARABIC_NUMERALS

            for idx, text in enumerate(num_list):
                # 12 is at idx 0 (-90 deg), 1 is at idx 1 (-60 deg), etc.
                angle = (idx * 30.0) - 90.0
                rad = math.radians(angle)
                nx = cx + num_dist * math.cos(rad)
                ny = cy + num_dist * math.sin(rad)

                text_surf = font.render(text, True, numeral_color[:3])
                t_rect = text_surf.get_rect(center=(int(round(nx)), int(round(ny))))
                surface.blit(text_surf, t_rect)

    def _get_formatted_date_str(self, now: datetime) -> str:
        lang = getattr(self.config, "date_format_lang", "system")
        if lang == "id":
            id_days = ["SEN", "SEL", "RAB", "KAM", "JUM", "SAB", "MIN"]
            id_months = ["JAN", "FEB", "MAR", "APR", "MEI", "JUN", "JUL", "AGU", "SEP", "OKT", "NOV", "DES"]
            return f"{id_days[now.weekday()]} {now.day} {id_months[now.month - 1]}"
        elif lang == "en":
            en_days = ["MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"]
            en_months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"]
            return f"{en_days[now.weekday()]} {now.day} {en_months[now.month - 1]}"
        elif lang == "full_id":
            id_days_full = ["Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu", "Minggu"]
            id_months_full = ["Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember"]
            return f"{id_days_full[now.weekday()]}, {now.day} {id_months_full[now.month - 1]}"
        elif lang == "full_en":
            en_days_full = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
            en_months_full = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
            return f"{en_days_full[now.weekday()]}, {now.day} {en_months_full[now.month - 1]}"
        elif lang == "numeric":
            return now.strftime("%d/%m/%Y")
        else:
            # Default system format
            try:
                import locale
                locale.setlocale(locale.LC_TIME, "")
                day_name = now.strftime("%a").upper().rstrip(".")
                month_name = now.strftime("%b").upper().rstrip(".")
                return f"{day_name} {now.day} {month_name}"
            except Exception:
                return f"{self.DAYS[now.weekday()]} {now.day} {self.MONTHS[now.month - 1]}"

    def _render_date_badge(self, surface: pygame.Surface, cx: int, cy: int, radius: int, now: datetime) -> None:
        """Renders an elegant date badge box."""
        badge_bg = hex_to_rgba(self.config.colors.date_badge_bg)
        text_color = hex_to_rgb(self.config.colors.date_text)

        date_str = self._get_formatted_date_str(now)

        font_size = max(11, int(round(radius * 0.058)))
        font = self._get_font(None, font_size, bold=True)
        text_surf = font.render(date_str, True, text_color)

        # Place badge below center (around 60% down towards 6 o'clock)
        badge_w = text_surf.get_width() + int(radius * 0.08)
        badge_h = text_surf.get_height() + int(radius * 0.03)
        badge_cx = cx
        badge_cy = cy + int(radius * 0.40)

        badge_rect = pygame.Rect(
            badge_cx - badge_w // 2,
            badge_cy - badge_h // 2,
            badge_w,
            badge_h,
        )

        # Draw rounded rectangle for badge
        border_radius = max(3, int(radius * 0.015))
        pygame.draw.rect(surface, badge_bg[:3], badge_rect, border_radius=border_radius)
        
        # Border around badge
        border_color = hex_to_rgba(self.config.colors.dial_border)
        pygame.draw.rect(surface, border_color[:3], badge_rect, width=1, border_radius=border_radius)

        # Blit text in center of badge
        t_rect = text_surf.get_rect(center=(badge_cx, badge_cy))
        surface.blit(text_surf, t_rect)

    def _calculate_hand_angles(self, now: datetime) -> Tuple[float, float, float]:
        """Calculates precise angles (in degrees) for hour, minute, and second hands."""
        microsecond = now.microsecond if self.config.smooth_sweep else 0
        second_frac = now.second + (microsecond / 1_000_000.0)
        minute_frac = now.minute + (second_frac / 60.0)
        hour_frac = (now.hour % 12) + (minute_frac / 60.0)

        second_angle = (second_frac * 6.0) - 90.0
        minute_angle = (minute_frac * 6.0) - 90.0
        hour_angle = (hour_frac * 30.0) - 90.0

        return hour_angle, minute_angle, second_angle

    def _render_hands(
        self,
        surface: pygame.Surface,
        cx: int,
        cy: int,
        radius: int,
        hour_angle: float,
        minute_angle: float,
        second_angle: float,
    ) -> None:
        """Renders hour, minute, second hands based on the selected visual style."""
        style = self.config.style

        hour_color = hex_to_rgba(self.config.colors.hour_hand)
        minute_color = hex_to_rgba(self.config.colors.minute_hand)
        second_color = hex_to_rgba(self.config.colors.second_hand)
        accent_color = hex_to_rgba(self.config.colors.accent_center)

        # Hand Lengths
        hour_len = radius * 0.50
        minute_len = radius * 0.78
        second_len = radius * 0.85
        counter_len = radius * 0.18

        # 1. Hour Hand
        if self.config.show_hour_hand:
            self._draw_single_hand(
                surface, cx, cy, hour_angle, hour_len, counter_len * 0.7,
                width=radius * 0.038, color=hour_color, style=style, is_hour=True
            )

        # 2. Minute Hand
        if self.config.show_minute_hand:
            self._draw_single_hand(
                surface, cx, cy, minute_angle, minute_len, counter_len * 0.8,
                width=radius * 0.026, color=minute_color, style=style, is_hour=False
            )

        # 3. Second Hand
        if self.config.show_second_hand:
            self._draw_second_hand(
                surface, cx, cy, second_angle, second_len, counter_len,
                radius=radius, color=second_color, style=style
            )

        # 4. Center Cap / Pin
        cap_radius = max(3, int(round(radius * 0.032)))
        self._draw_aa_circle(surface, cx, cy, cap_radius + 2, hex_to_rgba(self.config.colors.dial_face))
        self._draw_aa_circle(surface, cx, cy, cap_radius, accent_color)
        self._draw_aa_circle(surface, cx, cy, max(1, cap_radius // 2), (255, 255, 255, 200))

    def _draw_single_hand(
        self,
        surface: pygame.Surface,
        cx: int,
        cy: int,
        angle_deg: float,
        length: float,
        counter_len: float,
        width: float,
        color: Tuple[int, int, int, int],
        style: str,
        is_hour: bool,
    ) -> None:
        """Renders hour or minute hand geometry according to chosen style."""
        rad = math.radians(angle_deg)
        cos_a = math.cos(rad)
        sin_a = math.sin(rad)

        # Normal vector (perpendicular)
        nx = -sin_a * (width / 2.0)
        ny = cos_a * (width / 2.0)

        if style == "bauhaus" or style == "minimal":
            # Bauhaus: Clean straight rectangular batons
            tip_x = cx + length * cos_a
            tip_y = cy + length * sin_a
            tail_x = cx - counter_len * cos_a
            tail_y = cy - counter_len * sin_a

            poly = [
                (tail_x + nx, tail_y + ny),
                (tip_x + nx, tip_y + ny),
                (tip_x - nx, tip_y - ny),
                (tail_x - nx, tail_y - ny),
            ]
            self._draw_aa_polygon(surface, poly, color)

        elif style == "classic":
            # Classic / Spade / Vintage Breguet style hand
            tip_x = cx + length * cos_a
            tip_y = cy + length * sin_a
            tail_x = cx - counter_len * cos_a
            tail_y = cy - counter_len * sin_a

            # Shaft
            poly = [
                (tail_x + nx * 0.8, tail_y + ny * 0.8),
                (tip_x + nx * 0.4, tip_y + ny * 0.4),
                (tip_x - nx * 0.4, tip_y - ny * 0.4),
                (tail_x - nx * 0.8, tail_y - ny * 0.8),
            ]
            self._draw_aa_polygon(surface, poly, color)

            # Classic pear/diamond embellishment near tip
            ring_center_dist = length * 0.72
            ring_cx = cx + ring_center_dist * cos_a
            ring_cy = cy + ring_center_dist * sin_a
            ring_r = int(round(width * 1.5))
            self._draw_aa_circle(surface, int(round(ring_cx)), int(round(ring_cy)), ring_r, color)
            self._draw_aa_circle(
                surface, int(round(ring_cx)), int(round(ring_cy)), max(1, ring_r // 2),
                hex_to_rgba(self.config.colors.dial_face)
            )

        elif style == "sport":
            # Sport / Diver arrow sword hands
            tip_x = cx + length * cos_a
            tip_y = cy + length * sin_a
            tail_x = cx - counter_len * cos_a
            tail_y = cy - counter_len * sin_a
            mid_dist = length * 0.75
            mid_x = cx + mid_dist * cos_a
            mid_y = cy + mid_dist * sin_a

            poly = [
                (tail_x + nx * 0.6, tail_y + ny * 0.6),
                (mid_x + nx * 1.3, mid_y + ny * 1.3),
                (tip_x, tip_y),
                (mid_x - nx * 1.3, mid_y - ny * 1.3),
                (tail_x - nx * 0.6, tail_y - ny * 0.6),
            ]
            self._draw_aa_polygon(surface, poly, color)

        else:
            # Modern: Clean tapered hands with pointed tip
            tip_x = cx + length * cos_a
            tip_y = cy + length * sin_a
            tail_x = cx - counter_len * cos_a
            tail_y = cy - counter_len * sin_a

            poly = [
                (tail_x + nx * 0.7, tail_y + ny * 0.7),
                (tip_x + nx * 0.3, tip_y + ny * 0.3),
                (tip_x, tip_y),
                (tip_x - nx * 0.3, tip_y - ny * 0.3),
                (tail_x - nx * 0.7, tail_y - ny * 0.7),
            ]
            self._draw_aa_polygon(surface, poly, color)

    def _draw_second_hand(
        self,
        surface: pygame.Surface,
        cx: int,
        cy: int,
        angle_deg: float,
        length: float,
        counter_len: float,
        radius: int,
        color: Tuple[int, int, int, int],
        style: str,
    ) -> None:
        """Renders fine second hand with distinct counterweights."""
        rad = math.radians(angle_deg)
        cos_a = math.cos(rad)
        sin_a = math.sin(rad)

        tip_x = cx + length * cos_a
        tip_y = cy + length * sin_a
        tail_x = cx - counter_len * cos_a
        tail_y = cy - counter_len * sin_a

        # Needle shaft
        line_width = max(1.5, radius * 0.009)
        self._draw_thick_line(surface, (tail_x, tail_y), (tip_x, tip_y), line_width, color)

        if style == "bauhaus":
            # Iconic Swiss Railway red disc near tip
            disc_dist = length * 0.78
            disc_cx = cx + disc_dist * cos_a
            disc_cy = cy + disc_dist * sin_a
            disc_r = max(4, int(round(radius * 0.05)))
            self._draw_aa_circle(surface, int(round(disc_cx)), int(round(disc_cy)), disc_r, color)
        else:
            # Subtle round counterweight at tail
            cw_cx = cx - (counter_len * 0.65) * cos_a
            cw_cy = cy - (counter_len * 0.65) * sin_a
            cw_r = max(2, int(round(radius * 0.022)))
            self._draw_aa_circle(surface, int(round(cw_cx)), int(round(cw_cy)), cw_r, color)

    def _render_digital_clock(self, surface: pygame.Surface, cx: int, cy: int, min_dim: int, now: datetime) -> None:
        """Renders Fliqlo-style flip cards or minimal digital clock."""
        scale = getattr(self.config, "clock_scale", 0.72)
        base_size = min_dim * scale
        
        use_24h = getattr(self.config, "use_24_hour", True)
        show_sec = getattr(self.config, "show_digital_seconds", True)
        is_flip = getattr(self.config, "digital_style", "flip") == "flip"

        hour_val = now.hour if use_24h else (12 if now.hour % 12 == 0 else now.hour % 12)
        hr_str = f"{hour_val:02d}"
        min_str = f"{now.minute:02d}"
        sec_str = f"{now.second:02d}"
        ampm_str = "PM" if now.hour >= 12 else "AM"

        card_h = int(base_size * 0.46)
        card_w = int(card_h * 0.90)
        sec_card_w = int(card_w * 0.65)
        sec_card_h = int(card_h * 0.65)
        gap = int(base_size * 0.035)

        total_w = (card_w * 2 + sec_card_w + gap * 2) if show_sec else (card_w * 2 + gap)
        start_x = cx - total_w // 2
        start_y = cy - card_h // 2 - (int(base_size * 0.05) if self.config.show_date else 0)

        card_bg = hex_to_rgb(self.config.colors.dial_face)
        card_border = hex_to_rgb(self.config.colors.dial_border)
        digit_color = hex_to_rgb(self.config.colors.numerals)
        sec_color = hex_to_rgb(self.config.colors.second_hand)

        # 1. Hour Card
        self._draw_digital_tile(surface, start_x, start_y, card_w, card_h, hr_str, digit_color, card_bg, card_border, is_flip, None if use_24h else ampm_str)

        # 2. Minute Card
        self._draw_digital_tile(surface, start_x + card_w + gap, start_y, card_w, card_h, min_str, digit_color, card_bg, card_border, is_flip, None)

        # 3. Seconds Card (if enabled)
        if show_sec:
            sec_y = start_y + (card_h - sec_card_h)
            self._draw_digital_tile(surface, start_x + card_w * 2 + gap * 2, sec_y, sec_card_w, sec_card_h, sec_str, sec_color, card_bg, card_border, is_flip, None)

        # 4. Date Badge
        if self.config.show_date:
            date_str = self._get_formatted_date_str(now)
            font_size = max(12, int(round(base_size * 0.052)))
            font = self._get_font(None, font_size, bold=True)
            text_surf = font.render(date_str, True, hex_to_rgb(self.config.colors.date_text))

            badge_w = text_surf.get_width() + int(base_size * 0.08)
            badge_h = text_surf.get_height() + int(base_size * 0.03)
            badge_x = cx - badge_w // 2
            badge_y = start_y + card_h + int(base_size * 0.06)

            badge_rect = pygame.Rect(badge_x, badge_y, badge_w, badge_h)
            badge_bg = hex_to_rgb(self.config.colors.date_badge_bg)
            border_radius = max(3, int(base_size * 0.015))

            pygame.draw.rect(surface, badge_bg, badge_rect, border_radius=border_radius)
            if self.config.show_dial_border:
                pygame.draw.rect(surface, card_border, badge_rect, width=1, border_radius=border_radius)
            
            t_rect = text_surf.get_rect(center=badge_rect.center)
            surface.blit(text_surf, t_rect)

    def _draw_digital_tile(self, surface: pygame.Surface, x: int, y: int, w: int, h: int, text: str, text_color: Tuple[int, int, int], bg_color: Tuple[int, int, int], border_color: Tuple[int, int, int], is_flip: bool, badge_text: str | None) -> None:
        rect = pygame.Rect(x, y, w, h)
        corner_r = max(4, int(h * 0.08))

        if is_flip:
            pygame.draw.rect(surface, bg_color, rect, border_radius=corner_r)
            if self.config.show_dial_border:
                pygame.draw.rect(surface, border_color, rect, width=max(1, int(h * 0.012)), border_radius=corner_r)

            # Center crease / split groove
            mid_y = y + h // 2
            pygame.draw.line(surface, (10, 10, 15), (x, mid_y), (x + w, mid_y), max(1, int(h * 0.012)))
            pygame.draw.line(surface, (70, 70, 80), (x, mid_y + 1), (x + w, mid_y + 1), 1)

        # Draw Large Digits
        font_size = max(16, int(round(h * 0.62)))
        font = self._get_font(None, font_size, bold=True)
        t_surf = font.render(text, True, text_color)
        t_rect = t_surf.get_rect(center=rect.center)
        surface.blit(t_surf, t_rect)

        # AM/PM badge
        if badge_text:
            badge_font = self._get_font(None, max(9, int(h * 0.12)), bold=True)
            b_surf = badge_font.render(badge_text, True, text_color)
            surface.blit(b_surf, (x + int(w * 0.08), y + int(h * 0.07)))
