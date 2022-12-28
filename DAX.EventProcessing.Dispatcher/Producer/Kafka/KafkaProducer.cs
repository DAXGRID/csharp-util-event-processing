using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Topos.Config;
using Topos.Producer;

namespace DAX.EventProcessing
{
    public class KafkaProducer : IExternalEventProducer
    {
        private readonly string _kafkaServerName;
        private readonly string _certificateFilename;
        private IToposProducer _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(ILogger<KafkaProducer> logger, string kafkaServerName, string certificateFilename)
        {
            _kafkaServerName = kafkaServerName;
            _certificateFilename = certificateFilename;
            _logger = logger;
        }

        private void Init()
        {
            if (_producer is null)
            {
                _producer = Configure.Producer(c =>
                {
                    var kafkaConfig = c.UseKafka(_kafkaServerName);

                    if (_certificateFilename != null)
                    {
                        kafkaConfig.WithCertificate(_certificateFilename);
                    }
                })
                 .Logging(l => l.UseSerilog())
                 .Serialization(s => s.UseNewtonsoftJson())
                 .Create();
            }
        }

        public async Task Produce(string name, object message)
        {
            _logger.LogDebug($"Sending message topicname: {name} and body {JsonConvert.SerializeObject(message, Formatting.Indented)}");

            if (_producer == null)
                Init();

            await _producer.Send(name, new ToposMessage(message));
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }

    public static class KafkaSslExtension
    {
        public static KafkaProducerConfigurationBuilder WithCertificate(this KafkaProducerConfigurationBuilder builder, string sslCaLocation)
        {
            KafkaProducerConfigurationBuilder.AddCustomizer(builder, config =>
            {
                config.SecurityProtocol = SecurityProtocol.Ssl;
                config.SslCaLocation = sslCaLocation;
                return config;
            });
            return builder;
        }
    }
}
