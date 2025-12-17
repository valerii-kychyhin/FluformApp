namespace FluformApp.Models.Workflow;

public class WorkflowStep
{
    public string Id { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int IsLocked { get; set; }
    public int IsClientFill { get; set; }
    public List<object?> Signatures { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}