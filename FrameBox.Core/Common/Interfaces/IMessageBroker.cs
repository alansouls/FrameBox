namespace FrameBox.Core.Common.Interfaces;

public interface IMessageBroker
{
    Task SendMessagesAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken) where T : class, IMessage;
}
