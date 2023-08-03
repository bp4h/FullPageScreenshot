using PuppeteerSharp;
using System;
using System.IO;
using System.Threading.Tasks;

public class PuppeteerService
{
    private readonly string _screenshotsFolderPath;

    public PuppeteerService(string screenshotsFolderPath)
    {
        _screenshotsFolderPath = screenshotsFolderPath;
    }

    public async Task<(byte[] fullPageScreenshot, byte[] smallScreenshot)> TakeScreenshots(string url)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

        var launchOptions = new LaunchOptions
        {
            Headless = false
        };

        using var browser = await Puppeteer.LaunchAsync(launchOptions);
        using var page = await browser.NewPageAsync();

        await page.GoToAsync(url, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded } });

        await Task.Delay(1000);

        await page.EvaluateFunctionAsync(@"async () => {
        await new Promise((resolve) => {
            const distance = 500;
            let totalHeight = 0;
            const timer = setInterval(() => {
                const scrollHeight = document.body.scrollHeight;
                window.scrollBy(0, distance);
                totalHeight += distance;
                if (totalHeight >= scrollHeight) {
                    clearInterval(timer);
                    resolve();
                    }
                }, 1000);
            });
        }");

        

        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 1920,
            Height = 1080
        });

        var fullPageScreenshotOptions = new ScreenshotOptions
        {
            Type = ScreenshotType.Png,
            FullPage = true
        };
        var fullPageScreenshotData = await page.ScreenshotDataAsync(fullPageScreenshotOptions);

        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 450,
            Height = 800
        });

        var smallScreenshotOptions = new ScreenshotOptions
        {
            Type = ScreenshotType.Png,
            FullPage = true
        };
        var smallScreenshotData = await page.ScreenshotDataAsync(smallScreenshotOptions);

        var fileNameFull = $"full_{DateTime.Now:yyyyMMddHHmmssfff}.png";
        var filePathFull = Path.Combine(_screenshotsFolderPath, fileNameFull);
        File.WriteAllBytes(filePathFull, fullPageScreenshotData);

        var fileNameSmall = $"small_{DateTime.Now:yyyyMMddHHmmssfff}.png";
        var filePathSmall = Path.Combine(_screenshotsFolderPath, fileNameSmall);
        File.WriteAllBytes(filePathSmall, smallScreenshotData);

        return (fullPageScreenshotData, smallScreenshotData);
    }    
}
