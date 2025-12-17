namespace FluformApp.Models.Placeholder;

public class PlaceholderResource
{
    public string Id { get; set; } = string.Empty;
    public string? Parent_Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<PlaceholderResource> Children { get; set; } = new();

    public PlaceholderAudit Created { get; set; } = new();
    public PlaceholderAudit Updated { get; set; } = new();
}