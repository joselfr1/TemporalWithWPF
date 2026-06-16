using Model.Dtos;
using Model.Ports;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Temporalio.Client;
using Workflow; // Ensure access to ISayHelloWorkflow

namespace HelloWinUI.ViewModels;

public class SayHelloViewModel : INotifyPropertyChanged
{
    // Flat primitive fields for bulletproof UI tracking
    private string _inputNameText = string.Empty;
    private string _languageCodeText = string.Empty;
    private string _greetingResult = string.Empty;
    private bool _isProcessing;

    private readonly ITemporalClient _client;
    private readonly ExecutionWorkflowClient _options;

    public SayHelloViewModel(ITemporalClient client, ExecutionWorkflowClient options)
    {
        _client = client;
        _options = options;
    }

    // Bind this to your Name TextBox: Text="{Binding InputNameText}"
    public string InputNameText
    {
        get => _inputNameText;
        set { _inputNameText = value; OnPropertyChanged(); }
    }

    // Bind this to your Language TextBox: Text="{Binding LanguageCodeText}"
    public string LanguageCodeText
    {
        get => _languageCodeText;
        set { _languageCodeText = value; OnPropertyChanged(); }
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
        // Guard checks against flat tracking strings
        if (string.IsNullOrWhiteSpace(InputNameText) || string.IsNullOrWhiteSpace(LanguageCodeText) || 
            IsProcessing || _options == null || _client == null)
            return;

        try
        {
            IsProcessing = true;
            GreetingResult = "Executing workflow on Temporal server...";

            string workflowId = $"{_options.WorkflowNamespaceId}-{InputNameText.Trim().ToLower()}";

            // 1. Pack the UI data into the DTO instance on the fly for Temporal
            var helloPayload = new HelloDto
            {
                Name = InputNameText.Trim(),
                LanguageCode = LanguageCodeText.Trim().ToLower()
            };

            // 2. Dispatch the DTO package over the network grid safely
            var result = await _client.ExecuteWorkflowAsync(
                (ISayHelloWorkflow wf) => wf.RunAsync(helloPayload),
                new(id: workflowId, _options.QueueName!)
            );

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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}