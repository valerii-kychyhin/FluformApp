using Refit;
using FluformApp.Models.Classifier;

namespace FluformApp.Services.Classifier;

public interface IClassifierApi
{
    [Get("/user/classifiers")]
    Task<ClassifierCollection> GetAllAsync();

    [Get("/user/classifiers/{id}")]
    Task<ClassifierResource> GetAsync(string id);

    [Post("/user/classifiers")]
    Task<ClassifierResource> CreateAsync([Body] ClassifierRequest request);

    [Put("/user/classifiers/{id}")]
    Task<ClassifierResource> UpdateAsync(string id, [Body] ClassifierRequest request);

    [Delete("/user/classifiers/{id}")]
    Task DeleteAsync(string id);
}