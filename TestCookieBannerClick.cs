using Microsoft.Playwright;

namespace canudo_news;

public static class GeoPopup
{
    private const string Resource = "https://www.geopop.it/";

    public static async Task BannerClick()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Webkit.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(Resource);
        var popupDiv = page.Locator("div[class='gdpr-dlg-wrp']");
        var button = (await popupDiv.Locator("button[class='gdpr-btn blue']").AllAsync())[1];
        await button.ClickAsync();
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshot.png",
        });
    }
}
