using Refit;
using System.Threading.Tasks;
using FluformApp.Models.Profile;

namespace FluformApp.Services.Profile
{
    public interface IProfileApi
    {
        [Get("/client/profile/show")]
        [Headers("Accept: application/json")]
        Task<ClientResourceResponse> ShowProfile([Header("Authorization")] string bearerToken);

        [Post("/client/profile/update")]
        [Headers("Accept: application/json")]
        Task<ClientResourceResponse> UpdateProfile(
            [Header("Authorization")] string bearerToken,
            [Body] UpdateProfileRequest request
        );
    }
}