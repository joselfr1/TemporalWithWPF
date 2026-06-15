
using Temporalio.Activities;

namespace Model.Ports;

public interface IHelloActivity : ITemporalActivities
{
    [Activity("SayHelloActivity")]
    string SayHello(string name);
}
