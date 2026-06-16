using Microsoft.Extensions.DependencyInjection;
using Model.Ports;
using Repository;
using Services;
using Temporalio.Extensions.Hosting;
using Temporalio.Worker;
using Workflow.Activities;
using Workflow.Workflows;

namespace Utils;

public static class ConfigurationBuilder
{
    readonly static HashSet<Type> activities = [];
    private static Type SubscribeHelloActivity(this ServiceCollection services)
    {
        services.AddScoped<IHelloActivity, HelloActivity>();
        services.AddScoped<HelloActivity>();
        return typeof(HelloActivity);
    }

    private static Type SubscribeGreetingLanguageActivity(this ServiceCollection services)
    {
        services.AddScoped<IGreetingLanguageActivity, GreetingLanguageActivity>();
        services.AddScoped<GreetingLanguageActivity>();
        return typeof(GreetingLanguageActivity);
    }

    private static void AddRepositories(this ServiceCollection services)
    {
        services.AddSingleton<ILanguageRepository, LanguageRepository>();
    }

    private static void AddServices(this ServiceCollection services)
    {
        services.AddScoped<IHelloService, HelloService>();
    }

    public static void RegisterInfrastructure(this ServiceCollection services)
    {
        services.AddRepositories();
        services.AddServices();

        var helloActivityType = services.SubscribeHelloActivity();
        var languageGreetingActivityType = services.SubscribeGreetingLanguageActivity();
        activities.Add(helloActivityType);
        activities.Add(languageGreetingActivityType);

    }

    public static TemporalWorkerOptions CreateOptions(this ServiceCollection services, string queueName, ServiceProvider serviceProvider)
    {
        var workerOptions = new TemporalWorkerOptions(queueName)
            .AddWorkflow<SayHelloWorkflow>();


        // Safely map definitions using the reflection mapping extensions
        foreach (var activity in activities)
        {
            SubscribeActivities(activity, workerOptions, serviceProvider);
        }

        return workerOptions;
    }

    private static void SubscribeActivities(Type type, TemporalWorkerOptions options, ServiceProvider serviceProvider)
    {
        var activityDefinitions = serviceProvider.CreateTemporalActivityDefinitions(type);

        foreach (var definition in activityDefinitions)
        {
            options.AddActivity(definition);
        }
    }
}