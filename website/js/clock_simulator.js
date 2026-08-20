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

    // 3D Split-Flap animation states (Fliqlo mechanical fold)
    this.flipStates = {
      hour: { prev: null, curr: null, start: 0, dur: 450 },
      min:  { prev: null, curr: null, start: 0, dur: 450 },
      sec:  { prev: null, curr: null, start: 0, dur: 380 }
    };

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
    this.resize = () => {
      if (!this.canvas || !this.canvas.parentElement) return;
      const rect = this.canvas.parentElement.getBoundingClientRect();
      const dpr = window.devicePixelRatio || 1;

      const displayWidth = Math.floor(rect.width);
      const displayHeight = Math.floor(rect.height);
      if (displayWidth <= 0 || displayHeight <= 0) return;

      this.canvas.width = Math.round(displayWidth * dpr);
      this.canvas.height = Math.round(displayHeight * dpr);
      this.canvas.style.width = displayWidth + 'px';
      this.canvas.style.height = displayHeight + 'px';

      this.ctx.setTransform(1, 0, 0, 1, 0, 0);
      this.ctx.scale(dpr, dpr);
      this.logicalWidth = displayWidth;
      this.logicalHeight = displayHeight;
    };

    window.addEventListener('resize', this.resize);
    if (window.ResizeObserver && this.canvas.parentElement) {
      const ro = new ResizeObserver(() => this.resize());
      ro.observe(this.canvas.parentElement);
    }
    this.resize();
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
    const cardH = baseSize * 0.44;
    const cardW = cardH * 1.15;
    const secCardW = cardW * 0.58;
    const secCardH = cardH * 0.58;
    const gap = baseSize * 0.055;

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
    const startY = cy - cardH / 2 - (this.config.showDate ? baseSize * 0.04 : 0);

    // 1. Hour Flip Card
    this.drawFlipCard(startX, startY, cardW, cardH, 'hour', hrStr, this.config.colors.numerals, use24 ? null : ampmStr);

    // 2. Minute Flip Card
    this.drawFlipCard(startX + cardW + gap, startY, cardW, cardH, 'min', minStr, this.config.colors.numerals, null);

    // 3. Second Flip Card
    if (showSec) {
      this.drawFlipCard(startX + cardW * 2 + gap * 2, startY + (cardH - secCardH), secCardW, secCardH, 'sec', secStr, this.config.colors.secHand, null);
    }

    // 4. Date Badge
    if (this.config.showDate) {
      const dateStr = this.getFormattedDate(now);
      const dateFontSize = Math.max(10, baseSize * 0.048);
      ctx.font = `bold ${dateFontSize}px 'Plus Jakarta Sans', sans-serif`;
      const textMetrics = ctx.measureText(dateStr);
      const bw = textMetrics.width + baseSize * 0.09;
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

  drawFlipCard(x, y, w, h, key, newText, textColor, badgeText) {
    const state = this.flipStates[key];
    const nowMs = performance.now();

    if (state.curr !== newText) {
      if (state.curr !== null) {
        state.prev = state.curr;
        state.start = nowMs;
      } else {
        state.prev = newText;
      }
      state.curr = newText;
    }

    const elapsed = nowMs - state.start;
    const duration = state.dur || 420;
    const progress = Math.min(1.0, elapsed / duration);

    if (progress >= 1.0 || state.prev === state.curr) {
      // Static Card (no flip active)
      this.drawHalfCard(x, y, w, h, state.curr, textColor, badgeText, true, 1.0, 0);
      this.drawHalfCard(x, y, w, h, state.curr, textColor, null, false, 1.0, 0);
    } else {
      // Smooth 3D mechanical flip easing
      const p = progress < 0.5 ? 2 * progress * progress : 1 - Math.pow(-2 * progress + 2, 2) / 2;

      if (p <= 0.5) {
        const scaleY = Math.cos(p * Math.PI);
        const shadow = p * 1.1;

        // 1. Static Bottom (shows previous number with shadow darkening)
        this.drawHalfCard(x, y, w, h, state.prev, textColor, null, false, 1.0, p * 0.45);

        // 2. Static Top Behind (reveals next number)
        this.drawHalfCard(x, y, w, h, state.curr, textColor, badgeText, true, 1.0, 0);

        // 3. Flipping Top Flap (folds down showing previous number)
        this.drawHalfCard(x, y, w, h, state.prev, textColor, badgeText, true, scaleY, shadow);
      } else {
        const scaleY = -Math.cos(p * Math.PI);
        const shadow = (1.0 - p) * 1.1;

        // 1. Static Bottom Behind (shows previous number)
        this.drawHalfCard(x, y, w, h, state.prev, textColor, null, false, 1.0, 0);

        // 2. Static Top (shows next number)
        this.drawHalfCard(x, y, w, h, state.curr, textColor, badgeText, true, 1.0, 0);

        // 3. Flipping Bottom Flap (drops down showing next number)
        this.drawHalfCard(x, y, w, h, state.curr, textColor, null, false, scaleY, shadow);
      }
    }

    // Crease line & side hinges
    this.drawCreaseAndHinges(x, y, w, h);
  }

  drawHalfCard(x, y, w, h, text, textColor, badgeText, isTop, scaleY = 1.0, shadowAlpha = 0.0) {
    const ctx = this.ctx;
    const cornerR = Math.max(6, h * 0.09);
    const midY = y + h / 2;
    const cx = x + w / 2;
    const cy = y + h / 2;

    ctx.save();

    if (scaleY < 0.999) {
      ctx.translate(cx, midY);
      ctx.scale(1, Math.max(0.001, scaleY));
      ctx.translate(-cx, -midY);
    }

    // Clip to half card
    if (isTop) {
      this.roundTopHalf(x, y, w, h, cornerR);
    } else {
      this.roundBottomHalf(x, y, w, h, cornerR);
    }
    ctx.clip();

    // Fill background
    ctx.fillStyle = this.config.colors.dial;
    if (isTop) {
      this.roundTopHalf(x, y, w, h, cornerR);
    } else {
      this.roundBottomHalf(x, y, w, h, cornerR);
    }
    ctx.fill();

    // Border
    if (this.config.showBorder) {
      ctx.strokeStyle = this.config.colors.border;
      ctx.lineWidth = Math.max(1.2, h * 0.014);
      ctx.stroke();
    }

    // Digits (100% perfectly aligned and centered across top & bottom halves)
    const fontSize = Math.max(16, h * 0.50);
    ctx.font = `bold ${fontSize}px 'Plus Jakarta Sans', sans-serif`;
    ctx.fillStyle = textColor;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(text, cx, cy);

    // AM/PM badge (top-left indicator pill, 100% isolated and separated with plenty of vertical clearance)
    if (isTop && badgeText) {
      const badgeFontSize = Math.max(7, h * 0.070);
      ctx.font = `bold ${badgeFontSize}px 'Plus Jakarta Sans', sans-serif`;
      const textMetrics = ctx.measureText(badgeText);
      const px = x + w * 0.055;
      const py = y + h * 0.055;
      const pw = textMetrics.width + w * 0.020;
      const ph = badgeFontSize * 1.5;

      ctx.fillStyle = 'rgba(0, 0, 0, 0.45)';
      this.roundRect(px, py, pw, ph, Math.max(2, h * 0.015), true, false);

      ctx.fillStyle = 'rgba(255, 255, 255, 0.90)';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(badgeText, px + pw / 2, py + ph / 2);
    }

    // 3D Shadow shading overlay
    if (shadowAlpha > 0.01) {
      ctx.fillStyle = `rgba(0, 0, 0, ${Math.min(0.85, shadowAlpha)})`;
      if (isTop) {
        ctx.fillRect(x - 2, y - 2, w + 4, h / 2 + 4);
      } else {
        ctx.fillRect(x - 2, midY - 2, w + 4, h / 2 + 4);
      }
    }

    ctx.restore();
  }

  drawCreaseAndHinges(x, y, w, h) {
    const ctx = this.ctx;
    const midY = y + h / 2;

    // Flip center crease / split line
    ctx.strokeStyle = 'rgba(0, 0, 0, 0.55)';
    ctx.lineWidth = Math.max(1.5, h * 0.018);
    ctx.beginPath();
    ctx.moveTo(x, midY);
    ctx.lineTo(x + w, midY);
    ctx.stroke();

    ctx.strokeStyle = 'rgba(255, 255, 255, 0.09)';
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.moveTo(x, midY + 1.2);
    ctx.lineTo(x + w, midY + 1.2);
    ctx.stroke();

    // Side notch hinges
    const notchW = w * 0.035;
    const notchH = h * 0.050;
    ctx.fillStyle = this.config.colors.bg;
    ctx.fillRect(x - 1, midY - notchH / 2, notchW, notchH);
    ctx.fillRect(x + w - notchW + 1, midY - notchH / 2, notchW, notchH);
  }

  roundTopHalf(x, y, w, h, r) {
    const midY = y + h / 2;
    const ctx = this.ctx;
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.lineTo(x + w - r, y);
    ctx.quadraticCurveTo(x + w, y, x + w, y + r);
    ctx.lineTo(x + w, midY);
    ctx.lineTo(x, midY);
    ctx.lineTo(x, y + r);
    ctx.quadraticCurveTo(x, y, x + r, y);
    ctx.closePath();
  }

  roundBottomHalf(x, y, w, h, r) {
    const midY = y + h / 2;
    const ctx = this.ctx;
    ctx.beginPath();
    ctx.moveTo(x, midY);
    ctx.lineTo(x + w, midY);
    ctx.lineTo(x + w, y + h - r);
    ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
    ctx.lineTo(x + r, y + h);
    ctx.quadraticCurveTo(x, y + h, x, y + h - r);
    ctx.lineTo(x, midY);
    ctx.closePath();
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

    // 3. Numerals (Arabic or Roman)
    if (this.config.numeralType === 'arabic' || this.config.numeralType === 'roman') {
      const isRoman = this.config.numeralType === 'roman';
      const arabics = ['12', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11'];
      const romans = ['XII', 'I', 'II', 'III', 'IV', 'V', 'VI', 'VII', 'VIII', 'IX', 'X', 'XI'];
      const list = isRoman ? romans : arabics;
      const fontSize = Math.max(9, radius * (isRoman ? 0.105 : 0.125));

      ctx.font = `${isRoman ? '600' : 'bold'} ${fontSize}px ${isRoman ? "'Georgia', 'Times New Roman', serif" : "'Plus Jakarta Sans', sans-serif"}`;
      ctx.fillStyle = this.config.colors.numerals;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';

      const numDist = radius * (isRoman ? 0.76 : 0.74);
      for (let i = 0; i < 12; i++) {
        const angle = (i * 30 - 90) * (Math.PI / 180);
        const nx = cx + numDist * Math.cos(angle);
        const ny = cy + numDist * Math.sin(angle);
        ctx.fillText(list[i], nx, ny);
      }
    }

    // 4. Date Badge (Positioned with optimal clearance from numerals)
    if (this.config.showDate) {
      const dateStr = this.getFormattedDate(now);
      const isRoman = this.config.numeralType === 'roman';
      const dateFontSize = Math.max(8, radius * (isRoman ? 0.050 : 0.054));
      ctx.font = `bold ${dateFontSize}px 'Plus Jakarta Sans', sans-serif`;
      const textMetrics = ctx.measureText(dateStr);
      const bw = textMetrics.width + radius * 0.06;
      const bh = dateFontSize * 1.6;
      const dateY = cy + radius * (isRoman ? 0.32 : 0.36);
      const bx = cx - bw / 2;
      const by = dateY - bh / 2;

      ctx.fillStyle = this.config.colors.dateBg;
      this.roundRect(bx, by, bw, bh, Math.max(3, radius * 0.015), true, false);

      if (this.config.showBorder) {
        ctx.strokeStyle = this.config.colors.border;
        ctx.lineWidth = 1;
        this.roundRect(bx, by, bw, bh, Math.max(3, radius * 0.015), false, true);
      }

      ctx.fillStyle = this.config.colors.dateText;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(dateStr, cx, dateY);
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
