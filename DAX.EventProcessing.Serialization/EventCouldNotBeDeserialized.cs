using System;
using System.Collections.Generic;
using System.Text;

namespace DAX.EventProcessing.Serialization
{
    public class EventCouldNotBeDeserialized
    {
        public string EventClassName { get; }
        public string ErrorMessage { get; }

        public EventCouldNotBeDeserialized(string eventClassName, string errorMessage)
        {
            EventClassName = eventClassName;
            ErrorMessage = errorMessage;
        }
    }
}
