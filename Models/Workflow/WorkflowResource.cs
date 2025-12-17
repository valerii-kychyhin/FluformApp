namespace FluformApp.Models.Workflow;

public class WorkflowResource
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ClientInfo Client { get; set; } = new();
    public List<WorkflowStep> Steps { get; set; } = new();
    public (AuthorInfo Author, DateTime CreatedAt) Created { get; set; }
    public (AuthorInfo Author, DateTime UpdatedAt) Updated { get; set; }
}