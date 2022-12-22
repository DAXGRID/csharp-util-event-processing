using System.Reactive.Subjects;

namespace DAX.EventProcessing.Dispatcher;

public class TypedEventObservable<BaseEventType> : ITypedEventObservable<BaseEventType>
{
    private readonly Subject<BaseEventType> _eventOccured = new();

    public void NewEvent(BaseEventType baseEvent)
    {
        _eventOccured.OnNext(baseEvent);
    }

    public Subject<BaseEventType> OnEvent => _eventOccured;
}
