using System.Reactive.Subjects;

namespace DAX.EventProcessing.Dispatcher;

public interface ITypedEventObservable<BaseEventType>
{
    void Dispatch(BaseEventType baseEvent);
    Subject<BaseEventType> OnEvent { get; }
}
