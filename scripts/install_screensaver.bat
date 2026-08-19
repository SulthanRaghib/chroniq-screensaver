@echo off
title Memasang Chroniq Screensaver ke Windows
echo ========================================================
echo   MEMASANG CHRONIQ SCREENSAVER KE WINDOWS
echo ========================================================
echo.
echo Membangun biner screensaver terbaru...
python "%~dp0build_screensaver.py"
echo.
echo Menginstal ke Windows...
start "" "%~dp0..\dist\Chroniq.scr" /s
echo.
echo Selesai! Anda juga dapat klik kanan 'dist\Chroniq.scr' lalu pilih 'Install'.
pause
