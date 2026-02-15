using Azure.Iot.Operations.Mqtt.Session;
using Azure.Iot.Operations.Protocol.Connection;
using Azure.Iot.Operations.Protocol.Events;
using Azure.Iot.Operations.Protocol.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace aio_edge_heatbeat;
public class CounterConnectionWorker(MqttSessionClient sessionClient, ILogger<CounterConnectionWorker> logger) : BackgroundService
{
    private int _counter = 0;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        sessionClient.SessionLostAsync += OnCrash;
        sessionClient.ConnectedAsync += OnConnected;
        sessionClient.DisconnectedAsync += OnDisConnected;
        sessionClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        MqttConnectionSettings mcs = MqttConnectionSettings.FromEnvVars();

        await sessionClient.ConnectAsync(mcs, cancellationToken);

        var subscribe = new MqttClientSubscribeOptions("aio/heartbeat/todo", MqttQualityOfServiceLevel.AtLeastOnce);
        await sessionClient.SubscribeAsync(subscribe, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var counterMessage = new CounterMessage
            {
                DeviceId = "myDeviceId",
                Counter = _counter++,
                Timestamp = DateTime.UtcNow
            };
          
            var payload = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(counterMessage));

            var message = new MqttApplicationMessage("aio/heartbeat/message")
            {
                PayloadSegment = payload,
            };

            message.AddUserProperty("messageType", "counter");
            message.ContentType = "application/json";

            await sessionClient.PublishAsync(
                message,   
                cancellationToken);

            await Task.Delay(10000, cancellationToken);
        }

        await sessionClient.DisconnectAsync(null, cancellationToken);
    }

    private Task OnConnected(MqttClientConnectedEventArgs args)
    {
        logger.LogInformation($"Connected '{args.ConnectResult}'");
        return Task.CompletedTask;
    }

    private Task OnDisConnected(MqttClientDisconnectedEventArgs args)
    {
        logger.LogInformation($"Disconnected '{args.ClientWasConnected}' - '{args.ConnectResult}'");
        return Task.CompletedTask;
    }
    private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        logger.LogInformation("Received a message with topic {t} with payload {p}", args.ApplicationMessage.Topic, args.ApplicationMessage.ConvertPayloadToString());

        // You can also acknowledge a message manually later via the args.AcknowledgeAsync() API
        args.AutoAcknowledge = true;

        return Task.CompletedTask;
    }

    // This callback is only executed on fatal errors. Any non-fatal error will be handled by the 
    // session client instead of reporting it to the application layer to handle.
    private Task OnCrash(MqttClientDisconnectedEventArgs args)
    {
        logger.LogWarning("The session client encountered a fatal error and is no longer connected. {ex}", args.Exception);
        return Task.CompletedTask;
    }
}

public class CounterMessage
{
    public string DeviceId { get; set; } = "";
    public int Counter { get; set; }
    public DateTime Timestamp { get; set; }
}