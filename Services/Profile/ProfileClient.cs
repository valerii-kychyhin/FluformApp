using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluformApp.Models.Profile;
using FluformApp.Services.Auth;

namespace FluformApp.Services.Profile;

public class ProfileClient
{
    private readonly IProfileApi _api;
    private readonly string _bearerToken;

    public ProfileClient(IProfileApi api, string bearerToken)
    {
        _api = api;
        _bearerToken = bearerToken;
    }

    public async Task<ClientResourceResponse> GetProfileAsync()
    {
        var response = await _api.ShowProfile($"Bearer {_bearerToken}");
        return response;
    }

    public async Task<ClientResourceResponse> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await _api.UpdateProfile($"Bearer {_bearerToken}", request);
        return response;
    }
}