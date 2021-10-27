using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TransfersService.Integration
{
    class KafkaListenerService : IHostedService
    {
        private readonly ILogger<KafkaListenerService> _logger;

        public KafkaListenerService(ILogger<KafkaListenerService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting kafka listener");

            var conf = new ConsumerConfig
            {
                GroupId = Guid.NewGuid().ToString(),
                BootstrapServers = "localhost:9093",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore,
                string>(conf).Build();

            consumer.Subscribe("user-service-events");
         
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);
                    if (result != null)
                    {
                        _logger.LogInformation($"Message received from {result.TopicPartitionOffset}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unexpected error: {e}");
                    break;
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
