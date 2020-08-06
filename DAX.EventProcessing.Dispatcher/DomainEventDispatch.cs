using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAX.EventProcessing.Dispatcher
{
    public class DomainEventDispatch<EventType> : IRequest
    {
        readonly EventType _event;

        public EventType Event => _event;

        public DomainEventDispatch(EventType @event)
        {
            _event = @event;
        }
    }
}
