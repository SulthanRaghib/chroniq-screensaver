/**
 * Chroniq Real-Time Canvas Clock Simulator
 * High-Performance Vector Clock Engine (Analog 60 FPS & Digital Flip-Card)
 */

class ClockSimulator {
  constructor(canvasId) {
    this.canvas = document.getElementById(canvasId);
    if (!this.canvas) return;
    this.ctx = this.canvas.getContext('2d');

    // Default configuration
    this.config = {
      mode: 'analog', // 'analog' | 'digital'
      preset: 'Modern Dark',
      style: 'modern',
      numeralType: 'arabic',
      smoothSweep: true,
      use24Hour: true,
      showSeconds: true,
      showDate: true,
      showBorder: true,
      dateLang: 'id', // 'id', 'en', 'system'
      scale: 0.76,
      colors: {
        bg: '#0B0F19',
        dial: '#111827',
        border: '#1F2937',
        hourMarkers: '#F3F4F6',
        minMarkers: '#4B5563',
        numerals: '#F9FAFB',
        hourHand: '#F9FAFB',
        minHand: '#E5E7EB',
        secHand: '#EF4444',
        accent: '#EF4444',
        dateBg: '#1F2937',
        dateText: '#9CA3AF',
      }
    };

    this.startTime = Date.now();
    this.animationFrameId = null;

    this.initPresets();
    this.setupResize();
    this.start();
  }

  initPresets() {
    this.presets = {
      'Modern Dark': {
        style: 'modern', numeralType: 'arabic', showBorder: true,
        colors: { bg: '#0B0F19', dial: '#111827', border: '#1F2937', hourMarkers: '#F3F4F6', minMarkers: '#4B5563', numerals: '#F9FAFB', hourHand: '#F9FAFB', minHand: '#E5E7EB', secHand: '#EF4444', accent: '#EF4444', dateBg: '#1F2937', dateText: '#9CA3AF' }
      },
      'Fliqlo Monochrome': {
        style: 'modern', numeralType: 'arabic', showBorder: false,
        colors: { bg: '#0D0D0D', dial: '#181818', border: '#282828', hourMarkers: '#E0E0E0', minMarkers: '#505050', numerals: '#FFFFFF', hourHand: '#FFFFFF', minHand: '#D0D0D0', secHand: '#E5A93C', accent: '#E5A93C', dateBg: '#222222', dateText: '#888888' }
      },
      'Classic Vintage Roman': {
        style: 'classic', numeralType: 'roman', showBorder: true,
        colors: { bg: '#121110', dial: '#F7F3E9', border: '#C5A059', hourMarkers: '#2C2A29', minMarkers: '#736F6D', numerals: '#1E1C1B', hourHand: '#1E1C1B', minHand: '#2C2A29', secHand: '#8B1E1E', accent: '#C5A059', dateBg: '#E8E2D2', dateText: '#3D3937' }
      },
      'Swiss Railway': {
        style: 'bauhaus', numeralType: 'none', showBorder: true,
        colors: { bg: '#18181B', dial: '#FFFFFF', border: '#E4E4E7', hourMarkers: '#09090B', minMarkers: '#71717A', numerals: '#09090B', hourHand: '#09090B', minHand: '#09090B', secHand: '#DC2626', accent: '#DC2626', dateBg: '#F4F4F5', dateText: '#52525B' }
      },
      'Midnight Sapphire': {
        style: 'modern', numeralType: 'dots', showBorder: true,
        colors: { bg: '#030712', dial: '#0B1528', border: '#1E3A8A', hourMarkers: '#60A5FA', minMarkers: '#1E40AF', numerals: '#93C5FD', hourHand: '#F0F9FF', minHand: '#BAE6FD', secHand: '#38BDF8', accent: '#38BDF8', dateBg: '#0F2445', dateText: '#7DD3FC' }
      },
      'Cyberpunk Neon': {
        style: 'sport', numeralType: 'arabic', showBorder: true,
        colors: { bg: '#050508', dial: '#0D0D14', border: '#06B6D4', hourMarkers: '#00F0FF', minMarkers: '#4338CA', numerals: '#00F0FF', hourHand: '#F43F5E', minHand: '#FB7185', secHand: '#00F0FF', accent: '#F43F5E', dateBg: '#1E1B4B', dateText: '#00F0FF' }
      },
      'Minimal Slate': {
        style: 'minimal', numeralType: 'lines', showBorder: false,
        colors: { bg: '#0F172A', dial: '#1E293B', border: '#334155', hourMarkers: '#94A3B8', minMarkers: '#475569', numerals: '#CBD5E1', hourHand: '#F8FAFC', minHand: '#94A3B8', secHand: '#38BDF8', accent: '#F8FAFC', dateBg: '#334155', dateText: '#64748B' }
      },
      'Emerald Luxury': {
        style: 'classic', numeralType: 'roman', showBorder: true,
        colors: { bg: '#04110B', dial: '#062317', border: '#D4AF37', hourMarkers: '#F5E6BE', minMarkers: '#1B4D3E', numerals: '#F5E6BE', hourHand: '#D4AF37', minHand: '#F3E5AB', secHand: '#E63946', accent: '#D4AF37', dateBg: '#0B3826', dateText: '#D4AF37' }
      }
    };
  }

  applyPreset(presetName) {
    if (!this.presets[presetName]) return;
    this.config.preset = presetName;
    const p = this.presets[presetName];
    this.config.style = p.style;
    this.config.numeralType = p.numeralType;
    this.config.showBorder = p.showBorder;
    this.config.colors = { ...p.colors };
  }

  setMode(mode) {
    this.config.mode = mode;
  }

  setupResize() {
    const resize = () => {
      const rect = this.canvas.parentElement.getBoundingClientRect();
      const dpr = window.devicePixelRatio || 1;
      this.canvas.width = rect.width * dpr;
      this.canvas.height = rect.height * dpr;
      this.ctx.scale(dpr, dpr);
      this.logicalWidth = rect.width;
      this.logicalHeight = rect.height;
    };

    window.addEventListener('resize', resize);
    resize();
  }

  start() {
    const loop = () => {
      this.render();
      this.animationFrameId = requestAnimationFrame(loop);
    };
    loop();
  }

  getFormattedDate(now) {
    const daysId = ['SEN', 'SEL', 'RAB', 'KAM', 'JUM', 'SAB', 'MIN'];
    const monthsId = ['JAN', 'FEB', 'MAR', 'APR', 'MEI', 'JUN', 'JUL', 'AGU', 'SEP', 'OKT', 'NOV', 'DES'];
    const daysEn = ['MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT', 'SUN'];
    const monthsEn = ['JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'];

    const dayIdx = (now.getDay() + 6) % 7;
    const isId = this.config.dateLang === 'id';
    const dayName = isId ? daysId[dayIdx] : daysEn[dayIdx];
    const monthName = isId ? monthsId[now.getMonth()] : monthsEn[now.getMonth()];

    return `${dayName} ${now.getDate()} ${monthName}`;
  }

  render() {
    const ctx = this.ctx;
    const w = this.logicalWidth;
    const h = this.logicalHeight;
    const now = new Date();

    // Clear background
    ctx.fillStyle = this.config.colors.bg;
    ctx.fillRect(0, 0, w, h);

    if (this.config.mode === 'digital') {
      this.renderDigital(now, w, h);
    } else {
      this.renderAnalog(now, w, h);
    }
  }

  renderDigital(now, w, h) {
    const ctx = this.ctx;
    const cx = w / 2;
    const cy = h / 2;

    const baseSize = Math.min(w, h) * this.config.scale;
    const cardH = baseSize * 0.48;
    const cardW = cardH * 0.90;
    const secCardW = cardW * 0.65;
    const secCardH = cardH * 0.65;
    const gap = baseSize * 0.035;

    const use24 = this.config.use24Hour;
    const showSec = this.config.showSeconds;
    let hour = now.getHours();
    if (!use24) hour = hour % 12 || 12;

    const hrStr = String(hour).padStart(2, '0');
    const minStr = String(now.getMinutes()).padStart(2, '0');
    const secStr = String(now.getSeconds()).padStart(2, '0');
    const ampmStr = now.getHours() >= 12 ? 'PM' : 'AM';

    const totalW = showSec ? (cardW * 2 + secCardW + gap * 2) : (cardW * 2 + gap);
    const startX = cx - totalW / 2;
    const startY = cy - cardH / 2 - (this.config.showDate ? baseSize * 0.05 : 0);

    // 1. Hour Card
    this.drawDigitalCard(startX, startY, cardW, cardH, hrStr, this.config.colors.numerals, use24 ? null : ampmStr);

    // 2. Minute Card
    this.drawDigitalCard(startX + cardW + gap, startY, cardW, cardH, minStr, this.config.colors.numerals, null);

    // 3. Second Card
    if (showSec) {
      this.drawDigitalCard(startX + cardW * 2 + gap * 2, startY + (cardH - secCardH), secCardW, secCardH, secStr, this.config.colors.secHand, null);
    }

    // 4. Date Badge
    if (this.config.showDate) {
      const dateStr = this.getFormattedDate(now);
      const dateFontSize = Math.max(11, baseSize * 0.052);
      ctx.font = `bold ${dateFontSize}px 'Plus Jakarta Sans', sans-serif`;
      const textMetrics = ctx.measureText(dateStr);
      const bw = textMetrics.width + baseSize * 0.08;
      const bh = dateFontSize * 1.8;
      const bx = cx - bw / 2;
      const by = startY + cardH + baseSize * 0.06;

      ctx.fillStyle = this.config.colors.dateBg;
      this.roundRect(bx, by, bw, bh, Math.max(3, baseSize * 0.015), true, false);

      if (this.config.showBorder) {
        ctx.strokeStyle = this.config.colors.border;
        ctx.lineWidth = 1;
        this.roundRect(bx, by, bw, bh, Math.max(3, baseSize * 0.015), false, true);
      }

      ctx.fillStyle = this.config.colors.dateText;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(dateStr, cx, by + bh / 2);
    }
  }

  drawDigitalCard(x, y, w, h, text, textColor, badgeText) {
    const ctx = this.ctx;
    const cornerR = Math.max(6, h * 0.08);

    // Card background
    ctx.fillStyle = this.config.colors.dial;
    this.roundRect(x, y, w, h, cornerR, true, false);

    // Border
    if (this.config.showBorder) {
      ctx.strokeStyle = this.config.colors.border;
      ctx.lineWidth = Math.max(1, h * 0.012);
      this.roundRect(x, y, w, h, cornerR, false, true);
    }

    // Flip center crease / split line
    const midY = y + h / 2;
    ctx.strokeStyle = 'rgba(0, 0, 0, 0.4)';
    ctx.lineWidth = Math.max(1.5, h * 0.015);
    ctx.beginPath();
    ctx.moveTo(x, midY);
    ctx.lineTo(x + w, midY);
    ctx.stroke();

    ctx.strokeStyle = 'rgba(255, 255, 255, 0.08)';
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.moveTo(x, midY + 1.5);
    ctx.lineTo(x + w, midY + 1.5);
    ctx.stroke();

    // Side notch hinges
    const notchW = w * 0.035;
    const notchH = h * 0.045;
    ctx.fillStyle = this.config.colors.bg;
    ctx.fillRect(x - 1, midY - notchH / 2, notchW, notchH);
    ctx.fillRect(x + w - notchW + 1, midY - notchH / 2, notchW, notchH);

    // Digits
    const fontSize = Math.max(18, h * 0.62);
    ctx.font = `bold ${fontSize}px 'Plus Jakarta Sans', sans-serif`;
    ctx.fillStyle = textColor;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(text, x + w / 2, y + h / 2);

    // AM/PM badge
    if (badgeText) {
      const badgeFontSize = Math.max(9, h * 0.12);
      ctx.font = `bold ${badgeFontSize}px 'Plus Jakarta Sans', sans-serif`;
      ctx.textAlign = 'left';
      ctx.textBaseline = 'top';
      ctx.fillText(badgeText, x + w * 0.09, y + h * 0.08);
    }
  }

  renderAnalog(now, w, h) {
    const ctx = this.ctx;
    const cx = w / 2;
    const cy = h / 2;
    const radius = (Math.min(w, h) / 2) * this.config.scale;
    if (radius < 10) return;

    // 1. Dial Face
    if (this.config.showBorder) {
      ctx.strokeStyle = this.config.colors.border;
      ctx.lineWidth = Math.max(1.5, radius * 0.025);
      ctx.beginPath();
      ctx.arc(cx, cy, radius, 0, Math.PI * 2);
      ctx.stroke();
    }

    ctx.fillStyle = this.config.colors.dial;
    ctx.beginPath();
    ctx.arc(cx, cy, radius, 0, Math.PI * 2);
    ctx.fill();

    // 2. Markers
    const hourTickLen = radius * (this.config.style === 'bauhaus' ? 0.14 : 0.09);
    const minTickLen = radius * 0.04;
    const hourTickW = Math.max(1.5, radius * (this.config.style === 'bauhaus' ? 0.04 : 0.022));
    const minTickW = Math.max(1, radius * 0.008);

    for (let i = 0; i < 60; i++) {
      const angle = (i * 6 - 90) * (Math.PI / 180);
      const cosA = Math.cos(angle);
      const sinA = Math.sin(angle);
      const isHour = i % 5 === 0;

      if (isHour) {
        const rOuter = radius * 0.94;
        const rInner = rOuter - hourTickLen;

        if (this.config.numeralType === 'dots') {
          const dotR = Math.max(2, radius * 0.022);
          ctx.fillStyle = this.config.colors.hourMarkers;
          ctx.beginPath();
          ctx.arc(cx + rInner * cosA, cy + rInner * sinA, dotR, 0, Math.PI * 2);
          ctx.fill();
        } else {
          ctx.strokeStyle = this.config.colors.hourMarkers;
          ctx.lineWidth = hourTickW;
          ctx.lineCap = 'round';
          ctx.beginPath();
          ctx.moveTo(cx + rInner * cosA, cy + rInner * sinA);
          ctx.lineTo(cx + rOuter * cosA, cy + rOuter * sinA);
          ctx.stroke();
        }
      } else if (this.config.numeralType !== 'dots') {
        const rOuter = radius * 0.94;
        const rInner = rOuter - minTickLen;
        ctx.strokeStyle = this.config.colors.minMarkers;
        ctx.lineWidth = minTickW;
        ctx.lineCap = 'round';
        ctx.beginPath();
        ctx.moveTo(cx + rInner * cosA, cy + rInner * sinA);
        ctx.lineTo(cx + rOuter * cosA, cy + rOuter * sinA);
        ctx.stroke();
      }
    }

    // 3. Numerals
    if (this.config.numeralType === 'arabic' || this.config.numeralType === 'roman') {
      const arabics = ['12', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11'];
      const romans = ['XII', 'I', 'II', 'III', 'IV', 'V', 'VI', 'VII', 'VIII', 'IX', 'X', 'XI'];
      const list = this.config.numeralType === 'roman' ? romans : arabics;
      const fontSize = Math.max(10, radius * 0.13);

      ctx.font = `bold ${fontSize}px ${this.config.numeralType === 'roman' ? 'Georgia' : "'Plus Jakarta Sans'"}, sans-serif`;
      ctx.fillStyle = this.config.colors.numerals;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';

      const numDist = radius * 0.73;
      for (let i = 0; i < 12; i++) {
        const angle = (i * 30 - 90) * (Math.PI / 180);
        const nx = cx + numDist * Math.cos(angle);
        const ny = cy + numDist * Math.sin(angle);
        ctx.fillText(list[i], nx, ny);
      }
    }

    // 4. Date Badge
    if (this.config.showDate) {
      const dateStr = this.getFormattedDate(now);
      const dateFontSize = Math.max(9, radius * 0.058);
      ctx.font = `bold ${dateFontSize}px 'Plus Jakarta Sans', sans-serif`;
      const textMetrics = ctx.measureText(dateStr);
      const bw = textMetrics.width + radius * 0.08;
      const bh = dateFontSize * 1.8;
      const bx = cx - bw / 2;
      const by = cy + radius * 0.40 - bh / 2;

      ctx.fillStyle = this.config.colors.dateBg;
      this.roundRect(bx, by, bw, bh, Math.max(3, radius * 0.02), true, false);

      if (this.config.showBorder) {
        ctx.strokeStyle = this.config.colors.border;
        ctx.lineWidth = 1;
        this.roundRect(bx, by, bw, bh, Math.max(3, radius * 0.02), false, true);
      }

      ctx.fillStyle = this.config.colors.dateText;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(dateStr, cx, cy + radius * 0.40);
    }

    // 5. Hands Angles
    const ms = this.config.smoothSweep ? now.getMilliseconds() : 0;
    const secFrac = now.getSeconds() + ms / 1000;
    const minFrac = now.getMinutes() + secFrac / 60;
    const hrFrac = (now.getHours() % 12) + minFrac / 60;

    const secAngle = (secFrac * 6 - 90) * (Math.PI / 180);
    const minAngle = (minFrac * 6 - 90) * (Math.PI / 180);
    const hrAngle = (hrFrac * 30 - 90) * (Math.PI / 180);

    // 6. Draw Hands
    this.drawHand(cx, cy, hrAngle, radius * 0.50, radius * 0.12, radius * 0.036, this.config.colors.hourHand);
    this.drawHand(cx, cy, minAngle, radius * 0.78, radius * 0.14, radius * 0.024, this.config.colors.minHand);
    this.drawSecondHand(cx, cy, secAngle, radius * 0.85, radius * 0.18, radius, this.config.colors.secHand);

    // 7. Center Cap
    const capR = Math.max(3, radius * 0.032);
    ctx.fillStyle = this.config.colors.accent;
    ctx.beginPath();
    ctx.arc(cx, cy, capR, 0, Math.PI * 2);
    ctx.fill();
  }

  drawHand(cx, cy, angle, length, counterLen, width, color) {
    const ctx = this.ctx;
    const cosA = Math.cos(angle);
    const sinA = Math.sin(angle);
    const nx = -sinA * (width / 2);
    const ny = cosA * (width / 2);

    const tipX = cx + length * cosA;
    const tipY = cy + length * sinA;
    const tailX = cx - counterLen * cosA;
    const tailY = cy - counterLen * sinA;

    ctx.fillStyle = color;
    ctx.beginPath();
    ctx.moveTo(tailX + nx * 0.7, tailY + ny * 0.7);
    ctx.lineTo(tipX + nx * 0.3, tipY + ny * 0.3);
    ctx.lineTo(tipX, tipY);
    ctx.lineTo(tipX - nx * 0.3, tipY - ny * 0.3);
    ctx.lineTo(tailX - nx * 0.7, tailY - ny * 0.7);
    ctx.closePath();
    ctx.fill();
  }

  drawSecondHand(cx, cy, angle, length, counterLen, radius, color) {
    const ctx = this.ctx;
    const cosA = Math.cos(angle);
    const sinA = Math.sin(angle);
    const tipX = cx + length * cosA;
    const tipY = cy + length * sinA;
    const tailX = cx - counterLen * cosA;
    const tailY = cy - counterLen * sinA;

    ctx.strokeStyle = color;
    ctx.lineWidth = Math.max(1.5, radius * 0.009);
    ctx.lineCap = 'round';
    ctx.beginPath();
    ctx.moveTo(tailX, tailY);
    ctx.lineTo(tipX, tipY);
    ctx.stroke();

    if (this.config.style === 'bauhaus') {
      const discDist = length * 0.78;
      const discX = cx + discDist * cosA;
      const discY = cy + discDist * sinA;
      const discR = Math.max(4, radius * 0.05);
      ctx.fillStyle = color;
      ctx.beginPath();
      ctx.arc(discX, discY, discR, 0, Math.PI * 2);
      ctx.fill();
    } else {
      const cwX = cx - counterLen * 0.65 * cosA;
      const cwY = cy - counterLen * 0.65 * sinA;
      const cwR = Math.max(2, radius * 0.022);
      ctx.fillStyle = color;
      ctx.beginPath();
      ctx.arc(cwX, cwY, cwR, 0, Math.PI * 2);
      ctx.fill();
    }
  }

  roundRect(x, y, w, h, r, fill, stroke) {
    const ctx = this.ctx;
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.arcTo(x + w, y, x + w, y + h, r);
    ctx.arcTo(x + w, y + h, x, y + h, r);
    ctx.arcTo(x, y + h, x, y, r);
    ctx.arcTo(x, y, x + w, y, r);
    ctx.closePath();
    if (fill) ctx.fill();
    if (stroke) ctx.stroke();
  }
}

window.ClockSimulator = ClockSimulator;
