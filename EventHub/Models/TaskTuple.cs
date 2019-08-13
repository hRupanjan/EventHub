using System.Threading;
using System.Threading.Tasks;

namespace EventHubProject
{
    internal class TaskTuple
    {
        public object Event { get; set; }
        public Task RunnableTask { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
    }
}
