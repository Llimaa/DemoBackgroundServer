using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace DemoBackgroundServer;

public class DemoServiceBus : BackgroundService
{
    private ServiceBusClient client;
    private readonly ILogger<DemoServiceBus> _logger;
    private readonly IConfiguration configuration;

    public DemoServiceBus(ILogger<DemoServiceBus> logger, IConfiguration configuration)
    {
        this.configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = configuration.GetSection("connection").Value;
        client = new ServiceBusClient(connection);

        var count = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var receiver = client.CreateReceiver("notification-queue");
            var message = await receiver.ReceiveMessageAsync();
            try
            {
                var body = message.Body.ToString();
                var data = JsonConvert.DeserializeObject<dynamic>(body);
                count ++;

                await receiver.CompleteMessageAsync(message);
            }
            catch (System.Exception)
            {
                await receiver.DeadLetterMessageAsync(message);
            }
        }
    }
}
