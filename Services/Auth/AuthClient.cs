using System;
using System.Threading.Tasks;
using FluformApp.Models.Auth;
using Microsoft.Maui.Storage;

namespace FluformApp.Services.Auth
{
    public class AuthClient
    {
        private readonly IAuthApi _api;

        // keys for SecureStorage
        private const string TokenKey = "access_token";
        private const string TokenTypeKey = "token_type";

        public AuthClient(IAuthApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Register user. Returns the LoginResponse (token + token_type) from server.
        /// </summary>
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            var resp = await _api.Register(request);
            // Save token if returned
            if (!string.IsNullOrWhiteSpace(resp?.token))
            {
                await SecureStorage.SetAsync(TokenKey, resp.token);
                await SecureStorage.SetAsync(TokenTypeKey, resp.token_type ?? "bearer");
            }
            return resp;
        }

        /// <summary>
        /// Login user. Returns the LoginResponse (token + token_type) from server.
        /// </summary>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var resp = await _api.Login(request);
            if (!string.IsNullOrWhiteSpace(resp?.token))
            {
                await SecureStorage.SetAsync(TokenKey, resp.token);
                await SecureStorage.SetAsync(TokenTypeKey, resp.token_type ?? "bearer");
            }
            return resp;
        }

        /// <summary>
        /// Get saved bearer token (or null).
        /// </summary>
        public async Task<string?> GetSavedTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(TokenKey);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Clear saved credentials locally.
        /// </summary>
        public void ClearLocalCredentials()
        {
            try
            {
                SecureStorage.Default.Remove(TokenKey);
                SecureStorage.Default.Remove(TokenTypeKey);
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// Logout via API (server) and clear local storage.
        /// </summary>
        public async Task LogoutAsync()
        {
            var token = await GetSavedTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                await _api.Logout($"Bearer {token}");
            }
            ClearLocalCredentials(); // чистим SecureStorage
        }
    }
}
