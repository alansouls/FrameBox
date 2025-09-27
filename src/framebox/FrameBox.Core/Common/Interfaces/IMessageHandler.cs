namespace FrameBox.Core.Common.Interfaces;

public interface IMessageHandler<TMessage> where TMessage : IMessage
{
    Task HandleMessage(TMessage message, CancellationToken cancellationToken);
}
