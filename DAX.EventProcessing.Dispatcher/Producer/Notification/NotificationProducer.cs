using OpenFTTH.NotificationClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAX.EventProcessing;

public class NotificationProducer : IExternalEventProducer
{
    private readonly OpenFTTH.NotificationClient.Client _notificationClient;

    public NotificationProducer(string domain, int port)
    {
        var ipAddress = Dns.GetHostEntry(domain).AddressList
            .First(x => x.AddressFamily == AddressFamily.InterNetwork);

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
