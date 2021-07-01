using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaProducer
{
    public class Program
    {
        public static async Task Main()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093"
                
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            producer.Produce("hello-world-2", new Message<Null, string>(){Value = "Hello world"});
            producer.Flush(new TimeSpan(0, 0, 5));
        }

        public class KafkaMessage
        {
            public string Text { get; set;  }
        }
        }
}
