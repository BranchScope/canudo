using canudo_news;

//await BotApi.Test();
var coms = await Scraper.GetComs();
var newComs = await Scraper.CheckForNewComs(coms);
foreach (var com in newComs)
{
    var button = new Dictionary<string, string>
    {
        { "url", com.Key }, { "text", "\ud83d\udcceBalza sul sito" }
    };
    var keyboard = new Dictionary<string, List<List<Dictionary<string, string>>>>
    {
        {
            "inline_keyboard", [[button]]
        }
    };
    await BotApi.SendMessage(-1002077160270, $"\ud83d\udccd<b>{com.Value[0]}</b>\n<i>{com.Value[1]}</i>", keyboard);
}
