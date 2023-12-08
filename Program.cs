// dio can

using RestSharp;

var token = System.Environment.GetEnvironmentVariable("TOKEN");

string resource = "https://api.telegram.org/bot";

Console.Write("Enter the method: ");
string? method = Console.ReadLine();

var client = new RestClient(resource + token);
var request = new RestRequest(method, Method.Post);
var response = await client.GetAsync(request);

Console.WriteLine(response.Content);