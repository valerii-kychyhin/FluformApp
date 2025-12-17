using FluformApp.Views.Auth;
using FluformApp.Views.Profile;
using FluformApp.ViewModels.Profile;
using FluformApp.Services.Profile;
using FluformApp.Services.Workflow;
using FluformApp.Views.Workflow;
using FluformApp.ViewModels.Workflow;
using FluformApp.Services.Auth;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

#if ANDROID
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Plugin.Firebase.CloudMessaging;
#endif

namespace FluformApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;

        // Попробуем получить токен из SecureStorage / Preferences
        var token = Preferences.Get("auth_token", string.Empty);

        if (!string.IsNullOrEmpty(token))
        {
            // Если токен есть — создаём профиль
            try
            {
                var profileApi = serviceProvider.GetRequiredService<IProfileApi>();
                var profileClient = new ProfileClient(profileApi, token);
                var profileViewModel = new ProfileViewModel(profileClient, serviceProvider);
                var profilePage = new ProfilePage(profileViewModel);

                // MainPage — NavigationPage с профилем
                MainPage = new NavigationPage(profilePage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Failed to create ProfilePage: {ex.Message}");
                // Если что-то сломалось, fallback на LoginPage
                SetLoginPage(serviceProvider);
            }
        }
        else
        {
            // Если токена нет — сразу LoginPage
            SetLoginPage(serviceProvider);
        }

#if ANDROID
        // Запрашиваем разрешения и FCM токен
        Task.Run(async () => await RequestNotificationsAndFcmTokenAsync());
#endif
    }

    private void SetLoginPage(IServiceProvider serviceProvider)
    {
        try
        {
            var loginPage = serviceProvider.GetRequiredService<LoginPage>();
            MainPage = new NavigationPage(loginPage);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App] Failed to create LoginPage: {ex.Message}");
            // На всякий случай — создаём пустую страницу, чтобы MAUI не падал
            MainPage = new ContentPage
            {
                Content = new Label
                {
                    Text = "Failed to load LoginPage",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
        }
    }

#if ANDROID
    private static async Task RequestNotificationsAndFcmTokenAsync()
    {
        while (Platform.CurrentActivity == null)
            await Task.Delay(100);

        var activity = Platform.CurrentActivity;

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu &&
            ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
        {
            ActivityCompat.RequestPermissions(activity, new[] { Android.Manifest.Permission.PostNotifications }, 0);
        }

        try
        {
            var tokenFirebase = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"✅ FCM TOKEN (APP): {tokenFirebase}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ FCM ERROR: {ex.Message}");
        }
    }
#endif
}