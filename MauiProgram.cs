using FluformApp;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using FluformApp.Services.Workflow;
using FluformApp.Services.Attempt;
using FluformApp.Services.Classifier;
using FluformApp.Services.FileClassifier;
using FluformApp.Services.Placeholder;

using FluformApp.ViewModels.Auth;
using FluformApp.ViewModels.Profile;
using FluformApp.ViewModels.Workflow;
using FluformApp.ViewModels.Classifier;
using FluformApp.ViewModels.Attempt;

using FluformApp.Views.Auth;
using FluformApp.Views.Profile;
using FluformApp.Views.Workflow;

using Refit;
using Microsoft.Maui.Storage;
using Plugin.Firebase.CloudMessaging;
using System;
using System.Net.Http.Headers;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Urbanist-SemiBold.ttf", "UrbanistSemiBold");
            });

        var baseUri = new Uri("https://fluform-api-dev.matavi.dev/api");

        // Для всех Refit клиентов создаём фабрику
        builder.Services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri);

        builder.Services.AddRefitClient<IProfileApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri);

        builder.Services.AddRefitClient<IWorkflowApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = baseUri;
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var token = Preferences.Get("auth_token", string.Empty);
                if (!string.IsNullOrEmpty(token))
                    c.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            });

        builder.Services.AddRefitClient<IClientAttemptApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = baseUri;
                var token = Preferences.Get("auth_token", string.Empty);
                if (!string.IsNullOrEmpty(token))
                    c.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            });

        builder.Services.AddRefitClient<IFileClassifierApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = baseUri;
                var token = Preferences.Get("auth_token", string.Empty);
                if (!string.IsNullOrEmpty(token))
                    c.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            });

        builder.Services.AddRefitClient<IPlaceholderApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = baseUri;
                var token = Preferences.Get("auth_token", string.Empty);
                if (!string.IsNullOrEmpty(token))
                    c.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            });

        // Clients
        builder.Services.AddSingleton<AuthClient>();
        builder.Services.AddTransient<GoogleAuthClient>();
        builder.Services.AddTransient<ClassifierClient>();
        builder.Services.AddTransient<ClientAttemptClient>();
        builder.Services.AddTransient<FileClassifierClient>();
        builder.Services.AddTransient<PlaceholderClient>();

        builder.Services.AddSingleton<WorkflowClient>(sp =>
        {
            var api = sp.GetRequiredService<IWorkflowApi>();
            return new WorkflowClient(api);
        });

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<WorkflowViewModel>();
        builder.Services.AddTransient<ClassifierViewModel>();
        builder.Services.AddTransient<ClientAttemptViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<WorkflowPage>();

#if ANDROID
        var firebase = CrossFirebaseCloudMessaging.Current;

        firebase.TokenChanged += (s, e) =>
            Console.WriteLine($"🔥 NEW FCM TOKEN: {e.Token}");

        firebase.NotificationReceived += (s, e) =>
            Console.WriteLine("📩 УВЕДОМЛЕНИЕ ПОЛУЧЕНО");
#endif

        return builder.Build();
    }
}