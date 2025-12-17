using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Auth;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using Microsoft.Maui.Controls;
using Refit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace FluformApp.ViewModels.Auth
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AuthClient _authClient;
        private readonly IProfileApi _profileApi;
        private readonly LoginViewModel _loginViewModel;

        public RegisterViewModel(AuthClient authClient, IProfileApi profileApi, LoginViewModel loginViewModel)
        {
            _authClient = authClient;
            _profileApi = profileApi;
            _loginViewModel = loginViewModel;
        }

        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string email = string.Empty;

        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string passwordConfirmation = string.Empty;

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isPasswordHidden = true;
        [ObservableProperty] private bool isConfirmPasswordHidden = true;

        [ObservableProperty] private bool hasMinLength;
        [ObservableProperty] private bool hasTwoDigits;
        [ObservableProperty] private int passwordStrength;
        [ObservableProperty] private bool canRegister;

        partial void OnPasswordChanged(string value)
        {
            UpdatePasswordState();
        }

        partial void OnPasswordConfirmationChanged(string value)
        {
            UpdatePasswordState();
        }

        private void UpdatePasswordState()
        {
            HasMinLength = Password.Length >= 8;
            HasTwoDigits = Password.Count(char.IsDigit) >= 2;

            // Strength: 1 = слабый, 2 = средний, 3 = сильный
            if (!HasMinLength || !HasTwoDigits)
                PasswordStrength = 1;
            else if (HasMinLength && HasTwoDigits && Password.Length < 12)
                PasswordStrength = 2;
            else
                PasswordStrength = 3;

            CanRegister = PasswordStrength == 3 && Password == PasswordConfirmation;
            OnPropertyChanged(nameof(CanRegister));
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy || !CanRegister) return;

            try
            {
                IsBusy = true;

                var request = new RegisterRequest
                {
                    name = FirstName,
                    surname = LastName,
                    email = Email,
                    password = Password,
                    password_confirmation = PasswordConfirmation
                };

                await _authClient.RegisterAsync(request);
                await Application.Current.MainPage.DisplayAlert("Success", "Registration successful", "OK");

                var loginResponse = await _authClient.LoginAsync(new LoginRequest
                {
                    Email = Email,
                    Password = Password
                });

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.token))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Auto-login failed", "OK");
                    return;
                }

                Preferences.Set("auth_token", loginResponse.token);

                var profileClient = new ProfileClient(_profileApi, loginResponse.token);
                var services = App.Services;
                var profileVm = new FluformApp.ViewModels.Profile.ProfileViewModel(profileClient, services);
                var profilePage = new FluformApp.Views.Profile.ProfilePage(profileVm);

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

        [RelayCommand]
        private async Task CancelAsync()
        {
            if (Application.Current.MainPage is NavigationPage nav)
            {
                await nav.PushAsync(new Views.Auth.LoginPage(_loginViewModel));
            }
        }
    }
}