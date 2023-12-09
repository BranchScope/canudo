using System.Text.Json.Serialization;

namespace canudo_news;

public record Response
{
    [JsonPropertyName("ok")] public bool Ok { get; init; }
    [JsonPropertyName("result")] public Result? Result { get; init; }
}

public record Result
{
    [JsonPropertyName("message_id")] public int? MessageId { get; init; }
    [JsonPropertyName("from")] public From? From { get; init; }
    [JsonPropertyName("chat")] public Chat? Chat { get; init; }
    [JsonPropertyName("date")] public int? Date { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
}

public record From
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("is_bot")] public bool IsBot { get; init; }
    [JsonPropertyName("first_name")] public string FirstName { get; init; }
    [JsonPropertyName("last_name")] public string? LastName { get; init; }
    [JsonPropertyName("username")] public string? Username { get; init; }
}

public record Chat
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("first_name")] public string FirstName { get; init; }
    [JsonPropertyName("last_name")] public string? LastName { get; init; }
    [JsonPropertyName("username")] public string? Username { get; init; }
    [JsonPropertyName("type")] public string Type { get; init; }
}