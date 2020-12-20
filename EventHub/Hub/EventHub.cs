using System;
namespace EventHubProject
{
    /// <summary>
    /// The class for calling the EventHub
    /// </summary>
    public abstract class EventHub
    {
        /// <summary>
        /// This event notifies of the exceptions that has occured on calling the subscribed events
        /// </summary>
        public event Action<AggregateException> OnCallFailure;
        /// <summary>
        /// method to invoke the exception notifier
        /// </summary>
        /// <param name="exceptions">The exceptions</param>
        protected void InvokeOnCallFailure(AggregateException exceptions)
        {
            OnCallFailure?.Invoke(exceptions);
        }
        private static EventHub defaultInstance;
        /// <summary>
        /// The default instance of the singleton class
        /// </summary>
        public static EventHub Instance
        {
            get
            {
                if (defaultInstance == null)
                    defaultInstance = new RealHub();
                return defaultInstance;
            }
        }
        /// <summary>
        /// The default constructor for the EventHub
        /// </summary>
        protected EventHub()
        {

        }
        /// <summary>
        /// The method that registers the instance of the subscribing class for calling the subscribing methods.
        /// 
        /// Throws Exception when multiple subscribers in the same class has same DataType or Subscribers have zero or more than one parameters in their set.
        /// </summary>
        /// <exception cref="EventHubException" />
        /// <exception cref="NullReferenceException" />
        /// <param name="subscribingClass">The instance of the subscribing class</param>
        public abstract void Register(object subscribingClass);
        /// <summary>
        /// The method that deregisters the instance of the subscribed class
        /// </summary>
        /// <exception cref="NullReferenceException" />
        /// <param name="subscribingClass">The instance of the subscribed class</param>
        public abstract void Deregister(object subscribingClass);
        /// <summary>
        /// The method to post an event with a unique DataType
        /// </summary>
        /// <exception cref="NullReferenceException" />
        /// <param name="eventType">The instance of the event</param>
        public abstract void Post(object eventType);
        /// <summary>
        /// Subscribe to a specific type of event
        /// </summary>
        /// <typeparam name="T">The type of event</typeparam>
        /// <param name="actionOnEvent">Passing the event object into the action</param>
        public abstract void Register<T>(Action<T> actionOnEvent);
        /// <summary>
        /// Deregister from a specific type of event
        /// </summary>
        /// <typeparam name="T">The type of event</typeparam>
        /// <param name="actionOnEvent">The action to be deregsitered</param>
        public abstract void Deregister<T>(Action<T> actionOnEvent);
        /// <summary>
        /// The method to cancel further posting of an event
        /// </summary>
        /// <exception cref="EventHubException" />
        /// <exception cref="NullReferenceException" />
        /// <param name="eventType">The instance of the event</param>
        public abstract void Cancel(object eventType);
    }
}