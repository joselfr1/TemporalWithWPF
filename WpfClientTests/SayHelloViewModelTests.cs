namespace WpfClientTests;

using Xunit;
using Moq;
using System.Threading.Tasks;
using Model.Dtos;
using Temporalio.Client;
using HelloWinUI.ViewModels;

public class SayHelloViewModelTests
{
    [Fact]
    public async Task SendAsync_Should_SetIsProcessing_And_InvokeTemporalWorkflow()
    {
        var mockClient = new Mock<ITemporalClient>();

        var testOptions = new ExecutionWorkflowClient
        {
            Endpoint = "localhost:7233",
            QueueName = "test-queue",
            Id="mockid",
            WorkflowNamespaceId="mocknamespace"
        };

        // Instantiate the ViewModel cleanly
        var viewModel = new SayHelloViewModel(mockClient.Object, testOptions)
        {
            InputName = "mock",
            GreetingResult= string.Empty
        };

        Assert.False(viewModel.IsProcessing);

        // Act
        await viewModel.SendAsync();

        // Assert
        Assert.False(viewModel.IsProcessing);
        Assert.NotEqual(string.Empty, viewModel.GreetingResult);
    }

    [Fact]
    public async Task SendAsync_Empty_Input()
    {
        var mockClient = new Mock<ITemporalClient>();

        var testOptions = new ExecutionWorkflowClient
        {
            Endpoint = "localhost:7233",
            QueueName = "test-queue",
            Id = "mockid",
            WorkflowNamespaceId = "mocknamespace"
        };

        // Instantiate the ViewModel cleanly
        var viewModel = new SayHelloViewModel(mockClient.Object, testOptions)
        {
            InputName = string.Empty,
            GreetingResult = string.Empty
        };

        Assert.False(viewModel.IsProcessing);

        // Act
        await viewModel.SendAsync();

        // Assert
        Assert.False(viewModel.IsProcessing);
        Assert.Equal(string.Empty, viewModel.GreetingResult);
    }

    [Fact]
    public async Task SendAsync_Should_Raise_PropertyChanged_Events()
    {
        // Arrange
        var mockClient = new Mock<ITemporalClient>();

        var testOptions = new ExecutionWorkflowClient
        {
            Endpoint = "localhost:7233",
            QueueName = "test-queue",
            Id = "mockid",
            WorkflowNamespaceId = "mocknamespace"
        };
        var viewModel = new SayHelloViewModel(mockClient.Object, testOptions)
        {
            InputName = "mock"
        };

        // 1. Create counters or tracking lists to catch the events
        bool greetingResultRaised = false;
        bool isProcessingRaised = false;

        // 2. Attach a native event handler directly to the ViewModel
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.GreetingResult))
            {
                greetingResultRaised = true;
            }
            if (e.PropertyName == nameof(viewModel.IsProcessing))
            {
                isProcessingRaised = true;
            }
        };

        // Act
        await viewModel.SendAsync();

        // Assert
        // If you forgot to call OnPropertyChanged() in your ViewModel, 
        // these assertions will fail instantly!
        Assert.True(isProcessingRaised, "PropertyChanged event was not raised for IsProcessing");
        Assert.True(greetingResultRaised, "PropertyChanged event was not raised for GreetingResult");
    }
}
