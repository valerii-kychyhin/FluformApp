namespace FluformApp.Models.Attempt;

public class AttemptResource
{
    public string Id { get; set; } = string.Empty;
    public List<AttemptStep> Steps { get; set; } = new();
}