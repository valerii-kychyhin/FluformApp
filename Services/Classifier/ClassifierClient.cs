using FluformApp.Models.Classifier;

namespace FluformApp.Services.Classifier;

public class ClassifierClient
{
    private readonly IClassifierApi _api;

    public ClassifierClient(IClassifierApi api)
    {
        _api = api;
    }

    public async Task<ClassifierCollection> GetAllAsync() =>
        await _api.GetAllAsync();

    public async Task<ClassifierResource> GetAsync(string id) =>
        await _api.GetAsync(id);

    public async Task<ClassifierResource> CreateAsync(string name, string content)
    {
        var request = new ClassifierRequest { Name = name, Content = content };
        return await _api.CreateAsync(request);
    }

    public async Task<ClassifierResource> UpdateAsync(string id, string name, string content)
    {
        var request = new ClassifierRequest { Name = name, Content = content };
        return await _api.UpdateAsync(id, request);
    }

    public async Task DeleteAsync(string id) =>
        await _api.DeleteAsync(id);
}