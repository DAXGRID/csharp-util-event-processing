using System.Reactive.Subjects;

namespace DAX.EventProcessing.Dispatcher;

public interface ITypedEventObservable<BaseEventType>
{
    void NewEvent(BaseEventType baseEvent);
    Subject<BaseEventType> OnEvent { get; }
}
