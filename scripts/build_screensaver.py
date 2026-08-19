"""
Automated Build Pipeline for Analog Clock Screensaver.
Compiles the C# Native Windows Screensaver into dist/AnalogClock.scr and dist/AnalogClock.exe.
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

    cs_source = root_dir / "src" / "native" / "NativeScreensaver.cs"
    csc_path = Path(r"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe")

    print("==================================================")
    print("  BUILDING NATIVE WINDOWS ANALOG SCREENSAVER (.SCR)")
    print("==================================================")

    if not cs_source.exists():
        print(f"[ERROR] Source file not found: {cs_source}")
        sys.exit(1)

    if not csc_path.exists():
        print(f"[ERROR] Native compiler not found: {csc_path}")
        sys.exit(1)

    exe_file = dist_dir / "AnalogClock.exe"
    scr_file = dist_dir / "AnalogClock.scr"

    cmd = [
        str(csc_path),
        "/target:winexe",
        f"/out:{exe_file}",
        "/r:System.Windows.Forms.dll",
        "/r:System.Drawing.dll",
        "/optimize+",
        str(cs_source),
    ]

    print(f"Compiling native binary: {' '.join(cmd)}")
    result = subprocess.run(cmd)

    if result.returncode == 0 and exe_file.exists():
        try:
            subprocess.run(["taskkill", "/f", "/im", "AnalogClock.scr"], capture_output=True)
            subprocess.run(["taskkill", "/f", "/im", "AnalogClock.exe"], capture_output=True)
        except Exception:
            pass

        shutil.copyfile(exe_file, scr_file)
        print("--------------------------------------------------")
        print("[SUCCESS] Native Ultra-Fast Screensaver Built!")
        print(f"File Size: {exe_file.stat().st_size / 1024:.1f} KB")
        print(f"Screensaver Executable: {exe_file}")
        print(f"Screensaver File (.scr): {scr_file}")
        print("--------------------------------------------------")
        print("Tips:")
        print("1. File siap pakai ada di folder 'dist/'")
        print("2. Klik kanan file 'dist/AnalogClock.scr' lalu pilih 'Install'")
        print("--------------------------------------------------")
    else:
        print("[ERROR] Native compilation failed!")
        sys.exit(result.returncode)


if __name__ == "__main__":
    build()
