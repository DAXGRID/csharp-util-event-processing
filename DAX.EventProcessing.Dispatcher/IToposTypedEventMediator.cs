using System;
using Topos.Config;

namespace DAX.EventProcessing.Dispatcher
{
    public interface IToposTypedEventMediator<BaseEventType>
    {
        ToposConsumerConfigurer Config(string groupName, Action<StandardConfigurer<global::Topos.IConsumerImplementation>> configure);
    }
}
