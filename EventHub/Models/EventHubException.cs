using System;

namespace EventHub
{
    public class EventHubException : Exception
    {
        public EventHubException(string message) : base(message)
        {

        }

        public EventHubException(string message, Exception e) : base(message, e)
        {

        }
    }
}
