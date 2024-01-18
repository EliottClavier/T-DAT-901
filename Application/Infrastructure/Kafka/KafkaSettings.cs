using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string DefaultTopic { get; set; }

        public string TransactionTopic { get; set; }
    }
}
