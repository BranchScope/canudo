// dio can
using RestSharp;

namespace canudo_news;

internal static class Umbrella
{
    private static readonly string? Token = Environment.GetEnvironmentVariable("TOKEN");

    private const string Resource = "https://api.telegram.org/bot";

    private static async Task Main()
    {
        Console.Write("Enter the method: ");
        var method = Console.ReadLine();

        var client = new RestClient((string)(Resource + Token));
        var request = new RestRequest(method, Method.Post);
        var response = await client.GetAsync(request);

        Console.WriteLine(response.Content);
    }
}