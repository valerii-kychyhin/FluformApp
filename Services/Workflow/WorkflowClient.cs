using FluformApp.Models.Workflow;
using System.Threading.Tasks;

namespace FluformApp.Services.Workflow;

public class WorkflowClient
{
    private readonly IWorkflowApi _api;

    public WorkflowClient(IWorkflowApi api)
    {
        _api = api;
    }

    // User workflow
    public async Task<WorkflowCollection> GetAllUserAsync() => await _api.GetAllUserAsync();

    public async Task<WorkflowResource> GetUserAsync(string id) => await _api.GetUserAsync(id);

    public async Task<WorkflowResource> CreateUserAsync(object request) => await _api.CreateUserAsync(request);

    public async Task<WorkflowResource> UpdateUserAsync(string id, object request) => await _api.UpdateUserAsync(id, request);

    public async Task DeleteUserAsync(string id) => await _api.DeleteUserAsync(id);

    // Client workflow
    public async Task<WorkflowCollection> GetAllClientAsync() => await _api.GetAllClientAsync();
}