using Refit;
using System.Threading.Tasks;

namespace FluformApp.Services.Auth
{
    public interface IGoogleAuthApi
    {
        [Get("/client/auth/google/redirect")]
        [Headers("Accept: application/json")]
        Task<GoogleRedirectResponse> GetRedirectUrlAsync();

        [Get("/client/auth/google/callback")]
        [Headers("Accept: application/json")]
        Task<GoogleCallbackResponse> GetCallbackAsync([Query] string code);
    }

    public class GoogleRedirectResponse
    {
        public string redirect_url { get; set; } = string.Empty;
    }

    public class GoogleCallbackResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = "bearer";
        public string expires_at { get; set; } = string.Empty;
    }
}

