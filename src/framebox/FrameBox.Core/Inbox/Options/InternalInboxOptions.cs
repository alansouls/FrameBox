namespace FrameBox.Core.Inbox.Options;


public class InboxOptions
{
    public int? MaxBatchCountToRetry { get; set; }
    public TimeSpan? RetryInterval { get; set; }
    public int? MaxRetryCount { get; set; }
    public TimeSpan? Timeout { get; set; }
    public TimeSpan? TimeoutCheckerFrequency { get; set; }
    public int? MaxBatchCountToTimeout { get; set; }
    public TimeSpan? CleanupFrequency { get; set; }
    public int? RetentionPeriodInDays { get; set; }
    public int? MaxBatchCountToCleanup { get; set; }

}

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

    public static void ApplyOptions(InboxOptions options)
    {
        if (options.MaxBatchCountToRetry.HasValue)
        {
            MaxBatchCountToRetry = options.MaxBatchCountToRetry.Value;
        }
        if (options.RetryInterval.HasValue)
        {
            RetryInterval = options.RetryInterval.Value;
        }
        if (options.MaxRetryCount.HasValue)
        {
            MaxRetryCount = options.MaxRetryCount.Value;
        }
        if (options.Timeout.HasValue)
        {
            Timeout = options.Timeout.Value;
        }
        if (options.TimeoutCheckerFrequency.HasValue)
        {
            TimeoutCheckerFrequency = options.TimeoutCheckerFrequency.Value;
        }
        if (options.MaxBatchCountToTimeout.HasValue)
        {
            MaxBatchCountToTimeout = options.MaxBatchCountToTimeout.Value;
        }
        if (options.CleanupFrequency.HasValue)
        {
            CleanupFrequency = options.CleanupFrequency.Value;
        }
        if (options.RetentionPeriodInDays.HasValue)
        {
            RetentionPeriodInDays = options.RetentionPeriodInDays.Value;
        }
        if (options.MaxBatchCountToCleanup.HasValue)
        {
            MaxBatchCountToCleanup = options.MaxBatchCountToCleanup.Value;
        }
    }
}
