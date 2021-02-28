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
        private IToposProducer _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(string kafkaServerName, ILogger<KafkaProducer> logger)
        {
            _kafkaServerName = kafkaServerName;
            _logger = logger;
        }

        public void Init()
        {
            if (_producer is null)
            {
                _producer = Configure.Producer(c => c.UseKafka(_kafkaServerName))
                    .Serialization(s => s.UseNewtonsoftJson())
                    .Create();
            }
        }

        public async Task Produce(string topicName, object toposMessage)
        {
            _logger.LogDebug($"Sending message topicname: {topicName} and body {JsonConvert.SerializeObject(toposMessage, Formatting.Indented)}");

            if (_producer == null)
                Init();

            await _producer.Send(topicName, new ToposMessage(toposMessage));
        }

        public async Task Produce(string topicName, object toposMessage, string partitionKey)
        {
            await _producer.Send(topicName, new ToposMessage(toposMessage), partitionKey);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
