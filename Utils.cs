using System.Collections;
using Microsoft.Playwright;

namespace canudo_news;

public static class Scraper
{
    private const string Resource = "https://canudo.edu.it/index.php";

    public static async Task<dynamic?> Wave()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Webkit.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(Resource);
        var firstJobElement = page.Locator("div[class='items-leading clearfix']").Locator("a[href]").First;
        var othersJobsElements = page.Locator("div[class='item column-1 span12']").Locator("a[href][itemprop='url']");
        Console.WriteLine(Resource + await firstJobElement.GetAttributeAsync("href"));
        foreach (var element in await othersJobsElements.ElementHandlesAsync())
        {
            Console.WriteLine(Resource + await element.GetAttributeAsync("href"));
        }
        return null;
    }
}