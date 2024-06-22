using canudo_libri;
using canudo_libri.Database;
using canudo_libri.TelegramApi;

var db = new Database();
var bot = new BotApi();

bot.UpdateReceived += OnUpdate;
var updateTask = bot.StartUpdatingAsync(db.Db);
Console.WriteLine("Bot is running, HALLELUJAH!");
await updateTask;

async void OnUpdate(object? sender, UpdateEventArgs e)
{
    var update = e.Update;
    var db = e.Database;
    
    if (update.Message != null)
    {
        var checkUser = await Database.CheckUser(db, update.Message.From.Id);
        if (checkUser == 0)
        {
            await Database.AddUser(db, update.Message.From);
        }

        InlineKeyboard? keyboard;
        switch (update.Message.Text)
        {
            case "/start":
            {
                keyboard = new InlineKeyboard(
                    [
                        [
                            new Button("test", "test")
                        ],
                        [
                            new Button("test", url: "https://example.com")
                        ]
                    ]
                );
                await BotApi.SendMessage(update.Message.From.Id, "miao", keyboard: keyboard);
                break;
            }
        }
    }
    
    if (update.CallbackQuery != null)
    {
        switch (update.CallbackQuery.Data)
        {
            case "test":
                await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "miao");
                await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                break;
        }
    }
}