namespace FrameBox.Core.Inbox.Options;

internal static class InternalInboxOptions
{
    public static int MaxBatchCountToRetry { get; set; } = 100;

    public static TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(30);

    public static int MaxRetryCount { get; set; } = 3;

    public static TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(60);

    public static TimeSpan TimeoutCheckerFrequency { get; set; } = TimeSpan.FromMinutes(1);

    public static int MaxBatchCountToTimeout { get; set; } = 1000;

    public static TimeSpan CleanupFrequency { get; set; } = TimeSpan.FromHours(1);

    public static int RetentionPeriodInDays { get; set; } = 7;

    public static int MaxBatchCountToCleanup { get; set; } = 1000;
}
