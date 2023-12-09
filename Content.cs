namespace canudo_news;

public abstract class Content(string com, string? title, string link)
{
    public required string Com = com;
    public required string? Title = title;
    public required string Link = link;
}