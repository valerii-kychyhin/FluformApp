using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Services.Workflow;
using FluformApp.Models.Workflow;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FluformApp.ViewModels.Workflow;

public partial class WorkflowViewModel : ObservableObject
{
    private readonly WorkflowClient _client;

    [ObservableProperty]
    private ObservableCollection<WorkflowResource> workflows = new();

    [ObservableProperty]
    private bool isBusy;

    public IRelayCommand CreateTestWorkflowCommand { get; }

    public WorkflowViewModel(WorkflowClient client)
    {
        _client = client;
        CreateTestWorkflowCommand = new RelayCommand(CreateTestWorkflow);
    }

    [RelayCommand]
    public async Task LoadUserWorkflowsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var collection = await _client.GetAllUserAsync();
            Workflows.Clear();
            foreach (var wf in collection.Items)
                Workflows.Add(wf);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load workflows: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CreateTestWorkflow()
    {
        var testWorkflow = new WorkflowResource
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Test Workflow {DateTime.Now:HH:mm:ss}",
            Description = "This is a local test workflow",
            Status = "draft",
            Created = (new AuthorInfo { Name = "Local User" }, DateTime.Now),
            Updated = (new AuthorInfo { Name = "Local User" }, DateTime.Now)
        };

        Workflows.Insert(0, testWorkflow);
    }
}

