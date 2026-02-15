using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using aio_edge_heatbeat;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(static services =>
    {
        services.AddSingleton(MqttSessionClientFactoryProvider.MqttSessionClientFactory);
        services.AddHostedService<CounterConnectionWorker>();
    })
    .Build();

host.Run();
