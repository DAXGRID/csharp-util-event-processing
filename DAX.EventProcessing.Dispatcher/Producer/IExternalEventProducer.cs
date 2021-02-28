using System;
using System.Threading.Tasks;

namespace DAX.EventProcessing
{
    public interface IExternalEventProducer : IDisposable
    {
        void Init();
        Task Produce(string topicName, Object message);
        Task Produce(string topicName, Object message, string partitionKey);
    }
}
