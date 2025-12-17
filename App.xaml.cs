using FluformApp.Views.Auth;
using FluformApp.Views.Profile;
using FluformApp.Views.Workflow;
using FluformApp.ViewModels.Profile;
using FluformApp.ViewModels.Workflow;
using FluformApp.Services.Profile;
using FluformApp.Services.Workflow;
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

        var token = Preferences.Get("auth_token", string.Empty);

        if (!string.IsNullOrEmpty(token))
        {
            // Профиль
            var profileApi = serviceProvider.GetRequiredService<IProfileApi>();
            var profileClient = new ProfileClient(profileApi, token);
            var profileViewModel = new ProfileViewModel(profileClient, serviceProvider);
            var profilePage = new ProfilePage(profileViewModel);

            // WorkflowClient через DI
            var workflowClient = serviceProvider.GetRequiredService<WorkflowClient>();

            // Создаем WorkflowViewModel и страницу, но пока не пушим
            var workflowViewModel = new WorkflowViewModel(workflowClient);
            var workflowPage = new WorkflowPage { BindingContext = workflowViewModel };

            // MainPage — профиль с навигацией
            MainPage = new NavigationPage(profilePage);
        }
        else
        {
            var loginPage = serviceProvider.GetRequiredService<LoginPage>();
            MainPage = new NavigationPage(loginPage);
        }

#if ANDROID
        // Запрос разрешения и получение FCM токена откладываем до OnStart
        Task.Run(async () => await RequestNotificationsAndFcmTokenAsync());
#endif
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

