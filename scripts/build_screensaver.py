"""
Automated Build Pipeline for Chroniq Screensaver.
Compiles:
1. dist/Chroniq.scr (Official Screensaver)
2. dist/Chroniq.exe (Portable Runner)
3. dist/Chroniq_Setup.exe (Standalone 1-Click GUI Installer with requireAdministrator manifest)
4. dist/Chroniq_Windows.zip (Complete distribution package)
"""

from __future__ import annotations

import os
import shutil
import subprocess
import sys
from pathlib import Path


def build() -> None:
    scripts_dir = Path(__file__).resolve().parent
    root_dir = scripts_dir.parent
    dist_dir = root_dir / "dist"
    dist_dir.mkdir(parents=True, exist_ok=True)

    native_src_dir = root_dir / "src" / "native"
    csc_path = Path(r"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe")
    ico_path = root_dir / "assets" / "favicon.ico"
    manifest_path = native_src_dir / "Installer" / "app.manifest"

    print("==================================================")
    print("  BUILDING MODULAR CHRONIQ SCREENSAVER & SETUP")
    print("==================================================")

    if not csc_path.exists():
        print(f"[ERROR] Native compiler not found: {csc_path}")
        sys.exit(1)

    # 1. Compile Chroniq.exe & Chroniq.scr (Excluding SetupProgram.cs)
    main_cs_files = [f for f in sorted(list(native_src_dir.rglob("*.cs"))) if "Installer" not in f.parts]
    exe_file = dist_dir / "Chroniq.exe"
    scr_file = dist_dir / "Chroniq.scr"

    cmd_main = [
        str(csc_path),
        "/target:winexe",
        f"/out:{exe_file}",
        "/r:System.Windows.Forms.dll",
        "/r:System.Drawing.dll",
        "/optimize+",
    ]
    if ico_path.exists():
        cmd_main.append(f"/win32icon:{ico_path}")
    for cs_file in main_cs_files:
        cmd_main.append(str(cs_file))

    print(f"1. Compiling {len(main_cs_files)} main C# source files...")
    res_main = subprocess.run(cmd_main)
    if res_main.returncode != 0 or not exe_file.exists():
        print("[ERROR] Failed to compile main executable!")
        sys.exit(1)

    shutil.copyfile(exe_file, scr_file)

    # 2. Compile Standalone 1-Click GUI Installer (Chroniq_Setup.exe)
    setup_file = dist_dir / "Chroniq_Setup.exe"
    setup_cs_files = [
        native_src_dir / "Installer" / "SetupProgram.cs",
        native_src_dir / "AssemblyInfo.cs",
    ]

    cmd_setup = [
        str(csc_path),
        "/target:winexe",
        f"/out:{setup_file}",
        f"/resource:{scr_file},Chroniq.scr",
        "/r:System.Windows.Forms.dll",
        "/r:System.Drawing.dll",
        "/optimize+",
    ]
    if ico_path.exists():
        cmd_setup.append(f"/win32icon:{ico_path}")
    if manifest_path.exists():
        cmd_setup.append(f"/win32manifest:{manifest_path}")
    for cs_file in setup_cs_files:
        cmd_setup.append(str(cs_file))

    print("2. Compiling 1-Click Standalone GUI Installer (Chroniq_Setup.exe)...")
    res_setup = subprocess.run(cmd_setup)
    if res_setup.returncode != 0 or not setup_file.exists():
        print("[WARNING] Setup compilation failed, continuing with scr/exe...")

    # 3. Create distribution zip package
    zip_temp = root_dir / "scratch" / "zip_temp"
    if zip_temp.exists():
        shutil.rmtree(zip_temp)
    zip_temp.mkdir(parents=True, exist_ok=True)

    if setup_file.exists():
        shutil.copyfile(setup_file, zip_temp / "Chroniq_Setup.exe")
    shutil.copyfile(scr_file, zip_temp / "Chroniq.scr")
    shutil.copyfile(exe_file, zip_temp / "Chroniq.exe")

    install_bat_content = """@echo off
setlocal EnableDelayedExpansion

fltmc >nul 2>&1 || (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd.exe -ArgumentList '/c \\"\\"%~f0\\"\\"' -Verb RunAs"
    exit /b
)

title Install Chroniq Screensaver
echo ==================================================
echo   MEMASANG CHRONIQ SCREENSAVER KE SISTEM WINDOWS
echo ==================================================
echo.

set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

taskkill /f /im Chroniq.scr 2>nul
taskkill /f /im Chroniq.exe 2>nul

:: Hapus alias lama
if exist "%SystemRoot%\\System32\\PChroniq.scr" del /f /q "%SystemRoot%\\System32\\PChroniq.scr" >nul 2>nul
if exist "%SystemRoot%\\SysWOW64\\PChroniq.scr" del /f /q "%SystemRoot%\\SysWOW64\\PChroniq.scr" >nul 2>nul
if exist "%SystemRoot%\\System32\\AnalogClock.scr" del /f /q "%SystemRoot%\\System32\\AnalogClock.scr" >nul 2>nul
if exist "%SystemRoot%\\SysWOW64\\AnalogClock.scr" del /f /q "%SystemRoot%\\SysWOW64\\AnalogClock.scr" >nul 2>nul

set "SRC_FILE=%SCRIPT_DIR%Chroniq.scr"
if not exist "!SRC_FILE!" (
    if exist "%SCRIPT_DIR%..\\dist\\Chroniq.scr" (
        set "SRC_FILE=%SCRIPT_DIR%..\\dist\\Chroniq.scr"
    ) else (
        echo [ERROR] File Chroniq.scr tidak ditemukan!
        pause
        exit /b
    )
)

echo Menyalin Chroniq ke folder sistem C:\\Windows\\System32...
copy /y "!SRC_FILE!" "%SystemRoot%\\System32\\Chroniq.scr" >nul
if exist "%SystemRoot%\\SysWOW64" (
    copy /y "!SRC_FILE!" "%SystemRoot%\\SysWOW64\\Chroniq.scr" >nul
)

echo Mendaftarkan Chroniq ke Registry Windows...
reg add "HKCU\\Control Panel\\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "%SystemRoot%\\System32\\Chroniq.scr" /f >nul
reg add "HKCU\\Control Panel\\Desktop" /v ScreenSaveActive /t REG_SZ /d "1" /f >nul

echo Membuka jendela pengaturan Screen Saver Windows...
start "" rundll32.exe desk.cpl,InstallScreenSaver "%SystemRoot%\\System32\\Chroniq.scr"

echo.
echo ==================================================
echo  [SUKSES] Chroniq Screensaver berhasil dipasang!
echo ==================================================
echo Nama 'Chroniq' kini muncul permanen di menu dropdown Windows.
echo.
pause
"""
    with open(zip_temp / "Install_Chroniq.bat", "w", encoding="utf-8") as f:
        f.write(install_bat_content)

    uninstall_bat_content = """@echo off
setlocal EnableDelayedExpansion

fltmc >nul 2>&1 || (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd.exe -ArgumentList '/c \\"\\"%~f0\\"\\"' -Verb RunAs"
    exit /b
)

title Uninstall Chroniq Screensaver
echo ==================================================
echo   UNINSTALL CHRONIQ SCREENSAVER DARI WINDOWS
echo ==================================================
echo.

taskkill /f /im Chroniq.scr 2>nul
taskkill /f /im Chroniq.exe 2>nul
taskkill /f /im AnalogClock.scr 2>nul
taskkill /f /im AnalogClock.exe 2>nul

if exist "%SystemRoot%\\System32\\Chroniq.scr" (
    del /f /q "%SystemRoot%\\System32\\Chroniq.scr" >nul
    echo Menghapus Chroniq dari C:\\Windows\\System32...
)
if exist "%SystemRoot%\\SysWOW64\\Chroniq.scr" (
    del /f /q "%SystemRoot%\\SysWOW64\\Chroniq.scr" >nul
    echo Menghapus Chroniq dari C:\\Windows\\SysWOW64...
)
if exist "%SystemRoot%\\System32\\PChroniq.scr" del /f /q "%SystemRoot%\\System32\\PChroniq.scr" >nul 2>nul
if exist "%SystemRoot%\\SysWOW64\\PChroniq.scr" del /f /q "%SystemRoot%\\SysWOW64\\PChroniq.scr" >nul 2>nul
if exist "%SystemRoot%\\System32\\AnalogClock.scr" del /f /q "%SystemRoot%\\System32\\AnalogClock.scr" 2>nul
if exist "%SystemRoot%\\SysWOW64\\AnalogClock.scr" del /f /q "%SystemRoot%\\SysWOW64\\AnalogClock.scr" 2>nul

echo Mereset pengaturan Screensaver di Registry Windows...
reg add "HKCU\\Control Panel\\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "" /f >nul
reg add "HKCU\\Control Panel\\Desktop" /v ScreenSaveActive /t REG_SZ /d "0" /f >nul

echo.
echo ==================================================
echo  [SUKSES] Chroniq Screensaver berhasil di-uninstall!
echo ==================================================
echo Pilihan screensaver telah di-reset ke default.
echo.
pause
"""
    with open(zip_temp / "Uninstall_Chroniq.bat", "w", encoding="utf-8") as f:
        f.write(uninstall_bat_content)

    readme_content = """# Chroniq Screensaver - Panduan Instalasi & Kustomisasi

Chroniq adalah screensaver jam estetis performa tinggi untuk Windows 10 & 11 dengan Dual-Engine: **Analog Modern** & **Digital Flip Clock (Fliqlo Style)**.

---

## 🚀 Cara Instalasi Cepat (Pilih Salah Satu):

### Opsi 1 (Paling Mudah - 1-Click Installer GUI):
1. Klik dua kali file **`Chroniq_Setup.exe`**.
2. Klik tombol biru **"Pasang Screensaver (1-Click Install)"**.
3. Selesai! Chroniq langsung terpasang ke `C:\\Windows\\System32` dan jendela screensaver Windows akan otomatis terbuka.

### Opsi 2 (Skrip Otomatis):
1. Klik dua kali file **`Install_Chroniq.bat`**.
2. Klik **Yes** pada konfirmasi izin Administrator.

---

## 🗑️ Cara Uninstall:
1. Klik dua kali file **`Uninstall_Chroniq.bat`** atau buka **`Chroniq_Setup.exe`** lalu klik tombol merah **"Copot (Uninstall)"**.
2. Screensaver akan dihapus bersih dari sistem Windows.

---

## 🌐 Repositori Resmi GitHub:
[https://github.com/SulthanRaghib/chroniq-screensaver](https://github.com/SulthanRaghib/chroniq-screensaver)

---

## ⚙️ Kustomisasi:
Buka pengaturan Windows Screensaver (*Screen Saver Settings*) -> Pilih **Chroniq** -> Klik **Settings...**.
"""
    with open(zip_temp / "README.md", "w", encoding="utf-8") as f:
        f.write(readme_content)

    # Make zip archive
    zip_dest = dist_dir / "Chroniq_Windows"
    shutil.make_archive(str(zip_dest), "zip", zip_temp)

    # Sync to website & docs dist folders
    web_dist = root_dir / "website" / "dist"
    web_dist.mkdir(parents=True, exist_ok=True)
    docs_dist = root_dir / "docs" / "dist"
    docs_dist.mkdir(parents=True, exist_ok=True)

    for target in [web_dist, docs_dist]:
        if setup_file.exists():
            shutil.copyfile(setup_file, target / "Chroniq_Setup.exe")
        shutil.copyfile(scr_file, target / "Chroniq.scr")
        shutil.copyfile(exe_file, target / "Chroniq.exe")
        shutil.copyfile(dist_dir / "Chroniq_Windows.zip", target / "Chroniq_Windows.zip")
        shutil.copyfile(zip_temp / "README.md", target / "README.md")

    print("--------------------------------------------------")
    print("[SUCCESS] All Chroniq Packages Successfully Built!")
    print(f"1-Click Standalone Installer: {setup_file}")
    print(f"Official Screensaver File:   {scr_file}")
    print(f"Standalone Executable:       {exe_file}")
    print(f"Full Distribution Zip:       {dist_dir / 'Chroniq_Windows.zip'}")
    print("--------------------------------------------------")


if __name__ == "__main__":
    build()
