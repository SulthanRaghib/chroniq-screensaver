@echo off
setlocal EnableDelayedExpansion

:: 1. Cek hak Administrator menggunakan fltmc (Built-in Windows tool)
fltmc >nul 2>&1 || (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd.exe -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

:: Berjalan dengan hak Administrator:
title Install Chroniq Screensaver
echo ==================================================
echo   MEMASANG CHRONIQ SCREENSAVER KE SISTEM WINDOWS
echo ==================================================
echo.

set "SCRIPT_DIR=%~dp0"
cd /d "%SCRIPT_DIR%"

taskkill /f /im Chroniq.scr 2>nul
taskkill /f /im Chroniq.exe 2>nul

set "SRC_FILE=%SCRIPT_DIR%..\dist\Chroniq.scr"
if not exist "!SRC_FILE!" (
    if exist "%SCRIPT_DIR%Chroniq.scr" (
        set "SRC_FILE=%SCRIPT_DIR%Chroniq.scr"
    ) else (
        echo [ERROR] File Chroniq.scr tidak ditemukan!
        pause
        exit /b
    )
)

echo Menyalin Chroniq ke folder sistem C:\Windows\System32...
copy /y "!SRC_FILE!" "%SystemRoot%\System32\Chroniq.scr" >nul
if exist "%SystemRoot%\SysWOW64" (
    copy /y "!SRC_FILE!" "%SystemRoot%\SysWOW64\Chroniq.scr" >nul
)

echo Mendaftarkan Chroniq ke Registry Windows...
reg add "HKCU\Control Panel\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "%SystemRoot%\System32\Chroniq.scr" /f >nul
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d "1" /f >nul

echo Membuka jendela pengaturan Screen Saver Windows...
start "" rundll32.exe desk.cpl,InstallScreenSaver "%SystemRoot%\System32\Chroniq.scr"

echo.
echo ==================================================
echo  [SUKSES] Chroniq Screensaver berhasil dipasang!
echo ==================================================
echo Nama 'Chroniq' kini muncul permanen di menu dropdown Windows.
echo.
pause
