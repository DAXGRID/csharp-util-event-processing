using System;

namespace DAX.EventProcessing.Dispatcher
{
    public interface IGenericEventDispatcher<BaseEventType>
    {
        void ConfigAndStart(string[] kafkaServers, string kafkaGroupName, string[] topics, string positionFilePath);
    }
}
