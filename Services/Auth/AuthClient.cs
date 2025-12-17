using System;
using System.Threading.Tasks;
using FluformApp.Models.Auth;
using Microsoft.Maui.Storage;

namespace FluformApp.Services.Auth
{
    public class AuthClient
    {
        private readonly IAuthApi _api;
        private readonly GoogleAuthClient _googleAuth;

        private const string TokenKey = "access_token";
        private const string TokenTypeKey = "token_type";

        public AuthClient(IAuthApi api, GoogleAuthClient googleAuth)
        {
            _api = api;
            _googleAuth = googleAuth;
        }

        // ===== EMAIL LOGIN =====
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var resp = await _api.Login(request);

            if (!string.IsNullOrWhiteSpace(resp?.token))
            {
                await SaveTokenAsync(resp.token, resp.token_type);
            }

            return resp;
        }

        // ===== REGISTER =====
        public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
        {
            var resp = await _api.Register(request);

            if (!string.IsNullOrWhiteSpace(resp?.token))
            {
                await SaveTokenAsync(resp.token, resp.token_type);
            }

            return resp;
        }

        // ===== GOOGLE LOGIN =====
        public async Task<LoginResponse?> LoginWithGoogleAsync()
        {
            var callback = await _googleAuth.AuthenticateAsync();

            if (callback == null || string.IsNullOrWhiteSpace(callback.access_token))
                return null;

            await SaveTokenAsync(callback.access_token, callback.token_type);

            return new LoginResponse
            {
                token = callback.access_token,
                token_type = callback.token_type
            };
        }

        // ===== TOKEN STORAGE =====
        private async Task SaveTokenAsync(string token, string? type)
        {
            await SecureStorage.SetAsync(TokenKey, token);
            await SecureStorage.SetAsync(TokenTypeKey, type ?? "bearer");
        }

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

        public void ClearLocalCredentials()
        {
            try
            {
                SecureStorage.Remove(TokenKey);
                SecureStorage.Remove(TokenTypeKey);
            }
            catch { }
        }

        public async Task LogoutAsync()
        {
            var token = await GetSavedTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                await _api.Logout($"Bearer {token}");
            }

            ClearLocalCredentials();
        }
    }
}