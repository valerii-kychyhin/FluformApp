using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Auth;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using FluformApp.Views.Profile;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Refit;
using System.Threading.Tasks;
using FluformApp.ViewModels.Profile;
using System;

namespace FluformApp.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthClient _authClient;
    private readonly IProfileApi _profileApi;
    private readonly IServiceProvider _serviceProvider;

    public LoginViewModel(AuthClient authClient, IProfileApi profileApi, IServiceProvider serviceProvider)
    {
        _authClient = authClient;
        _profileApi = profileApi;
        _serviceProvider = serviceProvider;
        IsPasswordHidden = true;
    }

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isPasswordHidden;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Email and password are required.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var tokenResponse = await _authClient.LoginAsync(new LoginRequest
            {
                Email = Email,
                Password = Password
            });

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.token))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No token returned from server.", "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Success", "Logged in successfully!", "OK");

            // Создаём клиент профиля с токеном
            var profileClient = new ProfileClient(_profileApi, tokenResponse.token);

            // Создаём ViewModel и страницу
            var profileViewModel = new ProfileViewModel(profileClient, _serviceProvider);
            var profilePage = new ProfilePage(profileViewModel);

            Application.Current.MainPage = new NavigationPage(profilePage);
        }
        catch (ApiException apiEx)
        {
            await ApiErrorHandler.HandleApiExceptionAsync(apiEx);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}