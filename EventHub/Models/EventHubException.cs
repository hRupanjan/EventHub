using System;

namespace EventHubProject
{
    /// <summary>
    /// Represents errors that occur during EventHub operations.
    /// </summary>
    public class EventHubException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubProject.EventHubException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EventHubException(string message) : base(message)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubProject.EventHubException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="e">The exception that is the cause of the current exception, or a null reference</param>
        public EventHubException(string message, Exception e) : base(message, e)
        {

        }
    }
}
