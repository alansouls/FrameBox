namespace FrameBox.Core.Common.Interfaces;

public interface IMessage
{
    Guid Id { get; }
    string Type { get; }
    string Payload { get; }
}
