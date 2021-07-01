using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KafkaConsumer
{

    public class Startup
    {
        public static async Task Main()
        {
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group-2",
                BootstrapServers = "localhost:9093",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var builder = new ConsumerBuilder<Ignore,
                string>(conf).Build())
            {
                builder.Subscribe("hello-world-2");
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancelToken.Token);
                        Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }

            Console.ReadLine();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransitHostedService();
        }
    }
}
