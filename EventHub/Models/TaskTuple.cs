using System.Threading;
using System.Threading.Tasks;

namespace EventHub
{
    internal class TaskTuple
    {
        public object Event { get; set; }
        public Task RunnableTask { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
    }
}
