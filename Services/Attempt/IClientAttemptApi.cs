using Refit;
using FluformApp.Models.Attempt;

namespace FluformApp.Services.Attempt;

public interface IClientAttemptApi
{
    [Get("/client/profile/attempts")]
    Task<AttemptCollection> GetAllAsync(
        [Query] int current = 1,
        [Query] int pageSize = 10,
        [Query] string? sort = "-created_at"
    );

    [Post("/client/profile/attempts")]
    Task<AttemptResource> CreateAsync([Body] CreateAttemptRequest request);

    [Get("/client/profile/attempts/{id}")]
    Task<AttemptResource> GetAsync(string id);

    [Put("/client/profile/attempts/{id}")]
    Task<AttemptResource> UpdateAsync(string id, [Body] object body);

    [Delete("/client/profile/attempts/{id}")]
    Task DeleteAsync(string id);
}