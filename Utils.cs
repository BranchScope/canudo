using Microsoft.Playwright;

namespace canudo_news;

public static class Scraper
{
    private const string Resource = "https://canudo.edu.it/index.php";

    public static async Task<Dictionary<string, List<string>>> GetComs()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Webkit.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(Resource);
        Dictionary<string, List<string>> elements = [];
        var firstJobElement = page.Locator("div[class='items-leading clearfix']").First;
        var firstJobElementLink = await firstJobElement.Locator("a[href]").First.GetAttributeAsync("href");
        var firstJobElementTitle = (await firstJobElement.Locator("a[href]").First.TextContentAsync())?.Trim();
        var firstJobElementContent = await firstJobElement.Locator("p").First.TextContentAsync();
        elements.Add(Resource + firstJobElementLink!, [firstJobElementTitle, firstJobElementContent ?? string.Empty]);
        var othersJobsElements = page.Locator("div[class='item column-1 span12']");
        foreach (var element in await othersJobsElements.AllAsync())
        {
            var elementLink = string.Empty;
            var elementTitle = string.Empty;
            var elementLinkDerivate = element.Locator("a[href][itemprop='url']");
            foreach (var elementDerivate in await elementLinkDerivate.ElementHandlesAsync())
            {
                elementLink = Resource + await elementDerivate.GetAttributeAsync("href");
                elementTitle = (await elementDerivate.TextContentAsync())?.Trim();
            }
            var elementContent = await element.Locator("p[style='text-align: justify;']").First.TextContentAsync();
            elements.Add(elementLink, [elementTitle, elementContent ?? string.Empty]);
        }
        return elements;
    }

    public static async Task<Dictionary<string, List<string>>> CheckForNewComs(Dictionary<string, List<string>> coms)
    {
        List<string> links = [..(coms).Keys];
        List<string> txtContent = [..await File.ReadAllLinesAsync("last.txt")];
        var newComs = links.Except(txtContent);
        var enumerable = newComs.ToList();
        await File.AppendAllLinesAsync("last.txt", enumerable);
        var cleanComsListWaiter = links.Intersect(txtContent);
        foreach (var com in cleanComsListWaiter)
        {
            coms.Remove(com);
        }
        return coms;
    }
    
}