using Refit;
using FluformApp.Models.Placeholder;

namespace FluformApp.Services.Placeholder;

public interface IPlaceholderApi
{
    [Get("/user/placeholders")]
    Task<PlaceholderCollection> GetAllAsync();

    [Get("/user/placeholders/{id}")]
    Task<PlaceholderResource> GetAsync(string id);

    [Post("/user/placeholders")]
    Task<PlaceholderResource> CreateAsync([Body] object body);

    [Put("/user/placeholders/{id}")]
    Task<PlaceholderResource> UpdateAsync(string id, [Body] object body);

    [Delete("/user/placeholders/{id}")]
    Task DeleteAsync(string id);
}