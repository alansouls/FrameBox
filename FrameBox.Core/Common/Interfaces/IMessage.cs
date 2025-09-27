namespace FrameBox.Core.Common.Interfaces;

public interface IMessage
{
    Guid Id { get; }

    //TODO: create IMessageSerializer
    byte[] ToJson();
}
