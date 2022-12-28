using System;
using System.Threading.Tasks;

namespace DAX.EventProcessing
{
    public interface IExternalEventProducer : IDisposable
    {
        Task Produce(string name, Object message);
    }
}
