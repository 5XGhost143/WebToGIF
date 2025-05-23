@echo off
setlocal

echo [+] Adding package 'PuppeteerSharp'...
dotnet add package PuppeteerSharp
if errorlevel 1 (
    echo [!] Failed to add PuppeteerSharp.
    pause
    exit /b 1
)

echo [+] Adding package 'Magick.NET-Q8-AnyCPU'...
dotnet add package Magick.NET-Q8-AnyCPU
if errorlevel 1 (
    echo [!] Failed to add Magick.NET.
    pause
    exit /b 1
)

echo [+] Building project in Release mode...
dotnet build -c Release -o release/win64
if errorlevel 1 (
    echo [!] Build failed.
    pause
    exit /b 1
)

echo [✓] Build successful! Output is in: release\win64
pause
