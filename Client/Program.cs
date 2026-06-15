using Microsoft.Extensions.Configuration;
using Model.Dtos;
using Model.Ports;
using Temporalio.Client;



IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var options = config.GetSection("Configurations").Get<ExecutionWorkflowClient>() ??
     throw new ArgumentException("Configuration section is missing, please check");
Console.WriteLine("Please write your name");

var name = Console.ReadLine() ?? "My default name";


var server= options.Endpoint ?? 
    throw new ArgumentException("The server is missing, please check the configuration");

var taskQueue = options.QueueName ??
    throw new ArgumentException("Task queue is missing, please check the configuration");
// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new(server));

var namespaceIdString = options.WorkflowNamespaceId ??
    throw new ArgumentException("Workflow namespace ID seed is missing in configuration");

if (!Guid.TryParse(namespaceIdString, out Guid clientNamespaceId))
{
    throw new ArgumentException("The WorkflowNamespaceId in appsettings.json is not a valid GUID format.");
}

string workflowId = $"{options.WorkflowNamespaceId}-{name.Trim().ToLower()}";
// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (ISayHelloWorkflow wf) => wf.RunAsync(name),
    new(id: workflowId, taskQueue)
);

Console.WriteLine("Workflow result: {0}", result);