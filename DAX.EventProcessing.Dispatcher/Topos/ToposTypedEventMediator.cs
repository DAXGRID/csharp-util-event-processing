using DAX.EventProcessing.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Topos.Config;

namespace DAX.EventProcessing.Dispatcher.Topos
{
    public class ToposTypedEventMediator<BaseEventType> : IToposTypedEventMediator<BaseEventType>
    {
        private readonly ILogger<ToposTypedEventMediator<BaseEventType>> _logger;
        private readonly IMediator _mediator;

        public ToposTypedEventMediator(
            ILogger<ToposTypedEventMediator<BaseEventType>> logger,
            IMediator mediator
            )
        {
            _logger = logger;
            _mediator = mediator;
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
                                          _logger.LogDebug($"The dispatcher got an event: {domainEvent.GetType().Name} which is now send to handler.");

                                          Type eventType = message.Body.GetType();

                                          try
                                          {
                                              await _mediator.Send(domainEvent);
                                          }
                                          catch (Exception ex)
                                          {
                                              _logger.LogError("Got an execpetion calling Mediatr.Send: " + ex);
                                              _logger.LogError(ex, ex.Message);
                                              throw;
                                          }
                                          break;

                                      // We received an event that could not be deserialized
                                      case EventCouldNotBeDeserialized unhandledEvent:
                                          _logger.LogDebug($"The received Kafka event: {unhandledEvent.EventClassName} could not be deserialized and therefore not dispatched to handler. {unhandledEvent.ErrorMessage}");
                                          break;
                                  }
                              }
                          });

            return config;
        }
    }
}
