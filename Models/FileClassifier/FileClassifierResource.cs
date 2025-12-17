namespace FluformApp.Models.FileClassifier;

public class FileClassifierResource
{
    public string Id { get; set; } = string.Empty;
    public List<string?> Name { get; set; } = new();
    public string Slug { get; set; } = string.Empty;
    public object Content { get; set; } = new();
}
