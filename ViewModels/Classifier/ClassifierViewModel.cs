using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Classifier;
using FluformApp.Services.Classifier;

namespace FluformApp.ViewModels.Classifier;

public partial class ClassifierViewModel : ObservableObject
{
    private readonly ClassifierClient _client;

    [ObservableProperty]
    private List<ClassifierResource> classifiers = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ClassifierViewModel(ClassifierClient client)
    {
        _client = client;
    }

    [RelayCommand]
    public async Task LoadClassifiersAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Classifiers = (await _client.GetAllAsync()).Items;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteClassifierAsync(string id)
    {
        try
        {
            await _client.DeleteAsync(id);
            await LoadClassifiersAsync(); // обновляем список
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}