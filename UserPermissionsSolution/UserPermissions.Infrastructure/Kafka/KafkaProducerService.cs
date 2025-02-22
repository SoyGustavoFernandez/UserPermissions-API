using Confluent.Kafka;

namespace UserPermissions.Infrastructure.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ProducerConfig _config;

        public KafkaProducerService(string bootstrapServers)
        {
            _config = new ProducerConfig { BootstrapServers = bootstrapServers };
        }

        public async Task SendMessageAsync(string topic, string message)
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                    Console.WriteLine($"Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}