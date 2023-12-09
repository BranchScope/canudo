using System.Net;
using RestSharp;

namespace canudo_news;

public static class BotApi
{
    private const string Resource = "https://api.telegram.org/bot";
    private static readonly string? Token = Environment.GetEnvironmentVariable("TOKEN");

    private static async Task<dynamic?> GetMe()
    {
        var client = new RestClient(Resource + Token);
        var request = new RestRequest("getMe", Method.Post);
        var response = await client.GetAsync(request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return response.Content;
        }
        else
        {
            Console.WriteLine("Oh, it seems like something did go wrong, pls check how many pilngul you have before debugging and then defenestrate yourself from the attic of Burshcsdh-Kalifa.");
            return false;
        }
    }

    private static async Task<dynamic?> SendMessage(int chatId, string text)
    {
        var client = new RestClient(Resource + Token);
        var request = new RestRequest("sendMessage", Method.Post);
        var param = new { chat_id = chatId, text = text };
        Console.WriteLine(ObjectDumper.Dump(param));
        request.AddJsonBody(param);
        var response = await client.GetAsync(request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return response.Content;
        }
        else
        {
            Console.WriteLine("Oh, it seems like something did go wrong, pls check how many pilngul you have before debugging and then defenestrate yourself from the attic of Burshcsdh-Kalifa.");
            return false;
        }
    }

    public static async Task Test()
    {
        var test = await SendMessage(1876496621, "Sent with C#, but can't see more sharp as said in the docs.");
        Console.WriteLine($"The output: {test}");
    }
    
}