using FluformApp.ViewModels.Workflow;
using Microsoft.Maui.Controls;

namespace FluformApp.Views.Workflow;

public partial class WorkflowPage : ContentPage
{
    public WorkflowPage()
    {
        InitializeComponent();
    }

    public WorkflowPage(WorkflowViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }
}