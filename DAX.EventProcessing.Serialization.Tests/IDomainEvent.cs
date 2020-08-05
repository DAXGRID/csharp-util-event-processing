using System;
using System.Collections.Generic;
using System.Text;

namespace DAX.EventProcessing.Serialization.Tests
{
    /// <summary>
    /// Marker interface used by deserializer to find event classes
    /// </summary>
    interface IDomainEvent
    {
    }
}
