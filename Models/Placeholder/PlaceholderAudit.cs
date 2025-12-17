namespace FluformApp.Models.Placeholder;

public class PlaceholderAudit
{
    public PlaceholderAuthor Author { get; set; } = new();
    public DateTime Created_At { get; set; }
}
