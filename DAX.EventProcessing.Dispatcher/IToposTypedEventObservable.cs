using System;
using Topos.Config;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DAX.EventProcessing.Dispatcher
{
    public interface IToposTypedEventObservable<BaseEventType>
    {
        ToposConsumerConfigurer Config(string groupName, Action<StandardConfigurer<global::Topos.IConsumerImplementation>> configure);

        Subject<BaseEventType> OnEvent { get; }
    }
}
