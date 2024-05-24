using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Helper
{
    public class KafkaProducerConfig
    {
        public static ProducerConfig GetProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };
        }
    }
}
