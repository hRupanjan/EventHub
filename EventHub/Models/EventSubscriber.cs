using System;

namespace EventHub
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class EventSubscriberAttribute : Attribute
    {
        public EventSubscriberAttribute()
        {
            Mode = ThreadMode.Async;
        }
        public ThreadMode Mode { get; set; }
    }

    public enum ThreadMode
    {
        Async
    }
}
