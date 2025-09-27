namespace FrameBox.Core.Inbox.Enums;

public enum InboxState
{
    Pending,
    Running,
    Finished,
    Failed,
    ReadyToRetry,
    Retrying
}
