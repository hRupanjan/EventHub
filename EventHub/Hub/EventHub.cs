using System;
namespace EventHub
{
    public abstract class EventHub
    {
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
        /// The method to cancel further posting of an event
        /// </summary>
        /// <exception cref="EventHubException" />
        /// <exception cref="NullReferenceException" />
        /// <param name="eventType">The instance of the event</param>
        public abstract void Cancel(object eventType);
    }
}