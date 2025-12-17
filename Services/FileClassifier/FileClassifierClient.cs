using FluformApp.Models.FileClassifier;

namespace FluformApp.Services.FileClassifier;

public class FileClassifierClient
{
    private readonly IFileClassifierApi _api;

    public FileClassifierClient(IFileClassifierApi api)
    {
        _api = api;
    }

    public Task<FileClassifierCollection> GetAllAsync() =>
        _api.GetAllAsync();

    public Task<FileClassifierResource> GetAsync(string id) =>
        _api.GetAsync(id);

    public Task<FileClassifierResource> CreateAsync(object body) =>
        _api.CreateAsync(body);

    public Task<FileClassifierResource> UpdateAsync(string id, object body) =>
        _api.UpdateAsync(id, body);

    public Task DeleteAsync(string id) =>
        _api.DeleteAsync(id);
}