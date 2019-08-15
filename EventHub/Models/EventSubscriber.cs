using System;

namespace EventHubProject
{
    /// <summary>
    /// Mentioning this attribute will mark the function as event handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class EventSubscriberAttribute : Attribute
    {
        /// <summary>
        /// The default constructor that creates an instance of this attribute
        /// </summary>
        public EventSubscriberAttribute()
        {
            Mode = ThreadMode.Async;
        }
        /// <summary>
        /// The mode in which the handler will be executed
        /// </summary>
        public ThreadMode Mode { get; set; }
    }
    /// <summary>
    /// The handler execution type enum
    /// </summary>
    public enum ThreadMode
    {
        /// <summary>
        /// Asynchronous mode
        /// </summary>
        Async
    }
}
