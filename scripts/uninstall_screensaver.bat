@echo off
setlocal EnableDelayedExpansion

:: 1. Cek hak Administrator menggunakan fltmc
fltmc >nul 2>&1 || (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd.exe -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

:: Berjalan dengan hak Administrator:
title Uninstall Chroniq Screensaver
echo ==================================================
echo   UNINSTALL CHRONIQ SCREENSAVER DARI WINDOWS
echo ==================================================
echo.

:: 1. Tutup screensaver yang sedang aktif
taskkill /f /im Chroniq.scr 2>nul
taskkill /f /im Chroniq.exe 2>nul
taskkill /f /im AnalogClock.scr 2>nul
taskkill /f /im AnalogClock.exe 2>nul

:: 2. Hapus file biner dari folder sistem Windows
if exist "%SystemRoot%\System32\Chroniq.scr" (
    del /f /q "%SystemRoot%\System32\Chroniq.scr" >nul
    echo Menghapus Chroniq dari C:\Windows\System32...
)
if exist "%SystemRoot%\SysWOW64\Chroniq.scr" (
    del /f /q "%SystemRoot%\SysWOW64\Chroniq.scr" >nul
    echo Menghapus Chroniq dari C:\Windows\SysWOW64...
)
if exist "%SystemRoot%\System32\AnalogClock.scr" del /f /q "%SystemRoot%\System32\AnalogClock.scr" 2>nul
if exist "%SystemRoot%\SysWOW64\AnalogClock.scr" del /f /q "%SystemRoot%\SysWOW64\AnalogClock.scr" 2>nul

:: 3. Reset Windows Screensaver ke (None)
echo Mereset pengaturan Screensaver di Registry Windows...
reg add "HKCU\Control Panel\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "" /f >nul
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d "0" /f >nul

echo.
echo ==================================================
echo  [SUKSES] Chroniq Screensaver berhasil di-uninstall!
echo ==================================================
echo Pilihan screensaver telah di-reset ke default.
echo.
pause
