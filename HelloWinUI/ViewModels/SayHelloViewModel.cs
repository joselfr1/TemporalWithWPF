using Model.Dtos;
using Model.Ports;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Temporalio.Client;

namespace HelloWinUI.ViewModels;

public class SayHelloViewModel : INotifyPropertyChanged
{
    private string _inputName = string.Empty;
    private string _greetingResult = string.Empty;
    private bool _isProcessing;

    private readonly ITemporalClient _client;
    private readonly ExecutionWorkflowClient _options;
    private readonly SynchronizationContext? _uiContext;
    public SayHelloViewModel(ITemporalClient client, ExecutionWorkflowClient options)
    {
        _client = client;
        _options = options;

        // Natively grabs the UI thread's token during construction
        _uiContext = SynchronizationContext.Current;
    }

    public string InputName
    {
        get => _inputName;
        set { _inputName = value; OnPropertyChanged(); }
    }
    public string GreetingResult
    {
        get => _greetingResult;
        set { _greetingResult = value; OnPropertyChanged(); }
    }
    public bool IsProcessing
    {
        get => _isProcessing;
        set { _isProcessing = value; OnPropertyChanged(); }
    }

    public async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(InputName) || IsProcessing || _options == null || _client == null )
            return;

        try
        {
            IsProcessing = true;
            GreetingResult = "Executing workflow on Temporal server...";

            string workflowId = $"{_options.WorkflowNamespaceId}-{InputName.Trim().ToLower()}";

            // Execute the workflow via your clean Port Interface
            var result = await _client.ExecuteWorkflowAsync(
                (ISayHelloWorkflow wf) => wf.RunAsync(InputName),
                new(id: workflowId, _options.QueueName!)
            );

            // Update the UI instantly via Data Binding
            GreetingResult = result;
        }
        catch (Exception ex)
        {
            GreetingResult = $"Workflow Error: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    // INotifyPropertyChanged Plumbing
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
