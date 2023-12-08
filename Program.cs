// dio can
using RestSharp;

namespace canudo_news;
internal static class Umbrella
{
    private static readonly string? Token = Environment.GetEnvironmentVariable("TOKEN");

    private const string Resource = "https://api.telegram.org/bot";

    private static readonly List<string?> AllowedMethods = new List<string?>() { "getMe", "sendMessage" };

    private static async Task Main()
    {
        Console.Write("Enter the method: ");
        var method = Console.ReadLine();

        if (!AllowedMethods.Contains(method))
        {
            throw new MissingMethodException("This method has not been implemented yet.");
        }
        
        var client = new RestClient(Resource + Token);
        var request = new RestRequest(method, Method.Post);
        var response = await client.GetAsync(request);

        Console.WriteLine(response.Content);
    }
}