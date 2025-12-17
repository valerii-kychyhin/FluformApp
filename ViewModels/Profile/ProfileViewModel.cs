using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Profile;
using FluformApp.Services.Profile;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Refit;
using System;
using System.Threading.Tasks;
using FluformApp.Services.Workflow;
using FluformApp.Views.Workflow;
using FluformApp.Services.Auth;
using FluformApp.ViewModels.Workflow;
using FluformApp.Views.Auth;
using FluformApp.ViewModels.Auth;

namespace FluformApp.ViewModels.Profile;

public partial class ProfileViewModel : ObservableObject
{
    private readonly ProfileClient _profileClient;
    private readonly IServiceProvider _services;

    // Observable properties
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string surname = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string emailVerified = "Not Verified";
    [ObservableProperty] private string socialLoginType = "None";
    [ObservableProperty] private string createdAt = string.Empty;
    [ObservableProperty] private string updatedAt = string.Empty;
    [ObservableProperty] private bool isBusy;

    // Commands
    public IAsyncRelayCommand LoadProfileCommand { get; }
    public IAsyncRelayCommand UpdateProfileCommand { get; }
    public IRelayCommand LogoutCommand { get; }
    public IRelayCommand OpenWorkflowsCommand { get; }
    public IRelayCommand OpenNotificationsCommand { get; }

    public ProfileViewModel(ProfileClient profileClient, IServiceProvider services)
    {
        _profileClient = profileClient;
        _services = services;

        LoadProfileCommand = new AsyncRelayCommand(LoadProfileAsync);
        UpdateProfileCommand = new AsyncRelayCommand(UpdateProfileAsync);
        LogoutCommand = new RelayCommand(Logout);
        OpenWorkflowsCommand = new RelayCommand(OpenWorkflows);
        

        _ = LoadProfileAsync(); // Автозагрузка профиля
    }

    private async Task LoadProfileAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var profile = await _profileClient.GetProfileAsync();
            if (profile != null)
            {
                Name = profile.name ?? string.Empty;
                Surname = profile.surname ?? string.Empty;
                Email = profile.email ?? string.Empty;
                EmailVerified = profile.email_verified_at ?? "Not Verified";
                SocialLoginType = profile.social_login_type ?? "None";
                CreatedAt = TryFormatDate(profile.created_at);
                UpdatedAt = TryFormatDate(profile.updated_at);
            }
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

    private async Task UpdateProfileAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Validation Error",
                    "First Name and Last Name cannot be empty.",
                    "OK"
                );
                return;
            }

            var updated = await _profileClient.UpdateProfileAsync(new UpdateProfileRequest
            {
                name = Name,
                surname = Surname
            });

            if (updated == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Profile data not returned from the server.",
                    "OK"
                );
                return;
            }

            Name = updated.name;
            Surname = updated.surname;

            await Application.Current.MainPage.DisplayAlert(
                "Success",
                "Profile updated successfully!",
                "OK"
            );
        }
        catch (ApiException apiEx)
        {
            await ApiErrorHandler.HandleApiExceptionAsync(apiEx);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Failed to update profile: {ex.Message}",
                "OK"
            );
        }
        finally
        {
            IsBusy = false;
        }
    }

    // 🔥 Logout
    private void Logout()
    {
        Preferences.Remove("auth_token");
        Preferences.Remove("token_type");

        var loginPage = _services.GetRequiredService<LoginPage>();

        // Переключаем MainPage на LoginPage, полностью сбрасывая стек навигации
        Application.Current.MainPage = new NavigationPage(loginPage);
    }

    private async void OpenWorkflows()
    {
        try
        {
            // Получаем WorkflowClient через DI
            var workflowClient = _services.GetRequiredService<WorkflowClient>();

            // Создаем ViewModel и передаем клиент
            var workflowViewModel = new WorkflowViewModel(workflowClient);

            // Создаем страницу и устанавливаем BindingContext
            var workflowPage = new WorkflowPage
            {
                BindingContext = workflowViewModel
            };

            // Пушим страницу в навигацию
            await Application.Current.MainPage.Navigation.PushAsync(workflowPage);

            // Загружаем воркфлоу (для user workflow)
            await workflowViewModel.LoadUserWorkflowsAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to open workflows: {ex.Message}", "OK");
        }
    }


    private string TryFormatDate(string? dateString)
    {
        if (DateTime.TryParse(dateString, out var dt))
            return dt.ToString("yyyy-MM-dd HH:mm");
        return dateString ?? string.Empty;
    }
}

