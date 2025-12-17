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

using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls;

using Refit;
using Plugin.Firebase.CloudMessaging;
using System;
using System.Net.Http;

#if ANDROID
using Microsoft.Maui.Handlers;
using Android.Content.Res;
using Android.Graphics;
#endif

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

#if ANDROID
        // =========================
        // ANDROID ENTRY: REMOVE UNDERLINE
        // =========================
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<Entry, EntryHandler>();
        });
#endif

        var baseUri = new Uri("https://fluform-api-dev.matavi.dev/api");

        // =========================
        // AUTH HANDLER
        // =========================
        builder.Services.AddTransient<AuthHeaderHandler>();

        // =========================
        // REFIT CLIENTS
        // =========================
        builder.Services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri);

        builder.Services.AddRefitClient<IGoogleAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri);

        builder.Services.AddRefitClient<IProfileApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IWorkflowApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IClientAttemptApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IFileClassifierApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddRefitClient<IPlaceholderApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseUri)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // =========================
        // CLIENTS
        // =========================
        builder.Services.AddSingleton<GoogleAuthClient>(sp =>
        {
            var api = sp.GetRequiredService<IGoogleAuthApi>();
            return new GoogleAuthClient(api);
        });

        builder.Services.AddSingleton<AuthClient>(sp =>
        {
            var api = sp.GetRequiredService<IAuthApi>();
            var googleAuth = sp.GetRequiredService<GoogleAuthClient>();
            return new AuthClient(api, googleAuth);
        });

        builder.Services.AddTransient<ClassifierClient>();
        builder.Services.AddTransient<ClientAttemptClient>();
        builder.Services.AddTransient<FileClassifierClient>();
        builder.Services.AddTransient<PlaceholderClient>();

        builder.Services.AddSingleton<WorkflowClient>();

        // =========================
        // VIEWMODELS
        // =========================
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<WorkflowViewModel>();
        builder.Services.AddTransient<ClassifierViewModel>();
        builder.Services.AddTransient<ClientAttemptViewModel>();

        // =========================
        // PAGES
        // =========================
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

        var app = builder.Build();

#if ANDROID
        // =========================
        // APPLY ENTRY STYLE (NO UNDERLINE)
        // =========================
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList =
                ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
        });
#endif

        // =========================
        // ВАЖНО: вернуть app
        // =========================
        return app;
    }
}