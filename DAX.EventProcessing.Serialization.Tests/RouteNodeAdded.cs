using System;
using System.Collections.Generic;
using System.Text;

namespace DAX.EventProcessing.Serialization.Tests
{
    /// <summary>
    /// Event type used for testing
    /// </summary>
    public class RouteNodeAdded : DomainEvent
    {
        public Guid NodeId { get; set; }
    }
}
