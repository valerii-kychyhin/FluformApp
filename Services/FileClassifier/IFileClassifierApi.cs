using Refit;
using FluformApp.Models.FileClassifier;

namespace FluformApp.Services.FileClassifier;

public interface IFileClassifierApi
{
    [Get("/user/file-classifiers")]
    Task<FileClassifierCollection> GetAllAsync();

    [Get("/user/file-classifiers/{id}")]
    Task<FileClassifierResource> GetAsync(string id);

    [Post("/user/file-classifiers")]
    Task<FileClassifierResource> CreateAsync([Body] object body);

    [Put("/user/file-classifiers/{id}")]
    Task<FileClassifierResource> UpdateAsync(string id, [Body] object body);

    [Delete("/user/file-classifiers/{id}")]
    Task DeleteAsync(string id);
}
