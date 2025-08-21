namespace FrameBox.Core.Outbox.Options;

internal static class InternalOutboxOptions
{
    public static int MaxMessagesPerDispatch { get; set; } = 100;

    public static TimeSpan DispatchInterval { get; set; } = TimeSpan.FromSeconds(30);
}
