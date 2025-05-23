@echo off
setlocal

echo [+] Changing directory to WebToGIF...
pushd WebToGIF

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
dotnet publish -c Release -r linux-x64 --self-contained false -o ../release/linux64
if errorlevel 1 (
    echo [!] Build failed.
    cd ..
    echo [+] Returning to original directory...
    pause
    exit /b 1
)

echo [+] Returning to original directory...
cd ..

echo [✓] Build successful! Output is in: release\linux64
pause


