namespace FluformApp.Models.Attempt;

public class AttemptStep
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int IsLocked { get; set; }
    public int Position { get; set; }
}