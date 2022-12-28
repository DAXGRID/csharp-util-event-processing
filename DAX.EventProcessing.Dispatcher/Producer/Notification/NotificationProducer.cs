using OpenFTTH.NotificationClient;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAX.EventProcessing;

public class NotificationProducer : IExternalEventProducer
{
    private readonly OpenFTTH.NotificationClient.Client _notificationClient;

    public NotificationProducer(IPAddress ipAddress, int port)
    {
        _notificationClient = new(ipAddress, port, true);
        _notificationClient.Connect();
    }

    public Task Produce(string name, object message)
    {
        var notification = new Notification(
            name,
            JsonSerializer.Serialize(message));

        _notificationClient.Send(notification);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _notificationClient.Dispose();
    }
}
