namespace FluformApp.Models.Workflow;

public class WorkflowCollection
{
    public List<WorkflowResource> Items { get; set; } = new();
    public int Current { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}