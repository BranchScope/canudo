using System.Text.Json;
using RestSharp;

namespace canudo_news;

public static class BotApi
{
    private const string Resource = "https://api.telegram.org/bot";
    private static readonly string? Token = Environment.GetEnvironmentVariable("TOKEN");

    private static readonly RestClientOptions Options = new RestClientOptions(Resource + Token) {
        ThrowOnAnyError = false,
        ThrowOnDeserializationError = false
    };
    private static readonly RestClient Client = new RestClient(options: Options);

    public static async Task<Response> GetMe()
    {
        var request = new RestRequest("getMe", Method.Post);
        var response = await Client.ExecutePostAsync(request);
        return JsonSerializer.Deserialize<Response>(response.Content ?? throw new MissingFieldException()) ?? throw new Exception("wtf!?");
    }

    public static async Task<Response> SendMessage(long chatId, string text, Dictionary<string, List<List<Dictionary<string, string>>>>? keyboard = null)
    {
        var request = new RestRequest("sendMessage", Method.Post);
        var param = new
        {
            chat_id = chatId, 
            text, 
            reply_markup = JsonSerializer.Serialize(keyboard),
            parse_mode = "HTML"
        };
        Console.WriteLine(ObjectDumper.Dump(param));
        request.AddJsonBody(param);
        var response = await Client.ExecutePostAsync(request);
        return JsonSerializer.Deserialize<Response>(response.Content ?? throw new MissingFieldException()) ?? throw new Exception("wtf!?");
    }

    public static async Task Test()
    {
        var button = new Dictionary<string, string>
        {
            { "url", "https://example.com" }, { "text", "test" }
        };
        var keyboard = new Dictionary<string, List<List<Dictionary<string, string>>>>
        {
            {
                "inline_keyboard", [[button]]
            }
        };
        var test = await SendMessage(1876496621, "Sent with C#, but can't see more sharp as said in the docs.", keyboard);
        //var test = await GetMe();
        Console.WriteLine($"The output: {(test.Result?.MessageId != null ? test.Result.MessageId : test.Description)}");
    }
    
}