namespace FrameBox.Core.Outbox.Enums;

public enum OutboxState
{
    Pending,
    Sending,
    Failed,
    Sent,
    ReadyToRetry
}
