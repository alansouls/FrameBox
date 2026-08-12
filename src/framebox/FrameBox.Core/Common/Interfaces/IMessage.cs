namespace FrameBox.Core.Common.Interfaces;

public interface IMessage
{
    Guid Id { get; }

    Guid EventId { get; }

    //TODO: create IMessageSerializer
    byte[] ToJson();
}
