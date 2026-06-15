using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Dtos;

public class ExecutionWorkflowWorker: Configurations
{
    public int MaxConcurrentActivityExecutions { get; set; } = 100;
    public bool EnableLocalActivities { get; set; } = true;
}
