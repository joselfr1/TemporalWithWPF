using System;
using System.Collections.Generic;
using System.Text;
using Temporalio.Activities;

namespace Model.Ports;

public interface IGreetingLanguageActivity : ITemporalActivities
{
    [Activity("GreetingLanguageActivity")]
    string? GetGreetingMessage(string code);
}
