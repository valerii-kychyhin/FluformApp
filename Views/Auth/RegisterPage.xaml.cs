using FluformApp.ViewModels.Auth;
using Microsoft.Maui.Controls;
using FluformApp.Services.Auth;
using FluformApp.Services.Profile;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Firebase.CloudMessaging;
using FluformApp.ViewModels.Profile;
using FluformApp.Views.Profile;

namespace FluformApp.Views.Auth
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}