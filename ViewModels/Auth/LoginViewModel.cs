using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Auth;
using FluformApp.Services.Auth;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace FluformApp.ViewModels.Auth
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthClient _authClient;

        public LoginViewModel(AuthClient authClient)
        {
            _authClient = authClient;
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        private bool rememberMe; 


        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        // EMAIL LOGIN
        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            IsBusy = true;

            var result = await _authClient.LoginAsync(new LoginRequest
            {
                Email = Email,
                Password = Password
            });

            IsBusy = false;
            
            OnPropertyChanged(nameof(CanLogin));


            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Couldn't log in", "OK");
                return;
            }

            // Переходим на профиль после успешного входа
            await Shell.Current.GoToAsync("//profile");
        }

        // GOOGLE LOGIN
        [RelayCommand]
        private async Task LoginWithGoogle()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var result = await _authClient.LoginWithGoogleAsync();

                if (result == null || string.IsNullOrWhiteSpace(result.token))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't authorize with Google", "OK");
                    return;
                }

                // Токен уже сохранён в SecureStorage в AuthClient
                await Shell.Current.GoToAsync("//profile");
            }
            catch (System.Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool CanLogin =>
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !IsBusy;

        partial void OnEmailChanged(string value)
        {
            OnPropertyChanged(nameof(CanLogin));
        }

        partial void OnPasswordChanged(string value)
        {
            OnPropertyChanged(nameof(CanLogin));
        }


    }
}
