using FluformApp.Models.Attempt;

namespace FluformApp.Services.Attempt;

public class ClientAttemptClient
{
    private readonly IClientAttemptApi _api;

    public ClientAttemptClient(IClientAttemptApi api)
    {
        _api = api;
    }

    public Task<AttemptCollection> GetAllAsync() =>
        _api.GetAllAsync();

    public Task<AttemptResource> CreateAsync(string workflowId) =>
        _api.CreateAsync(new CreateAttemptRequest
        {
            Workflow_Id = workflowId
        });

    public Task<AttemptResource> GetAsync(string id) =>
        _api.GetAsync(id);

    public Task DeleteAsync(string id) =>
        _api.DeleteAsync(id);
}