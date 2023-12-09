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
        Console.WriteLine(Resource + await firstJobElement.GetAttributeAsync("href"));
        return null;
    }
}