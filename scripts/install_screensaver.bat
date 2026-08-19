@echo off
title Memasang Screensaver Jam Analog ke Windows
echo ========================================================
echo   MEMASANG NATIVE ANALOG CLOCK SCREENSAVER KE WINDOWS
echo ========================================================
echo.
echo Membangun biner screensaver terbaru...
python "%~dp0build_screensaver.py"
echo.
echo Menginstal ke Windows...
start "" "%~dp0..\dist\AnalogClock.scr" /s
echo.
echo Selesai! Anda juga dapat klik kanan 'dist\AnalogClock.scr' lalu pilih 'Install'.
pause
