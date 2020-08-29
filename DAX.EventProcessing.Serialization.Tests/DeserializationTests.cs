using System;
using System.Collections.Generic;
using System.Text;
using Topos.Consumer;
using Topos.Serialization;
using Xunit;

namespace DAX.EventProcessing.Serialization.Tests
{
    public class DeserializationTests
    {
        [Fact]
        public void Deserialize_StandardToposSerializedEvent_ShouldWork()
        {
            var position = new Position("hey", 1, 666);

            var headers = new Dictionary<string, string>();

            headers.Add("tps-msg-type", "OpenFTTH.GDBIntegrator.Integrator.EventMessages.RouteNodeAdded, OpenFTTH.GDBIntegrator.Integrator");

            var bodyJson = "{\"EventType\":\"RouteNodeAdded\",\"EventTs\":\"2020-07-28T08:04:37.8098439Z\",\"CmdId\":\"e3693ffe-be4b-490b-9b1d-5c2883ea3849\",\"EventId\":\"351428bc-1606-44ff-b6e4-396664ccef9b\",\"NodeId\":\"b41a377c-ebe6-4327-9f08-ff58f70c2bb1\",\"Geometry\":\"[538888.8171487605,6210640.758167612]\"}";

            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);

            var messageToDeserialized = new ReceivedTransportMessage(position, headers, bodyBytes);

            var logicalMessage = new GenericEventDeserializer<DomainEvent>().Deserialize(messageToDeserialized);

            // Check if got a route node added object from deserializer
            Assert.True(logicalMessage.Body is RouteNodeAdded);

            var routeNodeAddedEvent = logicalMessage.Body as RouteNodeAdded;

            Assert.Equal("RouteNodeAdded", (routeNodeAddedEvent).EventType);

            // Check if event sequence number was set to 666
            Assert.Equal(666, routeNodeAddedEvent.EventSequenceNumber);

        }

        [Fact]
        public void Deserialize_SpecificType_ShouldWork()
        {
            var position = new Position("hey", 1, 666);

            var headers = new Dictionary<string, string>();

            headers.Add("tps-msg-type", "OpenFTTH.GDBIntegrator.Integrator.EventMessages.RouteNodeAdded, OpenFTTH.GDBIntegrator.Integrator");

            var bodyJson = "{\"EventType\":\"RouteNodeAdded\",\"EventTs\":\"2020-07-28T08:04:37.8098439Z\",\"CmdId\":\"e3693ffe-be4b-490b-9b1d-5c2883ea3849\",\"EventId\":\"351428bc-1606-44ff-b6e4-396664ccef9b\",\"NodeId\":\"b41a377c-ebe6-4327-9f08-ff58f70c2bb1\",\"Geometry\":\"[538888.8171487605,6210640.758167612]\"}";

            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);

            var messageToDeserialized = new ReceivedTransportMessage(position, headers, bodyBytes);

            var logicalMessage = new GenericEventDeserializer<RouteNodeAdded>().Deserialize(messageToDeserialized);

            // Check if got a route node added object from deserializer
            Assert.True(logicalMessage.Body is RouteNodeAdded);

            var routeNodeAddedEvent = logicalMessage.Body as RouteNodeAdded;

            Assert.Equal("RouteNodeAdded", (routeNodeAddedEvent).EventType);

            // Check if event sequence number was set to 666
            Assert.Equal(666, routeNodeAddedEvent.EventSequenceNumber);

        }

        [Fact]
        public void Deserialize_UndefinedClass_ShouldReturnEventCouldNotBeDeserializedEvent()
        {
            // Create an event of type SomeWeirdEvent, that we have no class defined for
            var position = new Position();
            var headers = new Dictionary<string, string>();

            headers.Add("tps-msg-type", "OpenFTTH.GDBIntegrator.Integrator.EventMessages.SomeWeirdEvent, OpenFTTH.GDBIntegrator.Integrator");

            var bodyJson = "{\"EventType\":\"SomeWeirdEvent\",\"EventTs\":\"2020-07-28T08:04:37.8098439Z\",\"CmdId\":\"e3693ffe-be4b-490b-9b1d-5c2883ea3849\",\"EventId\":\"351428bc-1606-44ff-b6e4-396664ccef9b\",\"NodeId\":\"b41a377c-ebe6-4327-9f08-ff58f70c2bb1\",\"Geometry\":\"[538888.8171487605,6210640.758167612]\"}";

            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);

            var messageToDeserialized = new ReceivedTransportMessage(position, headers, bodyBytes);

            var logicalMessage = new GenericEventDeserializer<DomainEvent>().Deserialize(messageToDeserialized);

            Assert.True(logicalMessage.Body is EventCouldNotBeDeserialized);
            Assert.Equal("SomeWeirdEvent", ((EventCouldNotBeDeserialized)logicalMessage.Body).EventClassName);
        }

        [Fact]
        public void Deserialize_NoToposHeader_ShouldReturnEventCouldNotBeDeserialized()
        {
            // Create an event with no topos header information
            var position = new Position();
            var headers = new Dictionary<string, string>();
            var bodyJson = "{\"EventType\":\"RouteNodeAddedCommand\",\"EventTs\":\"2020-07-28T08:04:37.8098439Z\",\"CmdId\":\"e3693ffe-be4b-490b-9b1d-5c2883ea3849\",\"EventId\":\"351428bc-1606-44ff-b6e4-396664ccef9b\",\"NodeId\":\"b41a377c-ebe6-4327-9f08-ff58f70c2bb1\",\"Geometry\":\"[538888.8171487605,6210640.758167612]\"}";

            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);

            var messageToDeserialized = new ReceivedTransportMessage(position, headers, bodyBytes);

            var logicalMessage = new GenericEventDeserializer<DomainEvent>().Deserialize(messageToDeserialized);

            Assert.True(logicalMessage.Body is EventCouldNotBeDeserialized);
        }

      
    }
}
