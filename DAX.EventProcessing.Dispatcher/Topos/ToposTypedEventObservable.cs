using DAX.EventProcessing.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Topos.Config;

namespace DAX.EventProcessing.Dispatcher.Topos
{
    public class ToposTypedEventObservable<BaseEventType> : IToposTypedEventObservable<BaseEventType>
    {
        private readonly ILogger<IToposTypedEventObservable<BaseEventType>> _logger;
        private readonly Subject<BaseEventType> _eventOccured = new();

        public ToposTypedEventObservable(ILogger<ToposTypedEventObservable<BaseEventType>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sets up serialization and handler. You need to setup topics, logging etc. according to Topos documentation.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ToposConsumerConfigurer Config(string groupName, Action<StandardConfigurer<global::Topos.IConsumerImplementation>> configure)
        {
            var config = Configure.Consumer(groupName, configure)
                .Serialization(s => s.GenericEventDeserializer<BaseEventType>())
                .Handle(async (messages, context, token) =>
                {
                    foreach (var message in messages)
                    {
                        switch (message.Body)
                        {
                            // We received an event that a class is defined for, so it should be handled by someone
                            case BaseEventType domainEvent:
                                _logger.LogDebug($"The dispatcher got an event: {domainEvent.GetType().Name} which is send to observers.");

                                try
                                {
                                    _eventOccured.OnNext(domainEvent);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("Got an execpetion calling observer: " + ex);
                                    _logger.LogError(ex, ex.Message);
                                    throw;
                                }
                                break;

                            // We received an event that could not be deserialized
                            case EventCouldNotBeDeserialized unhandledEvent:
                                _logger.LogDebug($"The received Kafka event: {unhandledEvent.EventClassName} could not be deserialized and therefore not dispatched to observers. {unhandledEvent.ErrorMessage}");
                                break;
                        }
                    }

                    await Task.CompletedTask;
                });

            return config;
        }

        public Subject<BaseEventType> OnEvent => _eventOccured;
    }
}
