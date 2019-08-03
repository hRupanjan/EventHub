using System.Reflection;

namespace EventHub
{
    internal class CalleeMethod
    {
        public MethodInfo Method { get; set; }
        public ThreadMode Mode { get; set; }
    }
}
