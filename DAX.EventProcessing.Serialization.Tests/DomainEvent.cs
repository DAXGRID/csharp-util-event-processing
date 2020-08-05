using System;
using System.Collections.Generic;
using System.Text;

namespace DAX.EventProcessing.Serialization.Tests
{
    /// <summary>
    /// General event attibutes
    /// </summary>
    public class DomainEvent : IDomainEvent
    {
        public string EventType { get; set; }
        public Guid EventId { get; set; }
        public string CmdType { get; set; }
        public Guid CmdId { get; set; }
    }
}
