using Model.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsTests;

public class OptionsBinderTests
{
    [Fact]
    public void ExecutionWorkflowWorker_Should_MaintainPropertyState()
    {
        // Act: Invoke the constructor that the coverage window is flagged on
        var worker = new ExecutionWorkflowWorker() { Endpoint = "mock", QueueName = "mock", WorkflowNamespaceId = "mock" };


        Assert.NotNull(worker);
        Assert.NotNull(worker.QueueName);
        Assert.NotNull(worker.WorkflowNamespaceId);
        Assert.True(worker.EnableLocalActivities);
        Assert.Equal(100, worker.MaxConcurrentActivityExecutions);
    }
    [Fact]
    public void ExecutionWorkClient_Should_MaintainPropertyState()
    {
        // Act: Invoke the constructor that the coverage window is flagged on
        var worker = new ExecutionWorkflowClient() { Endpoint = "mock", QueueName = "mock", WorkflowNamespaceId = "mock", Id = "mock" };


        Assert.NotNull(worker);
        Assert.NotNull(worker.QueueName);
        Assert.NotNull(worker.WorkflowNamespaceId);
        Assert.NotNull(worker.Id);
    }
}
