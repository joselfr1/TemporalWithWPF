using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Dtos;
using Temporalio.Client;
using Temporalio.Worker;
using Utils;

IConfiguration config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var options = config.GetSection("Configurations").Get<ExecutionWorkflowWorker>() ??
    throw new ArgumentException("The Configurations section is missing, please check the configuration file");

var clientAddress = options.Endpoint ??
    throw new ArgumentException("The client is missing, please check the configuration");

var client = await TemporalClient.ConnectAsync(new(clientAddress));

using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// 1. Create the collection and run your extension setup
var serviceCollection = new ServiceCollection();
serviceCollection.RegisterInfrastructure();
using var serviceProvider = serviceCollection.BuildServiceProvider();
var workerOptions = serviceCollection.CreateOptions(options.QueueName,serviceProvider);


using var worker = new TemporalWorker(client, workerOptions);
Console.WriteLine("Running worker");

try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}