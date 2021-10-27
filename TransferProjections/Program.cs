using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using TransfersService.Events;

namespace TransferProjections
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

            using var consumer = new ConsumerBuilder<Ignore,
                ISerializableTransferEvent>(conf).Build();

            var cancelToken = new CancellationTokenSource();

            StartConsumerLoop(consumer, "hello-world-2", cancelToken.Token);

            Console.ReadLine();
            cancelToken.Cancel();
        }


        private static void StartConsumerLoop(IConsumer<Ignore, ISerializableTransferEvent> consumer, string topic, CancellationToken cancellationToken)
        {
            consumer.Subscribe(topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);
                    switch (result)
                    {
                        
                    }
                    if (result != null)
                    {
                        Console.WriteLine($"Message received from {result.TopicPartitionOffset}");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }
    }
}
