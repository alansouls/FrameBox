using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Options;

public class RabbitMQOptions
{
    public string OutboxExchangeName { get; set; } = "outbox_exchange";
}
