using Azure.Iot.Operations.Mqtt.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace aio_edge_heatbeat;
internal static class MqttSessionClientFactoryProvider
{
    public static Func<IServiceProvider, MqttSessionClient> MqttSessionClientFactory = service =>
    {
        IConfiguration? config = service.GetService<IConfiguration>();
        bool mqttDiag = config!.GetValue<bool>("mqttDiag");
        MqttSessionClientOptions sessionClientOptions = new()
        {
            EnableMqttLogging = mqttDiag,
        };

        if (mqttDiag)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        return new MqttSessionClient(sessionClientOptions);
    };
}