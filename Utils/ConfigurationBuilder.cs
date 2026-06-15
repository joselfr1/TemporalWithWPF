
using Microsoft.Extensions.DependencyInjection;
using Model.Ports;
using Temporalio.Worker;
using Workflow.Activities;
using Workflow.Workflows;
using Temporalio.Extensions.Hosting;
using Services;

namespace Utils;

public static class ConfigurationBuilder
{
    private static Type SubcribeHelloActivity(this ServiceCollection services)
    {
        services.AddScoped<IHelloService, HelloService>();
        services.AddScoped<IHelloActivity, HelloActivity>();
        services.AddScoped<HelloActivity>();
        return typeof(HelloActivity); 
    }
    private static Type SubcribeHelloWorkflow(this ServiceCollection services)
    {
        return typeof(SayHelloWorkflow);
    }

    public static TemporalWorkerOptions CreateOptions(this ServiceCollection services, string queueName)
    {
        var helloActivityType = services.SubcribeHelloActivity();
        var serviceProvider = services.BuildServiceProvider();

        var activityDefinitions = serviceProvider.CreateTemporalActivityDefinitions(helloActivityType);
        var helloWorkflowType = services.SubcribeHelloWorkflow();



        var workerOptions = new TemporalWorkerOptions(queueName)
            .AddWorkflow(helloWorkflowType);
        foreach (var definition in activityDefinitions)
        {
            workerOptions.AddActivity(definition);
        }

        return workerOptions;

    }

}
