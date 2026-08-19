@echo off
title Install Chroniq Screensaver

:: 1. Auto-request Administrator elevation (UAC Prompt)
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ==================================================
    echo   MEMINTA IZIN ADMINISTRATOR (UAC)...
    echo ==================================================
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

:: 2. Running with Admin privileges:
echo ==================================================
echo   MEMASANG CHRONIQ SCREENSAVER KE SISTEM WINDOWS
echo ==================================================
echo.

:: Tutup instance lama jika ada
taskkill /f /im Chroniq.scr 2>nul
taskkill /f /im Chroniq.exe 2>nul

:: Copy ke folder sistem Windows (System32 & SysWOW64)
copy /y "%~dp0..\dist\Chroniq.scr" "%SystemRoot%\System32\Chroniq.scr" >nul
if exist "%SystemRoot%\SysWOW64" (
    copy /y "%~dp0..\dist\Chroniq.scr" "%SystemRoot%\SysWOW64\Chroniq.scr" >nul
)

:: Daftarkan ke Registry Windows sebagai Screensaver Aktif
reg add "HKCU\Control Panel\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "%SystemRoot%\System32\Chroniq.scr" /f >nul
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d "1" /f >nul

:: Buka dialog pengaturan screensaver resmi Windows
start "" rundll32.exe desk.cpl,InstallScreenSaver "%SystemRoot%\System32\Chroniq.scr"

echo [SUKSES] Chroniq Screensaver berhasil dipasang ke sistem Windows!
echo Nama 'Chroniq' sekarang muncul permanen di menu dropdown Windows.
echo.
pause
