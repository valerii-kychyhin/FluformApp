using FluformApp.ViewModels.Profile;

namespace FluformApp.Views.Profile;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnNotificationsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Notifications", "System notifications will appear here.", "OK");
    }
}
