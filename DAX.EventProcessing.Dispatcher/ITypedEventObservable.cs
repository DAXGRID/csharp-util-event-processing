using System.Reactive.Subjects;

namespace DAX.EventProcessing.Dispatcher;

public interface ITypedEventObservable<BaseEventType>
{
    Subject<BaseEventType> OnEvent { get; }
}
