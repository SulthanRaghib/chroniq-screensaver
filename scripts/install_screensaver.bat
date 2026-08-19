@echo off
title Install Chroniq Screensaver
echo ==================================================
echo   INSTALLING CHRONIQ SCREENSAVER KE WINDOWS
echo ==================================================
echo.

:: 1. Copy to user LocalAppData folder (Tidak perlu Run as Administrator)
set "TARGET_DIR=%LOCALAPPDATA%\ChroniqScreensaver"
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"
copy /y "%~dp0..\dist\Chroniq.scr" "%TARGET_DIR%\Chroniq.scr" >nul
copy /y "%~dp0..\dist\Chroniq.exe" "%TARGET_DIR%\Chroniq.exe" >nul

:: 2. Register ke Windows Screensaver Registry
reg add "HKCU\Control Panel\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "%TARGET_DIR%\Chroniq.scr" /f >nul
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d "1" /f >nul

:: 3. Jalankan dialog screensaver resmi Windows
start "" rundll32.exe desk.cpl,InstallScreenSaver "%TARGET_DIR%\Chroniq.scr"

echo [BERHASIL] Chroniq Screensaver berhasil dipasang ke sistem Windows!
echo Jendela pengaturan screensaver Windows telah terbuka dengan Chroniq aktif.
echo.
pause
