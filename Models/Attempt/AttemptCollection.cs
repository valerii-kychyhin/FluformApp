namespace FluformApp.Models.Attempt;

public class AttemptCollection
{
    public List<AttemptResource> Items { get; set; } = new();
    public int Current { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}