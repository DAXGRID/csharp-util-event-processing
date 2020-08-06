﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Topos.Config;
using Topos.Serialization;

namespace DAX.EventProcessing.Serialization
{
    /// <summary>
    /// Deserializer logic that makes it easy for a consumer to deserialized payloads from an event topic into classes.
    /// The deserializer will search assemblies for classes implementing the BaseEventType.
    /// If the event class name in the Topos header "tps-msg-type" matches your class name, the event payload will be deserialized into an instance of the class.
    /// If the event class name in the Topos header don't matches any class names, you'll get an EventCouldNotBeDeserialized object back from the deserializer.
    /// </summary>
    public class GenericEventDeserializer<BaseEventType> : IMessageSerializer
    {
        private string _rebusMsgTypeHeaderKey = "tps-msg-type";
        private Dictionary<string, Type> _types = null;

        public ReceivedLogicalMessage Deserialize(ReceivedTransportMessage message)
        {
            var typeList = GetEventTypes();

            if (message is null)
                throw new ArgumentNullException($"{nameof(ReceivedTransportMessage)} is null");

            if (message.Body is null || message.Body.Length == 0)
                throw new ArgumentNullException($"{nameof(ReceivedTransportMessage)} body is null");

            var messageBody = Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);

            var eventTypeName = GetEventTypeNameFromMessage(message);

            if (eventTypeName == null)
                return new ReceivedLogicalMessage(message.Headers, new EventCouldNotBeDeserialized("notset", $"Could not find {_rebusMsgTypeHeaderKey} header"), message.Position);

            if (GetEventTypes().ContainsKey(eventTypeName.ToLower()))
            {
                var eventType = GetEventTypes()[eventTypeName.ToLower()];

                var eventObject = JsonConvert.DeserializeObject(messageBody, eventType);

                return new ReceivedLogicalMessage(message.Headers, eventObject, message.Position);
            }

            var errorEvent = new EventCouldNotBeDeserialized(eventTypeName, $"Deserializer did not found a class with the name: '{eventTypeName}' So, the event will not be deserialized!");

            return new ReceivedLogicalMessage(message.Headers, errorEvent, message.Position);
        }

        public TransportMessage Serialize(LogicalMessage message)
        {
            throw new NotImplementedException();
        }

        public string GetEventTypeNameFromMessage(ReceivedTransportMessage message)
        {
            if (message.Headers.ContainsKey(_rebusMsgTypeHeaderKey))
            {
                string msgType = message.Headers[_rebusMsgTypeHeaderKey];

                if (msgType.Contains(",") && msgType.Contains("."))
                {
                    string[] msgTypeSplit = msgType.Split(',')[0].Split('.');
                    return msgTypeSplit[msgTypeSplit.Length - 1];
                }
            }

           return null;
        }


        public Dictionary<string, Type> GetEventTypes()
        {
            if (_types != null)
                return _types;
                      
            _types = new Dictionary<string, Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GetName().Name.ToLower().StartsWith("microsoft") && !a.GetName().Name.ToLower().StartsWith("system"));

            var eventTypes =
                assemblies
                .SelectMany(x => x.GetTypes().Where(y => y.GetInterfaces().Contains(typeof(BaseEventType)) || (y.IsClass && !y.IsAbstract && y.IsSubclassOf(typeof(BaseEventType)))))
                .ToList();



            foreach (var type in eventTypes)
            {
                _types.Add(type.Name.ToLower(), type);
            }

            return _types;
        }
    }
}
