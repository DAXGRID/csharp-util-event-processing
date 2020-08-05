using System;
using System.Collections.Generic;
using System.Text;
using Topos.Config;
using Topos.Serialization;

namespace DAX.EventProcessing.Serialization
{
    public static class ToposConfigSerializationExtension
    {
        public static void GenericEventDeserializer<BaseEventType>(this StandardConfigurer<IMessageSerializer> configurer)
        {
            if (configurer == null)
                throw new ArgumentNullException(nameof(configurer));

            StandardConfigurer.Open(configurer)
                .Register(c => new GenericEventDeserializer<BaseEventType>());
        }
    }
}
