using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaba.Infrastructure.Kafka.TopologySetup
{
    public class KafkaTopologySetup
    {
        private readonly string _bootstrapServers;

        public KafkaTopologySetup(KafkaConfig kafkaConfig)
        {
            _bootstrapServers = bootstrapServers;
        }

        public async Task EnsureTopicsExistAsync(IEnumerable<string> topicNames, int numPartitions = 3, short replicationFactor = 1)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers })
                .Build();

            try
            {
                // Get the list of existing topics
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                var existingTopics = new HashSet<string>(metadata.Topics.Select(t => t.Topic));

                var topicsToCreate = topicNames
                    .Where(topic => !existingTopics.Contains(topic))
                    .Select(topic => new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = numPartitions,
                        ReplicationFactor = replicationFactor
                    })
                    .ToList();

                if (topicsToCreate.Count > 0)
                {
                    await adminClient.CreateTopicsAsync(topicsToCreate);
                    Console.WriteLine($"Created topics: {string.Join(", ", topicsToCreate.Select(t => t.Name))}");
                }
                else
                {
                    Console.WriteLine("All topics already exist.");
                }
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"Error creating topics: {e.Results[0].Error.Reason}");
                throw;
            }
        }
    }
}
