using System;
using System.Threading.Tasks;
using FluformApp.Models.Auth;
using Microsoft.Maui.Storage;
using Refit;

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
            try
            {
                var resp = await _api.Login(request);

                if (!string.IsNullOrWhiteSpace(resp?.token))
                    await SaveTokenAsync(resp.token, resp.token_type);

                return resp;
            }
            catch (ApiException apiEx)
            {
                // Ошибка API (например, 401, 400)
                System.Diagnostics.Debug.WriteLine($"Login API error: {apiEx.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                // Любая другая ошибка (сеть, сериализация)
                System.Diagnostics.Debug.WriteLine($"Login failed: {ex}");
                return null;
            }
        }

        // ===== REGISTER =====
        public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var resp = await _api.Register(request);

                if (!string.IsNullOrWhiteSpace(resp?.token))
                    await SaveTokenAsync(resp.token, resp.token_type);

                return resp;
            }
            catch (ApiException apiEx)
            {
                System.Diagnostics.Debug.WriteLine($"Register API error: {apiEx.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register failed: {ex}");
                return null;
            }
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
            try
            {
                await SecureStorage.SetAsync(TokenKey, token);
                await SecureStorage.SetAsync(TokenTypeKey, type ?? "bearer");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SecureStorage error: {ex}");
            }
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
            try
            {
                var token = await GetSavedTokenAsync();

                if (!string.IsNullOrWhiteSpace(token))
                    await _api.Logout($"Bearer {token}");

                ClearLocalCredentials();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logout failed: {ex}");
            }
        }
    }
}