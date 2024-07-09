using System.Text.Json;
using PhoneNumbers;
using canudo_libri;
using canudo_libri.Database;
using canudo_libri.TelegramApi;

var db = new Database();
var bot = new BotApi();
var phoneNumberUtil = PhoneNumberUtil.GetInstance();

var CHANNEL = Convert.ToInt64(Environment.GetEnvironmentVariable("CHANNEL"));
var strings = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("strings.json"));

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
            // soy el fuego que arde tu piel
            case "/start":
                keyboard = new InlineKeyboard(
                    [
                        [
                            new Button(strings["write_ad_btn"], "write_ad")
                        ],
                        [
                            new Button(strings["source_code_btn"], url: "https://github.com/BranchScope/canudo")
                        ]
                    ]
                );
                await BotApi.SendMessage(update.Message.From.Id, strings["start"], keyboard: keyboard);
                await Database.SetStatus(db, update.Message.From.Id, "");
                break;
            
            // admin section
            case "/subs":
                if (user.Rank == 10)
                {
                    var subCount = await Database.GetSubs(db);
                    await BotApi.SendMessage(update.Message.From.Id, $"Total count: {subCount}");
                }
                break;
            case { } when update.Message.Text.StartsWith("/addsubject"):
                if (user.Rank == 10)
                    {
                    try
                    {
                        var data = update.Message.Text.Replace("/addsubject ", "");
                        var name = data.Split(":")[0];
                        var codeName = data.Split(":")[1];
                        await Database.AddSubject(db, codeName, name);
                        await BotApi.SendMessage(update.Message.From.Id, "Subject added.");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        await BotApi.SendMessage(update.Message.From.Id, "USAGE: /addsubject NAME:CODENAME");
                    }
                }
                break;
            case { } when update.Message.Text.StartsWith("/delsubject"):
                if (user.Rank == 10)
                    {
                    try
                    {
                        var codeName = update.Message.Text.Split(" ")[1];
                        await Database.DeleteSubject(db, codeName);
                        await BotApi.SendMessage(update.Message.From.Id, "Subject deleted.");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        await BotApi.SendMessage(update.Message.From.Id, "USAGE: /delsubject CODENAME");
                    }
                }
                break;
            
            // general conditions
            default:
                if (user.Status.Contains("set_bookname"))
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
                    buttons.Add([new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")]);
                    keyboard = new InlineKeyboard(buttons);
                    await BotApi.SendMessage(update.Message.From.Id, strings["ad_years_selection"], keyboard: keyboard, update.Message.MessageId);
                }
                else if (user.Status.Contains("set_contacts"))
                {
                    var ad_id = Convert.ToInt32(user.Status.Split(":")[1].Split("/")[0]);
                    try
                    {
                        string[] contacts = update.Message.Text.Split(
                            ["\r\n", "\r", "\n"],
                            StringSplitOptions.None
                        );
                        List<List<string>> c_array = [];
                        foreach (var contact in contacts)
                        {
                            var social = contact.Split(" - ")[0];
                            var url = contact.Split(" - ")[1];
                            
                            // you know.
                            if (url.Contains("@") && social.ToLower() == "telegram")
                            {
                                social = url;
                                url = "";
                            }

                            // famm ste' tranguill pur a me, famm (cit. Checco Zalone)
                            // humans are basically low-ended IQ animals
                            if (url.Contains("@") && social.ToLower() == "instagram")
                            {
                                url = $"https://instagram.com/{url.Replace("@", "")}";
                            }

                            // yeah I should use IsValidNumber method, but I can't Parse the number in this static context
                            if (phoneNumberUtil.IsPossibleNumber(url, "IT"))
                            {
                                social = url;
                                url = $"tel:{url}";
                            }
                            
                            // extra bacon check because I don't believe in humans like Marco Mengoni
                            if (social.ToLower() == "telefono" || social.ToLower() == "numero")
                            {
                                social = url;
                                url = $"tel:{url}";
                            }

                            c_array.Add([social, url]);
                        }
                        
                        string[,] c_multid_array = ConvertToMultidimensionalArray(c_array);

                        await Database.UpdateAdvertisement(db, ad_id, "contacts", c_multid_array);
                        await Database.SetStatus(db, ad_id, $"ad:{ad_id}");
                        keyboard = new InlineKeyboard(
                            [
                                [
                                    new Button(strings["send_ad_btn"], "send_ad")
                                ],
                                [
                                    new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")
                                ]
                            ]
                        );
                        await BotApi.SendMessage(update.Message.From.Id, strings["done_send"], keyboard: keyboard, update.Message.MessageId);
                    }
                    catch (Exception exception)
                    {
                        // hard coding a debug print because I'm lazy
                        // todo: debug system done "accom s dev" (translated: "how it should be done")
                        Console.WriteLine(exception);
                        await BotApi.SendMessage(update.Message.From.Id, strings["ad_contacts_imposteiscion_error"], keyboard: null, update.Message.MessageId);
                    }
                }
                else if (user.Status.Contains("set_bookcode"))
                {
                    var ad_id = Convert.ToInt32(user.Status.Split(":")[1].Split("/")[0]);
                    await Database.UpdateAdvertisement(db, ad_id, "book_code", update.Message.Text);
                    await Database.SetStatus(db, update.Message.From.Id, $"{user.Status}/set_contacts");
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button("Cancella", $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.SendMessage(update.Message.From.Id, strings["ad_contacts_imposteiscion"], keyboard: keyboard, update.Message.MessageId);
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
                buttons.Add([new Button(strings["cancel_ad_btn"], $"cancel_ad/ad_id:{ad_id}")]);
                keyboard = new InlineKeyboard(buttons);
                await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["ad_subject_selection"], keyboard: keyboard);
                await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                break;
            case { } when update.CallbackQuery.Data.StartsWith("cancel_ad"):
                await Database.SetStatus(db, update.CallbackQuery.From.Id, "");
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
                                new Button(strings["write_ad_btn"], "write_ad")
                            ],
                            [
                                new Button(strings["source_code_btn"], url: "https://github.com/BranchScope/canudo")
                            ]
                        ]
                    );
                    var toUnpin = await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["start"], keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    await BotApi.UnpinChatMessage(update.CallbackQuery.From.Id, toUnpin.Result.MessageId);
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
                                new Button(strings["cancel_ad_btn"], $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["ad_bookname_imposteiscion"], keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case { } when update.CallbackQuery.Data.StartsWith("trigger_year"):
                    ad = await Database.GetAdvertisementById(db, ad_id);
                    List<int> years = ad.Years ?? [];
                    var year = Convert.ToInt32(update.CallbackQuery.Data.Split(":")[1]);
                    if (!years.Remove(year))
                    {
                        years.Add(year);
                    }
                    await Database.UpdateAdvertisement(db, ad_id, "years", years);
                    List<List<Button>> buttons = [];
                    for (int i = 1; i <= 5; i++)
                    {
                        var button = new Button($"{i} {(years.Contains(i) ? "🟢" : "🔘")}", $"trigger_year:{i}");
                        buttons.Add([button]);
                    }
                    buttons.Add([new Button(strings["done"], "years_ok")]);
                    buttons.Add([new Button(strings["cancel_ad_btn"], $"cancel_ad/ad_id:{ad_id}")]);
                    keyboard = new InlineKeyboard(buttons);
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["ad_years_selection"], keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case "years_ok":
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, $"{user.Status}/set_bookcode");
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button(strings["skip_btn"], "skip_set_bookcode")
                            ],
                            [
                                new Button(strings["cancel_ad_btn"], $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["ad_bookcode_imposteiscion"], keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case "skip_set_bookcode":
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, $"{user.Status}/set_contacts");
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button(strings["cancel_ad_btn"], $"cancel_ad/ad_id:{ad_id}")
                            ]
                        ]
                    );
                    await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["ad_contacts_imposteiscion"], keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    break;
                case "send_ad":
                    await Database.SetStatus(db, update.CallbackQuery.From.Id, "");
                    ad = await Database.GetAdvertisementById(db, ad_id);
                    List<string> contacts = [];
                    contacts.AddRange(ad.Contacts.Select(contact => $"<a href='{contact[1]}'>{contact[0]}</a>"));
                    var txt = strings["ad"]
                        .Replace("{BOOK_NAME}", ad.Book)
                        .Replace("{BOOK_CODE_STR}", (!string.IsNullOrEmpty(ad.BookCode) ? $"<b>Codice libro:</b> <code>{ad.BookCode}</code>\n" : ""))
                        .Replace("{SUBJECT}", ad.Subject)
                        .Replace("{YEARS}", string.Join(", ", ad.Years.Select(y => $"{y}\u00b0")))
                        .Replace("{CONTACTS}", string.Join(", ", ad.Contacts.Select(contact => $"<a href=\"{contact[1]}\">{contact[0]}</a>")))
                        .Replace("{FROM_USER_ID}", ad.FromUser.ToString())
                        .Replace("{FROM_USER_NAME}", (await Database.GetUser(db, ad.FromUser)).FirstName);
                    var msg = await BotApi.SendMessage(CHANNEL, txt);
                    keyboard = new InlineKeyboard(
                        [
                            [
                                new Button(strings["url_btn"], url: $"https://t.me/c/{Math.Abs(CHANNEL + 1e12)}/{msg.Result.MessageId}")
                            ],
                            [
                                new Button(strings["cancel_ad_btn"], $"cancel_ad/ad_id:{ad_id}/message_id:{msg.Result.MessageId}")
                            ]
                        ]
                    );
                    var toPin = await BotApi.EditMessageText(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, strings["congrats"], keyboard: keyboard);
                    await BotApi.AnswerCallbackQuery(update.CallbackQuery.Id);
                    await BotApi.PinChatMessage(update.CallbackQuery.From.Id, toPin.Result.MessageId);
                    break;
            }
        }
    }
}

// a little util for a database inconvenience
string[,] ConvertToMultidimensionalArray(List<List<string>> jaggedList)
{
    int outerLength = jaggedList.Count;
    int innerLength = jaggedList.Max(innerList => innerList.Count);
    string[,] multidimensionalArray = new string[outerLength, innerLength];

    for (int i = 0; i < outerLength; i++)
    {
        for (int j = 0; j < jaggedList[i].Count; j++)
        {
            multidimensionalArray[i, j] = jaggedList[i][j];
        }
    }

    return multidimensionalArray;
}
