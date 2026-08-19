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

        print("--------------------------------------------------")
        print("[SUCCESS] Chroniq Screensaver Successfully Built!")
        print(f"File Size: {exe_file.stat().st_size / 1024:.1f} KB")
        print(f"Official Screensaver File (.scr): {scr_file}")
        print(f"Standalone Executable: {exe_file}")
        print("--------------------------------------------------")
        print("Tips:")
        print("1. File siap pakai ada di folder 'dist/'")
        print("2. Klik kanan file 'dist/Chroniq.scr' lalu pilih 'Install'")
        print("--------------------------------------------------")
    else:
        print("[ERROR] Native compilation failed!")
        sys.exit(result.returncode)


if __name__ == "__main__":
    build()
