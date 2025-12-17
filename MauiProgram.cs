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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls;
using Refit;
using System;

#if ANDROID
using Microsoft.Maui.Handlers;
using Android.Graphics;
using Android.Content.Res;
#endif

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

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
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<Entry, EntryHandler>();
        });
#endif

        var baseUri = new Uri("https://fluform-api-dev.matavi.dev/api");

        // Refit Clients
        builder.Services.AddRefitClient<IAuthApi>().ConfigureHttpClient(c => c.BaseAddress = baseUri);
        builder.Services.AddRefitClient<IGoogleAuthApi>().ConfigureHttpClient(c => c.BaseAddress = baseUri);
        builder.Services.AddRefitClient<IProfileApi>().ConfigureHttpClient(c => c.BaseAddress = baseUri);

        // Clients
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

        builder.Services.AddTransient<WorkflowClient>();
        builder.Services.AddTransient<ClassifierClient>();
        builder.Services.AddTransient<ClientAttemptClient>();
        builder.Services.AddTransient<FileClassifierClient>();
        builder.Services.AddTransient<PlaceholderClient>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<WorkflowViewModel>();
        builder.Services.AddTransient<ClassifierViewModel>();
        builder.Services.AddTransient<ClientAttemptViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<WorkflowPage>();

        var app = builder.Build();

        // Save global IServiceProvider
        Services = app.Services;

#if ANDROID
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList =
                ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
        });
#endif

        return app;
    }
}