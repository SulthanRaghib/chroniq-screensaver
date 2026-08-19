# 🕰️ Windows Analog Clock Screensaver

<div align="center">

![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Language](https://img.shields.io/badge/C%23-Native%20.NET-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Python](https://img.shields.io/badge/Python-3.10+-3776AB?style=for-the-badge&logo=python&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Dual--Engine%20%7C%20Modular-blueviolet?style=for-the-badge)
![Performance](https://img.shields.io/badge/Startup-0ms%20Instant-success?style=for-the-badge&logo=speedtest&logoColor=white)

<p align="center">
  <b>Screensaver Jam Analog Modern, Elegan, dan Berperforma Tinggi untuk Windows.</b><br>
  Didesain dengan arsitektur bersih (*Clean Code & Modular*), transisi instan tanpa jeda (*Zero-Flash*), kustomisasi visual tanpa batas, dan perlindungan layar OLED (*Anti-Burn-In*).
</p>

[✨ Fitur Utama](#-fitur-utama) •
[🛠️ Teknologi](#️-teknologi-yang-digunakan) •
[📋 Prasyarat](#-prasyarat-sistem) •
[📁 Struktur Proyek](#-susunan-struktur-proyek) •
[🚀 Cara Pasang & Pakai](#-cara-pemasangan--penggunaan) •
[🧠 Logika & Arsitektur](#-arsitektur-dan-logika-sistem)

---

</div>

## 📖 Tentang Proyek

**Windows Analog Clock Screensaver** adalah aplikasi screensaver jam analog yang menggabungkan keindahan estetika minimalis (terinspirasi dari konsep Fliqlo) dengan kecanggihan grafis vektor presisi tinggi. 

Proyek ini dibangun dengan pendekatan **Dual-Engine Architecture**:
1. **Native Engine (`C# / GDI+ / Win32`)**: Menghasilkan file screensaver `.scr` berukuran sangat ringkas (**~35 KB**) dengan waktu buka instan **0.005 detik (0ms delay)** tanpa ekstraksi file ke `%TEMP%` dan tanpa kilatan layar abu-abu.
2. **Python Prototype Engine (`Pygame-ce / Tkinter`)**: Modul kode sumber Python modular untuk keperluan pengembangan, eksperimen logika, dan kompatibilitas lintas platform di masa mendatang.

---

## ✨ Fitur Utama

### 1. 🕰️ Multi-Style Dial (Gaya Jam Beragam)
- **Modern**: Tipografi bersih kontemporer dengan angka Arab `1`–`12` yang jelas dan jarum tapered tajam.
- **Classic / Vintage**: Angka Romawi (*I* – *XII*), rel menit *railroad track*, dan jarum klasik *spade/Breguet*.
- **Bauhaus / Swiss Railway**: Desain legendaris jam stasiun Swiss dengan penanda balok tebal dan jarum detik piringan merah khas.
- **Sport / Diver**: Penanda tebal kontras tinggi dengan jarum panah sporty.
- **Minimalist**: Tampilan bersih tanpa angka (hanya garis/titik penanda halus melayang).

### 2. ⏱️ Kontrol Jarum & Gerakan (Hands & Motion)
- **Independen Toggle**: Bebas menyalakan atau mematikan **Jarum Jam**, **Jarum Menit**, maupun **Jarum Detik**.
- **Smooth Continuous Sweep (60 FPS)**: Jarum detik mengalir mulus tanpa patah-patah layaknya jam tangan mekanik otomatis mewah (*luxury automatic watch*).
- **Classic Tick**: Opsi gerakan melompat 1 kali per detik seperti jam dinding quartz tradisional.

### 3. 🎨 Kustomisasi Warna Penuh & 8 Preset Siap Pakai
- **Interactive Color Picker**: Kustomisasi warna visual untuk semua elemen:
  - Latar belakang layar (*Background*)
  - Piringan jam (*Dial Face*)
  - Garis batas lingkaran (*Dial Border*)
  - Garis penanda jam & menit (*Markers*)
  - Teks angka (*Numerals*)
  - Jarum jam, menit, dan detik
  - Titik poros tengah (*Center Pin Accent*)
  - Kotak latar & teks tanggal
- **8 Preset Tema Bawaan**:
  1. 🌑 *Modern Dark* (Gelap elegan kontemporer)
  2. 🔲 *Fliqlo Monochrome* (Nuansa flip-clock hitam-putih-amber)
  3. 🏛️ *Classic Vintage Roman* (Krim, emas antik, dan angka Romawi)
  4. 🇨🇭 *Swiss Railway (Bauhaus)* (Putih bersih dengan detik merah)
  5. 🌌 *Midnight Sapphire* (Biru safir gelap dengan aksen cyan)
  6. ⚡ *Cyberpunk Neon* (Nuansa sci-fi neon magenta & cyan)
  7. 🪨 *Minimal Slate* (Abu-abu slate minimalis monokrom)
  8. 🌲 *Emerald Luxury* (Hijau zamrud tua dan aksen emas)

### 4. 🌐 Kustomisasi Format & Bahasa Tanggal (Language Support)
- **Default Sistem (Otomatis)**: Mengikuti bahasa dan format tanggal bawaan Windows OS Anda.
- **Bahasa Indonesia (RAB 19 AGU)**: Singkatan hari dan bulan bahasa Indonesia.
- **English (WED 19 AUG)**: Singkatan hari dan bulan bahasa Inggris.
- **Indonesia Lengkap (Rabu, 19 Agustus)**: Nama hari dan bulan lengkap.
- **English Full (Wednesday, 19 August)**: Nama lengkap format internasional.
- **Format Angka (19/08/2026)**: Format angka numerik ringkas.

### 5. 🛡️ Proteksi Layar OLED (Anti-Burn-In Protection)
- Jam bergeser secara mikro (*sinusoidal orbital drift*) sejauh beberapa pixel secara periodik setiap beberapa menit. Pergeseran ini tidak terasa oleh mata namun sangat efektif melindungi panel monitor OLED / AMOLED dari *image retention* atau *burn-in*.

### 6. 👁️ Jendela Pengaturan & Live Test Preview
- Dilengkapi tombol **"👁️ Test Preview"** langsung di jendela pengaturan untuk mencoba konfigurasi secara fullscreen tanpa harus menutup dialog setting.

---

## 🛠️ Teknologi yang Digunakan

| Komponen | Teknologi | Keterangan |
|---|---|---|
| **Core Screensaver** | **C# / .NET Framework 4.5+** | Dikompilasi dengan Microsoft `csc.exe` menjadi biner mandiri 35 KB. |
| **Graphics Engine** | **Windows GDI+ & Double-Buffering** | Rendering vektor 2D berkecepatan 60 FPS dengan *Anti-Aliasing* halus. |
| **OS Integration** | **Win32 API (User32 / Shell32)** | Menangani flag Windows screensaver (`/s`, `/c`, `/p`), DPI awareness, & multi-monitor. |
| **Python Prototype** | **Python 3.10+ & Pygame-ce** | Engine referensi modular untuk pengujian logika trigonometri dan rotasi. |
| **Config Engine** | **JSON Architecture** | Konfigurasi tersimpan otomatis di `%APPDATA%\AnalogClockScreensaver\clock_config.json`. |

---

## 📋 Prasyarat Sistem

### Untuk Menjalankan Screensaver:
- **Sistem Operasi**: Windows 10 atau Windows 11 (64-bit / 32-bit).
- **.NET Framework**: Versi 4.5 atau lebih baru (*Sudah terpasang secara bawaan di Windows 10 dan 11*).
- **Tanpa instalasi runtime tambahan** untuk file hasil kompilasi `dist/AnalogClock.scr`.

### Untuk Pengembangan / Kompilasi Kode Sumber (Opsional):
- **Compiler C#**: Microsoft `csc.exe` (*Sudah tersedia bawaan di folder `C:\Windows\Microsoft.NET\Framework64\`*).
- **Python (Opsional)**: Python 3.10+ dengan library `pygame-ce` jika ingin menjalankan versi Python (`pip install pygame-ce`).

---

## 📁 Susunan Struktur Proyek

```
jam-analog-screensaver/
│
├── 📂 src/                               # Seluruh Kode Sumber Utama
│   ├── 📂 native/                        # Native Windows Engine (C# / GDI+ / Win32)
│   │   └── NativeScreensaver.cs         # Engine Screensaver & Native Settings GUI
│   └── 📂 python/                        # Engine Python (Pygame / Cross-Platform Prototype)
│       ├── __init__.py
│       ├── main.py                      # Entry point Python
│       ├── clock_renderer.py            # Pygame 2D vector renderer
│       ├── config_manager.py            # JSON config & theme presets
│       └── settings_gui.py              # Tkinter GUI configurator
│
├── 📂 scripts/                           # Skrip Build & Automasi
│   ├── build_screensaver.py             # Pipeline kompilasi otomatis
│   ├── build.bat                        # Build 1-klik ke .scr
│   ├── install_screensaver.bat          # Installer 1-klik ke Windows
│   ├── open_settings.bat                # Membuka jendela pengaturan
│   ├── run_screensaver.bat              # Menjalankan screensaver fullscreen
│   └── test_windowed.bat                # Menjalankan mode jendela testing
│
├── 📂 tests/                             # Suite Pengujian Otomatis
│   └── test_suite.py                    # Automated test suite (Python engine)
│
├── 📂 dist/                              # File Hasil Jadi Siap Pasang
│   ├── AnalogClock.scr                  # Windows Screensaver Resmi (35 KB)
│   └── AnalogClock.exe                  # Standalone Executable
│
├── .gitignore                           # Aturan ignore Git
└── README.md                            # Dokumentasi resmi proyek
```

---

## 🚀 Cara Pemasangan & Penggunaan

### 1. Pemasangan ke Windows (Metode Cepat):
1. Buka folder **[`dist/`](file:///d:/Kerja/jam-analog-screensaver/dist/)**.
2. **Klik kanan** pada file **`AnalogClock.scr`** -> Pilih **Install** (atau jalankan [`scripts/install_screensaver.bat`](file:///d:/Kerja/jam-analog-screensaver/scripts/install_screensaver.bat)).
3. Jendela **Screen Saver Settings** bawaan Windows akan otomatis terbuka dengan **AnalogClock** terpilih.
4. Tentukan waktu tunggu (*Wait time*) dan klik **OK**.

### 2. Mengubah Warna & Gaya Jam:
1. Di jendela **Screen Saver Settings** Windows, klik tombol **Settings...** (atau jalankan [`scripts/open_settings.bat`](file:///d:/Kerja/jam-analog-screensaver/scripts/open_settings.bat)).
2. Pilih tema preset yang tersedia atau klik kotak warna untuk memilih warna kustom Anda sendiri.
3. Klik tombol **👁️ Test Preview** untuk melihat hasil jam secara langsung di layar monitor.
4. Klik **Simpan & Terapkan** untuk menyimpan konfigurasi secara permanen.

---

## 🧠 Arsitektur dan Logika Sistem

### 📐 1. Perhitungan Sudut Trigonometri Jarum
Posisi jarum jam dihitung berdasarkan koordinat polar lingkaran yang dikonversi ke koordinat kartesius $ (x, y) $:

$$\theta_{\text{detik}} = \left(\text{detik} + \frac{\text{milidetik}}{1000}\right) \times 6^\circ - 90^\circ$$

$$\theta_{\text{menit}} = \left(\text{menit} + \frac{\theta_{\text{detik}} + 90^\circ}{360}\right) \times 6^\circ - 90^\circ$$

$$\theta_{\text{jam}} = \left((\text{jam} \pmod{12}) + \frac{\text{menit}}{60}\right) \times 30^\circ - 90^\circ$$

$$x = x_{\text{pusat}} + r \cdot \cos(\theta), \quad y = y_{\text{pusat}} + r \cdot \sin(\theta)$$

*Pengurangan $ 90^\circ $ dilakukan karena pada koordinat layar komputer, sudut $ 0^\circ $ berada pada posisi jam 3, sedangkan titik awal jarum jam berada pada posisi jam 12.*

### ⚡ 2. Zero-Flash Window Initialization
Untuk mencegah kilatan abu-abu bawaan Windows saat jendela baru dibuat, aplikasi menginisialisasi buffer grafis sebelum jendela ditampilkan:
1. Jendela dibuat dengan style `WS_CHILD | WS_VISIBLE` atau `NOFRAME` dengan status tersembunyi.
2. Buffer grafis ganda (*Double Buffer*) langsung diisi penuh dengan warna tema dan render jam frame pertama.
3. Fungsi `ShowWindow` dan `SetForegroundWindow` dipanggil secara sinkron, sehingga saat layar monitor menyala, tampilan jam sudah 100% siap.

### 🛡️ 3. Mekanisme Exit yang Bersih
- Deteksi gerakan mouse menggunakan ambang batas Euclidean $\Delta d > 15\text{ px}$ untuk mencegah sensor mouse yang sensitif/bergetar mikro keluar dari screensaver tanpa sengaja.
- Pemanggilan `Environment.Exit(0)` memastikan seluruh handle sistem ditutup seketika dan Windows langsung mengaktifkan kembali tombol kontrol tanpa efek *freeze*.

---

## 🧪 Pengujian Otomatis (*Testing*)

Untuk memastikan seluruh kombinasi gaya dan perhitungan sudut jarum bebas dari galat (*error-free*), jalankan suite pengujian:

```bash
python tests/test_suite.py
```

---

<div align="center">

**Dibuat dengan dedikasi untuk estetika desktop Windows yang bersih dan elegan.**  
*Silakan sesuaikan, kembangkan, dan gunakan sesuai kebutuhan Anda!*

</div>
