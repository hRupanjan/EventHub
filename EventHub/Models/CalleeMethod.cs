using System.Reflection;

namespace EventHubProject
{
    internal class CalleeMethod
    {
        public MethodInfo Method { get; set; }
        public ThreadMode Mode { get; set; }
    }
}
