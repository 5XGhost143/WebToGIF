using PuppeteerSharp;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    class Config
    {
        public string Url { get; set; } = "https://example.com";
    }

    static async Task Main()
    {
        const string configDir = "config";
        const string configFile = "config/config.json";

        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
            var config = new Config();
            var defaultJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configFile, defaultJson);

            Console.WriteLine($"Config folder and default config.json created at '{configFile}'.");
            Console.WriteLine("Please edit the config.json to set the URL you want to screenshot.");
            Console.WriteLine("The program will exit in 5 seconds...");
            await Task.Delay(5000);
            return; 
        }

        if (!File.Exists(configFile))
        {
            var config = new Config();
            var defaultJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configFile, defaultJson);
            Console.WriteLine($"Default config.json created at '{configFile}'.");
            Console.WriteLine("Please edit the config.json to set the URL you want to screenshot.");
            Console.WriteLine("The program will exit in 5 seconds...");
            await Task.Delay(5000);
            return; 
        }

        Console.WriteLine("[+] Loading config...");
        var json = await File.ReadAllTextAsync(configFile);
        var configLoaded = JsonSerializer.Deserialize<Config>(json) ?? new Config();
        Console.WriteLine($"[i] Loaded URL from config: {configLoaded.Url}");

        Console.WriteLine("[+] Starting BrowserFetcher to download Chromium (if needed)...");
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        Console.WriteLine("[+] Launching headless browser...");
        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

        Console.WriteLine("[+] Opening new page...");
        using var page = await browser.NewPageAsync();

        Console.WriteLine("[+] Setting viewport to 680x240...");
        await page.SetViewportAsync(new ViewPortOptions { Width = 680, Height = 240 });

        Console.WriteLine($"[+] Navigating to {configLoaded.Url} ...");
        await page.GoToAsync(configLoaded.Url);

        Console.WriteLine("[+] Waiting 3 seconds for page to load...");
        await Task.Delay(3000);

        var frames = new List<byte[]>();
        Console.WriteLine("[+] Starting to capture frames for GIF...");

        for (int i = 0; i < 25; i++)
        {
            var screenshot = await page.ScreenshotDataAsync();
            frames.Add(screenshot);
            await Task.Delay(80);
        }

        Console.WriteLine("[+] Closing browser...");
        await browser.CloseAsync();

        Console.WriteLine("[+] Creating GIF from captured frames...");
        using var gif = new MagickImageCollection();

        foreach (var imgData in frames)
        {
            var image = new MagickImage(imgData);
            image.AnimationDelay = 9;
            gif.Add(image);
        }

        gif[0].AnimationIterations = 0; 
        gif.Optimize();

        var outputFile = "output.gif";
        gif.Write(outputFile);
        Console.WriteLine($"✅ Done! GIF saved as {outputFile}");
    }
}
