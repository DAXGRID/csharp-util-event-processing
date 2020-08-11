using System;
using System.Collections.Generic;
using System.Text;

namespace DAX.EventProcessing.Serialization
{
    public class EventCouldNotBeDeserialized
    {
        public string EventClassName { get; }
        public string ErrorMessage { get; }
        public string MessageBody { get;  }

        public EventCouldNotBeDeserialized(string eventClassName, string errorMessage, string messageBody)
        {
            EventClassName = eventClassName;
            ErrorMessage = errorMessage;
            MessageBody = messageBody;
        }
    }
}
