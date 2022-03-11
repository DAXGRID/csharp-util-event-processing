using System;
using System.Reactive.Subjects;
using Topos.Config;

namespace DAX.EventProcessing.Dispatcher
{
    public interface IToposTypedEventObservable<BaseEventType>
    {
        ToposConsumerConfigurer Config(string groupName, Action<StandardConfigurer<global::Topos.IConsumerImplementation>> configure);
        Subject<BaseEventType> OnEvent { get; }
    }
}
