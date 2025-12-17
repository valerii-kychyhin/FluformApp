using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluformApp.Models.Workflow;
using FluformApp.Services.Workflow;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
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

        // Создаём тестовый workflow сразу при создании ViewModel
        CreateTestWorkflow();

        Workflows.Add(new WorkflowResource
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Workflow Preview",
            Description = "This workflow is for visual preview",
            Status = "draft",
            Created = (new AuthorInfo { Name = "Local User" }, DateTime.Now),
            Updated = (new AuthorInfo { Name = "Local User" }, DateTime.Now)
        });
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
            Title = "✅ Test Workflow",
            Description = "This workflow is created for UI visualisation.",
            Status = "draft",
            Client = new ClientInfo
            {
                Name = "Test Client",
                Email = "client@example.com"
            },
            Steps = new List<WorkflowStep>
            {
                new WorkflowStep
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Step 1",
                    Description = "Description of Step 1",
                    Type = "manual",
                    Position = 1,
                    IsLocked = 0,
                    IsClientFill = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            },
            Created = (new AuthorInfo { Name = "Local User" }, DateTime.Now),
            Updated = (new AuthorInfo { Name = "Local User" }, DateTime.Now)
        };

        // Вставляем workflow в начало коллекции
        Workflows.Insert(0, testWorkflow);
    }
}
