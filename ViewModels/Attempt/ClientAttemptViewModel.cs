using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Attempt;
using FluformApp.Services.Attempt;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace FluformApp.ViewModels.Attempt;

public partial class ClientAttemptViewModel : ObservableObject
{
    private readonly ClientAttemptClient _client;

    [ObservableProperty]
    private ObservableCollection<AttemptResource> attempts = new();

    [ObservableProperty]
    private bool isBusy;

    public ClientAttemptViewModel(ClientAttemptClient client)
    {
        _client = client;
    }

    [RelayCommand]
    public async Task LoadAttemptsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var result = await _client.GetAllAsync();
            Attempts.Clear();

            foreach (var attempt in result.Items)
                Attempts.Add(attempt);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Failed to load attempts: {ex.Message}",
                "OK"
            );
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CreateAttemptAsync(string workflowId)
    {
        try
        {
            var attempt = await _client.CreateAsync(workflowId);
            Attempts.Insert(0, attempt);

            await Application.Current.MainPage.DisplayAlert(
                "Success",
                "Attempt created",
                "OK"
            );
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Failed to create attempt: {ex.Message}",
                "OK"
            );
        }
    }
}