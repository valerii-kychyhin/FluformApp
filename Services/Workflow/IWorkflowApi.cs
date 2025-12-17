using Refit;
using FluformApp.Models.Workflow;
using System.Threading.Tasks;

namespace FluformApp.Services.Workflow;

public interface IWorkflowApi
{
    // User workflow
    [Get("/user/workflows")]
    Task<WorkflowCollection> GetAllUserAsync([Query] int current = 1, [Query] int pageSize = 10, [Query] string? sort = "-created_at");

    [Get("/user/workflows/{id}")]
    Task<WorkflowResource> GetUserAsync(string id);

    [Post("/user/workflows")]
    Task<WorkflowResource> CreateUserAsync([Body] object request);

    [Put("/user/workflows/{id}")]
    Task<WorkflowResource> UpdateUserAsync(string id, [Body] object request);

    [Delete("/user/workflows/{id}")]
    Task DeleteUserAsync(string id);

    // Client workflow
    [Get("/client/profile/workflows")]
    Task<WorkflowCollection> GetAllClientAsync([Query] int per_page = 10, [Query] string? sort = "-created_at");
}