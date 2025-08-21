using FrameBox.Core.Common.Interfaces;

namespace FrameBox.Core.Common.Exceptions;

public class FailedToSendMessagesException<T> : Exception where T : IMessage
{
    public IReadOnlyList<T> FailedMessages { get; }

    public FailedToSendMessagesException(string message, IEnumerable<T> failedMessages, Exception? innerException = null)
        : base(message, innerException)
    {
        FailedMessages = failedMessages.ToList();
    }

    public FailedToSendMessagesException(IEnumerable<T> failedMessages, Exception? innerException = null) 
        : this("Failed to send messages", failedMessages, innerException)
    {
    }
}
