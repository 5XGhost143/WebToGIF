using PuppeteerSharp;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        using var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 680,
            Height = 240
        });

        await page.GoToAsync("https://example.com");
        await Task.Delay(3000); 

        var frames = new List<byte[]>();

        for (int i = 0; i < 25; i++) 
        {
            var screenshot = await page.ScreenshotDataAsync();
            frames.Add(screenshot);
            await Task.Delay(80);
        }

        await browser.CloseAsync();

        using var gif = new MagickImageCollection();
        foreach (var imgData in frames)
        {
            var image = new MagickImage(imgData);
            image.AnimationDelay = 8;
            gif.Add(image);
        }

        gif[0].AnimationIterations = 0;
        gif.Optimize();
        gif.Write("output.gif");

        Console.WriteLine("✅ Fertig! output.gif wurde erstellt.");
    }
}
