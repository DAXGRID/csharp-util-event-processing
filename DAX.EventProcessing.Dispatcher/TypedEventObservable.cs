using System.Reactive.Subjects;

namespace DAX.EventProcessing.Dispatcher;

public class TypedEventObservable<BaseEventType> : ITypedEventObservable<BaseEventType>
{
    private readonly Subject<BaseEventType> _eventOccured = new();

    public Subject<BaseEventType> OnEvent => _eventOccured;
}
