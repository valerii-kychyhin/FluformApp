using FluformApp.Models.Placeholder;

namespace FluformApp.Services.Placeholder;

public class PlaceholderClient
{
    private readonly IPlaceholderApi _api;

    public PlaceholderClient(IPlaceholderApi api)
    {
        _api = api;
    }

    public Task<PlaceholderCollection> GetAllAsync() =>
        _api.GetAllAsync();

    public Task<PlaceholderResource> GetAsync(string id) =>
        _api.GetAsync(id);

    public Task<PlaceholderResource> CreateAsync(object body) =>
        _api.CreateAsync(body);

    public Task<PlaceholderResource> UpdateAsync(string id, object body) =>
        _api.UpdateAsync(id, body);

    public Task DeleteAsync(string id) =>
        _api.DeleteAsync(id);
}