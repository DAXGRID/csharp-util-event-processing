using DAX.EventProcessing.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Topos.Config;

namespace DAX.EventProcessing.Dispatcher.Kafka
{
    public class KafkaEventDispatcher<BaseEventType> : IGenericEventDispatcher<BaseEventType>
    {
        private IDisposable _consumer;
        private readonly ILogger<KafkaEventDispatcher<BaseEventType>> _logger;
        private readonly IMediator _mediator;

        public KafkaEventDispatcher(
            ILogger<KafkaEventDispatcher<BaseEventType>> logger,
            IMediator mediator
            )
        {
            _logger = logger;
            _mediator = mediator;
        }

        public void ConfigAndStart(string[] kafkaServers, string kafkaGroupName, string[] topics, string positionFilePath)
        {
            var config = Configure
                          .Consumer(kafkaGroupName, c => c.UseKafka(kafkaServers))
                          .Logging(l => l.UseSerilog())
                          .Serialization(s => s.GenericEventDeserializer<BaseEventType>())
                          .Positions(p => p.StoreInFileSystem(positionFilePath))
                          .Handle(async (messages, context, token) =>
                          {
                              foreach (var message in messages)
                              {
                                  switch (message.Body)
                                  {
                                      // We received an event that a class is defined for, so it should be handled by someone
                                      case BaseEventType domainEvent:
                                          _logger.LogDebug($"The received Kafka event: {domainEvent.GetType().Name} is send to handler...");

                                          Type eventType = message.Body.GetType();
                                          await _mediator.Send(domainEvent);
                                          break;

                                      // We received an event that could not be deserialized
                                      case EventCouldNotBeDeserialized unhandledEvent:
                                          _logger.LogDebug($"The received Kafka event: {unhandledEvent.EventClassName} could not be deserialized and therefore not dispatched to handler. {unhandledEvent.ErrorMessage}");
                                          break;
                                  }
                              }
                          });

            foreach (var topic in topics)
                config.Topics(t => t.Subscribe(topic));

            _consumer = config.Start();
        }

        public void Dispose()
        {
        }
    }
}
