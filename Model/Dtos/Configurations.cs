namespace Model.Dtos;

public abstract class Configurations
{
    public required string Endpoint { get; set; }
    public required string QueueName { get; set; }
    public required string WorkflowNamespaceId { get; set; }
}
