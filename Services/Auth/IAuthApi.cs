using Refit;
using System.Threading.Tasks;
using FluformApp.Models.Auth;

namespace FluformApp.Services.Auth
{
    public interface IAuthApi
    {
        // Register: returns token + token_type according to API
        [Post("/client/auth/register")]
        [Headers("Content-Type: application/json", "Accept: application/json")]
        Task<LoginResponse> Register([Body] RegisterRequest request);

        // Login: returns token + token_type
        [Post("/client/auth/login")]
        [Headers("Content-Type: application/json", "Accept: application/json")]
        Task<LoginResponse> Login([Body] LoginRequest request);

        // Logout: server-side logout using bearer token
        [Post("/client/profile/logout")]
        [Headers("Accept: application/json")]
        Task Logout([Header("Authorization")] string authorization);
    }
}
