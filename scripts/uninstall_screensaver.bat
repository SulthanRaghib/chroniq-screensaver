@echo off
title Uninstall Chroniq Screensaver
echo ==================================================
echo   UNINSTALL / RESET WINDOWS SCREENSAVER
echo ==================================================
echo.

:: 1. Terminate running instances
taskkill /f /im Chroniq.scr 2>nul
taskkill /f /im Chroniq.exe 2>nul
taskkill /f /im AnalogClock.scr 2>nul
taskkill /f /im AnalogClock.exe 2>nul

:: 2. Remove legacy installed files in Windows System directory if any
if exist "%SystemRoot%\System32\AnalogClock.scr" del /f /q "%SystemRoot%\System32\AnalogClock.scr" 2>nul
if exist "%SystemRoot%\System32\Chroniq.scr" del /f /q "%SystemRoot%\System32\Chroniq.scr" 2>nul
if exist "%SystemRoot%\SysWOW64\AnalogClock.scr" del /f /q "%SystemRoot%\SysWOW64\AnalogClock.scr" 2>nul
if exist "%SystemRoot%\SysWOW64\Chroniq.scr" del /f /q "%SystemRoot%\SysWOW64\Chroniq.scr" 2>nul

:: 3. Reset Windows Screensaver Registry to (None)
reg add "HKCU\Control Panel\Desktop" /v SCRNSAVE.EXE /t REG_SZ /d "" /f >nul
reg add "HKCU\Control Panel\Desktop" /v ScreenSaveActive /t REG_SZ /d "0" /f >nul

echo [SUKSES] Screensaver telah berhasil di-uninstall dan di-reset dari Windows!
echo.
echo Untuk memasang kembali Chroniq:
echo Klik kanan file 'dist\Chroniq.scr' lalu pilih 'Install'.
echo.
pause
