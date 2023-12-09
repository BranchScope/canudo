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

    private static async Task<Response> GetMe()
    {
        var request = new RestRequest("getMe", Method.Post);
        var response = await Client.ExecutePostAsync(request);
        return JsonSerializer.Deserialize<Response>(response.Content ?? throw new MissingFieldException()) ?? throw new Exception("wtf!?");
    }

    private static async Task<Response> SendMessage(int chatId, string text)
    {
        var request = new RestRequest("sendMessage", Method.Post);
        var param = new { chat_id = chatId, text };
        Console.WriteLine(ObjectDumper.Dump(param));
        request.AddJsonBody(param);
        var response = await Client.ExecutePostAsync(request);
        return JsonSerializer.Deserialize<Response>(response.Content ?? throw new MissingFieldException()) ?? throw new Exception("wtf!?");
    }

    public static async Task Test()
    {
        var test = await SendMessage(1876496621, "Sent with C#, but can't see more sharp as said in the docs.");
        //var test = await GetMe();
        Console.WriteLine($"The output: {(test.Result?.MessageId != null ? test.Result.MessageId.ToString() : test.Description)}");
    }
    
}