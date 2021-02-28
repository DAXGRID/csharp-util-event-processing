using Aocl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DAX.EventProcessing
{
    public class FakeExternalEventProducer : IExternalEventProducer
    {
        private readonly ILogger<FakeExternalEventProducer> _logger;

        private readonly ConcurrentDictionary<string, AppendOnlyList<object>> _messagesByTopicName = new ConcurrentDictionary<string, AppendOnlyList<object>>();

        public FakeExternalEventProducer(ILogger<FakeExternalEventProducer> logger)
        {
            _logger = logger;
        }

        public object[] GetMessagesByTopic(string topicName)
        {
            return _messagesByTopicName[topicName].ToArray();
        }

        public Task Produce(string topicName, object toposMessage)
        {
            _logger.LogDebug($"Sending message topicname: {topicName} and body {JsonConvert.SerializeObject(toposMessage, Formatting.Indented)}");

            var topicMessages = _messagesByTopicName.GetOrAdd(topicName, new AppendOnlyList<object>());

            topicMessages.Append(toposMessage);

            return Task.CompletedTask;
        }

        public Task Produce(string topicName, object toposMessage, string partitionKey)
        {
            Produce(topicName, toposMessage);

            return Task.CompletedTask;
        }

        public void Init()
        {
        }

        public void Dispose()
        {
        }
    }
}
