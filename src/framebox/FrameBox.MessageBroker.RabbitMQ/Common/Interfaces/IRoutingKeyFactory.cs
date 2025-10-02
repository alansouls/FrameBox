using FrameBox.Core.Common.Interfaces;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Interfaces;

public interface IRoutingKeyFactory<T> where T : class, IMessage
{
    string CreateRoutingKey(T message);
}