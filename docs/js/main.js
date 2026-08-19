/**
 * Chroniq Website Interaction & UI Controls
 */

document.addEventListener('DOMContentLoaded', () => {
  // Initialize the real-time clock simulator
  const clock = new ClockSimulator('clock-canvas');
  window.chroniqClock = clock;

  // 1. Mode Switcher (Analog vs Digital)
  const btnAnalog = document.getElementById('btn-mode-analog');
  const btnDigital = document.getElementById('btn-mode-digital');

  if (btnAnalog && btnDigital) {
    btnAnalog.addEventListener('click', () => {
      btnAnalog.classList.add('bg-blue-600', 'text-white', 'shadow-lg', 'shadow-blue-500/30');
      btnAnalog.classList.remove('text-gray-400', 'hover:text-white');
      btnDigital.classList.remove('bg-blue-600', 'text-white', 'shadow-lg', 'shadow-blue-500/30');
      btnDigital.classList.add('text-gray-400', 'hover:text-white');
      clock.setMode('analog');
    });

    btnDigital.addEventListener('click', () => {
      btnDigital.classList.add('bg-blue-600', 'text-white', 'shadow-lg', 'shadow-blue-500/30');
      btnDigital.classList.remove('text-gray-400', 'hover:text-white');
      btnAnalog.classList.remove('bg-blue-600', 'text-white', 'shadow-lg', 'shadow-blue-500/30');
      btnAnalog.classList.add('text-gray-400', 'hover:text-white');
      clock.setMode('digital');
    });
  }

  // 2. Preset Selector Pills
  const presetPills = document.querySelectorAll('.preset-pill');
  presetPills.forEach(pill => {
    pill.addEventListener('click', () => {
      presetPills.forEach(p => {
        p.classList.remove('border-cyan-400', 'text-cyan-400', 'bg-cyan-500/10');
        p.classList.add('border-white/10', 'text-gray-400');
      });
      pill.classList.add('border-cyan-400', 'text-cyan-400', 'bg-cyan-500/10');
      pill.classList.remove('border-white/10', 'text-gray-400');

      const presetName = pill.getAttribute('data-preset');
      clock.applyPreset(presetName);
    });
  });

  // 3. 12H / 24H Toggle
  const toggle24h = document.getElementById('toggle-24h');
  if (toggle24h) {
    toggle24h.addEventListener('click', () => {
      clock.config.use24Hour = !clock.config.use24Hour;
      toggle24h.textContent = clock.config.use24Hour ? '24H Format' : '12H (AM/PM)';
      toggle24h.classList.toggle('border-cyan-400');
    });
  }

  // 4. Smooth Sweep Toggle
  const toggleSweep = document.getElementById('toggle-sweep');
  if (toggleSweep) {
    toggleSweep.addEventListener('click', () => {
      clock.config.smoothSweep = !clock.config.smoothSweep;
      toggleSweep.textContent = clock.config.smoothSweep ? '⚡ 60 FPS Sweep' : '⏱️ 1s Classic Tick';
      toggleSweep.classList.toggle('border-blue-400');
    });
  }

  // 5. Fullscreen Simulator Overlay Mode
  const btnFullscreen = document.getElementById('btn-fullscreen');
  const clockContainer = document.getElementById('clock-container');
  const fsNotice = document.getElementById('fs-exit-notice');

  if (btnFullscreen && clockContainer) {
    const enterFullscreen = () => {
      clockContainer.classList.add('fullscreen-clock');
      if (fsNotice) fsNotice.classList.remove('hidden');
      window.dispatchEvent(new Event('resize'));
    };

    const exitFullscreen = () => {
      clockContainer.classList.remove('fullscreen-clock');
      if (fsNotice) fsNotice.classList.add('hidden');
      window.dispatchEvent(new Event('resize'));
    };

    btnFullscreen.addEventListener('click', enterFullscreen);

    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && clockContainer.classList.contains('fullscreen-clock')) {
        exitFullscreen();
      }
    });

    clockContainer.addEventListener('click', () => {
      if (clockContainer.classList.contains('fullscreen-clock')) {
        exitFullscreen();
      }
    });
  }

  // 6. Interactive FAQ Accordion
  const faqItems = document.querySelectorAll('.faq-item');
  faqItems.forEach(item => {
    const header = item.querySelector('.faq-header');
    const answer = item.querySelector('.faq-answer');
    const icon = item.querySelector('.faq-icon');

    if (header && answer) {
      header.addEventListener('click', () => {
        const isOpen = answer.classList.contains('open');
        // Close all other items
        document.querySelectorAll('.faq-answer').forEach(a => a.classList.remove('open'));
        document.querySelectorAll('.faq-icon').forEach(i => i.classList.remove('rotate-180'));

        if (!isOpen) {
          answer.classList.add('open');
          if (icon) icon.classList.add('rotate-180');
        }
      });
    }
  });

  // 7. Mobile Navigation Menu Toggle
  const btnMobileMenu = document.getElementById('btn-mobile-menu');
  const mobileMenu = document.getElementById('mobile-menu');
  if (btnMobileMenu && mobileMenu) {
    btnMobileMenu.addEventListener('click', () => {
      mobileMenu.classList.toggle('hidden');
    });
  }
});
