using DemoBackgroundServer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DemoServiceBus>();
    })
    .Build();

host.Run();
