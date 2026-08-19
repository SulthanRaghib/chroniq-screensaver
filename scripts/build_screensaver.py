"""
Automated Build Pipeline for Chroniq Screensaver.
Compiles the C# Native Windows Screensaver with embedded icon into dist/Chroniq.scr and dist/Chroniq.exe.
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

    print("==================================================")
    print("  BUILDING MODULAR CHRONIQ SCREENSAVER (.SCR & .EXE)")
    print("==================================================")

    cs_files = sorted(list(native_src_dir.rglob("*.cs")))
    if not cs_files:
        print(f"[ERROR] No C# source files found in: {native_src_dir}")
        sys.exit(1)

    if not csc_path.exists():
        print(f"[ERROR] Native compiler not found: {csc_path}")
        sys.exit(1)

    exe_file = dist_dir / "Chroniq.exe"
    scr_file = dist_dir / "Chroniq.scr"

    cmd = [
        str(csc_path),
        "/target:winexe",
        f"/out:{exe_file}",
        "/r:System.Windows.Forms.dll",
        "/r:System.Drawing.dll",
        "/optimize+",
    ]

    if ico_path.exists():
        cmd.append(f"/win32icon:{ico_path}")

    for cs_file in cs_files:
        cmd.append(str(cs_file))

    print(f"Compiling {len(cs_files)} modular C# source files with icon...")
    result = subprocess.run(cmd)

    if result.returncode == 0 and exe_file.exists():
        try:
            subprocess.run(["taskkill", "/f", "/im", "Chroniq.scr"], capture_output=True)
            subprocess.run(["taskkill", "/f", "/im", "Chroniq.exe"], capture_output=True)
            subprocess.run(["taskkill", "/f", "/im", "AnalogClock.scr"], capture_output=True)
            subprocess.run(["taskkill", "/f", "/im", "AnalogClock.exe"], capture_output=True)
        except Exception:
            pass

        shutil.copyfile(exe_file, scr_file)

        # Create distribution zip package with scripts and README
        zip_temp = root_dir / "scratch" / "zip_temp"
        zip_temp.mkdir(parents=True, exist_ok=True)
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

        readme_src = root_dir / "website" / "dist" / "README.md"
        if readme_src.exists():
            shutil.copyfile(readme_src, zip_temp / "README.md")

        # Zip package
        import zipfile
        for zip_target in [dist_dir / "Chroniq_Windows.zip", root_dir / "website" / "dist" / "Chroniq_Windows.zip", root_dir / "docs" / "dist" / "Chroniq_Windows.zip"]:
            zip_target.parent.mkdir(parents=True, exist_ok=True)
            with zipfile.ZipFile(zip_target, "w", zipfile.ZIP_DEFLATED) as zipf:
                for file_path in zip_temp.glob("*"):
                    zipf.write(file_path, file_path.name)

        # Mirror SCR & EXE to website and docs
        for folder in [root_dir / "website" / "dist", root_dir / "docs" / "dist"]:
            folder.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(scr_file, folder / "Chroniq.scr")
            shutil.copyfile(exe_file, folder / "Chroniq.exe")
            if readme_src.exists() and readme_src.resolve() != (folder / "README.md").resolve():
                shutil.copyfile(readme_src, folder / "README.md")

        shutil.rmtree(zip_temp, ignore_errors=True)

        print("--------------------------------------------------")
        print("[SUCCESS] Chroniq Screensaver Successfully Built!")
        print(f"File Size: {exe_file.stat().st_size / 1024:.1f} KB")
        print(f"Official Screensaver File (.scr): {scr_file}")
        print(f"Standalone Executable: {exe_file}")
        print(f"Distribution Zip Archive: {dist_dir / 'Chroniq_Windows.zip'}")
        print("--------------------------------------------------")
        print("Tips:")
        print("1. File siap pakai ada di folder 'dist/'")
        print("2. Klik dua kali 'dist/Chroniq_Windows.zip' untuk melihat paket installer, uninstaller & README")
        print("3. Atau klik kanan file 'dist/Chroniq.scr' lalu pilih 'Install'")
        print("--------------------------------------------------")
    else:
        print("[ERROR] Native compilation failed!")
        sys.exit(result.returncode)


if __name__ == "__main__":
    build()
