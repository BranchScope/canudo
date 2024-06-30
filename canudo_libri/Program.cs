using canudo_libri;
using canudo_libri.Database;
using canudo_libri.TelegramApi;

var db = new Database();
var bot = new BotApi();

var CHANNEL = Convert.ToInt64(Environment.GetEnvironmentVariable("CHANNEL"));

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
        
        var user = await Database.GetUser(db, update.Message.From.Id);
        InlineKeyboard? keyboard;
        switch (update.Message.Text)
        {
            case "/start":
                keyboard = new InlineKeyboard(
                    [
                        [
                            new Button("Crea Annuncio", "write_ad")
                        ],
                        [
                            new Button("Codice Sorgente", url: "https://github.com/BranchScope/canudo")
                        ]
                    ]
                );
                await BotApi.SendMessage(update.Message.From.Id, "miao", keyboard: keyboard);
                await Database.SetStatus(db, update.Message.From.Id, "");
                break;
            
            // admin section
            case "/subs":
                Console.WriteLine(user.FirstName);
                if (user.Rank == 10)
                {
                    var subCount = await Database.GetSubs(db);
                    await BotApi.SendMessage(update.Message.From.Id, $"diocans: {subCount}");
                }
                break;
            
            default:
                if (user.Status == "set_bookname")
                {
                    var ad_id = Convert.ToInt32(user.Status.Split(":")[1].Split("/")[0]);
                    await Database.UpdateAdvertisement(db, ad_id, "book", update.Message.Text);
                    await Database.SetStatus(db, update.Message.From.Id, $"ad:{ad_id}");
                    List<List<Button>> buttons = [];
                    for (int i = 1; i <= 5; i++)
                    {
                        var button = new Button($"{i} 🔘", $"trigger_year:{i}");
                        buttons.Add([button]);
                    }
                    buttons.Add([new Button("Fatto", "years_ok")]);
                    buttons.Add([new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")]);
                    keyboard = new InlineKeyboard(buttons);
                    await BotApi.SendMessage(update.Message.From.Id, "Seleziona:", keyboard: keyboard, update.Message.MessageId);
                }
                break;
        }
    }
    
    if (update.CallbackQuery != null)
    {
        var user = await Database.GetUser(db, update.CallbackQuery.From.Id);
        InlineKeyboard? keyboard;
        int ad_id;
        switch (update.CallbackQuery.Data)
        {
            case "write_ad":
                ad_id = await Database.CreateAdvertisement(db, update.CallbackQuery.From.Id);
                await Database.SetStatus(db, update.CallbackQuery.From.Id, $"ad:{ad_id}");
                var subjects = await Database.GetSubjects(db);
                List<List<Button>> buttons = [];
                foreach (var subject in subjects)
                {
                    var button = new Button(subject.Name, $"set_ad_subject:{subject.CodeName}");
                    buttons.Add([button]);
                }
                buttons.Add([new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")]);
                keyboard = new InlineKeyboard(buttons);
                await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "Seleziona:", keyboard: keyboard);
                await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                break;
            case { } when update.CallbackQuery.Data.StartsWith("cancel_ad"):
                var p = update.CallbackQuery.Data.Split("/");
                ad_id = Convert.ToInt32(p[1].Split(":")[1]);
                var ad = await Database.GetAdvertisementById(db, ad_id);
                if (ad.FromUser == update.CallbackQuery.From.Id) //security check ('nse sa mai)
                {
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, "");
                    await Database.DeleteAdvertisement(db, ad_id);
                    if (update.CallbackQuery.Data.Contains("message_id"))
                    {
                        await BotApi.DeleteMessage(CHANNEL, Convert.ToInt32(p[2].Split(":")[1]));
                    }
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button("Crea Annuncio", "write_ad")
                            ],
                            [
                                new Button("Codice Sorgente", url: "https://github.com/BranchScope/canudo")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "Cancelado.", keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                }
                break;
        }

        if (user.Status.Contains("ad"))
        {
            ad_id = Convert.ToInt32(user.Status.Split(":")[1].Split("/")[0]);
            DBAdvertisement? ad;
            switch (update.CallbackQuery.Data)
            {
                case { } when update.CallbackQuery.Data.StartsWith("set_ad_subject"):
                    var subject = update.CallbackQuery.Data.Split(":")[1];
                    await Database.UpdateAdvertisement(db, ad_id, "subject", subject);
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, $"{user.Status}/set_bookname");
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "Seleziona:", keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case { } when update.CallbackQuery.Data.StartsWith("trigger_year"):
                    ad = await Database.GetAdvertisementById(db, ad_id);
                    List<int> years = [..ad.Years];
                    var year = Convert.ToInt32(update.CallbackQuery.Data.Split(":")[1]);
                    if (!years.Remove(year))
                    {
                        years.Add(year);
                    }
                    List<List<Button>> buttons = [];
                    for (int i = 1; i <= 5; i++)
                    {
                        var button = new Button($"{i} {(years.Contains(i) ? "🟢" : "🔘")}", $"trigger_year:{i}");
                        buttons.Add([button]);
                    }
                    buttons.Add([new Button("Fatto", "years_ok")]);
                    buttons.Add([new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")]);
                    keyboard = new InlineKeyboard(buttons);
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "Seleziona:", keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case "years_ok":
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, $"{user.Status}/set_bookcode");
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button("Skip", "skip_set_bookcode")
                            ],
                            [
                                new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "Bookcode:", keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case "skip_set_bookcode":
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, $"{user.Status}/set_contacts");
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, "Contacts:", keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case "send_ad":
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, "");
                    ad = await Database.GetAdvertisementById(db, ad_id);
                    List<string> contacts = [];
                    //todo parsing of the contacts
                    //todo send advertisement in the channel
                    break;
            }
        }
    }
}