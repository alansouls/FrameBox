namespace FrameBox.Core.Inbox.Options;

internal static class InternalInboxOptions
{
    public static int MaxInboxMessagesToProcess { get; set; } = 100;
    public static int MaxInboxMessagesBatch{ get; set; } = 5;
    public static int MaxInboxRetryCount { get; set; } = 3;
}
