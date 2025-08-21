using Ardalis.Result;
using FrameBox.Core.Inbox.Enums;
using FrameBox.Core.Inbox.Options;

namespace FrameBox.Core.Inbox.Models;

public class InboxMessage
{
    public Guid Id { get; }
    public Guid OutboxMessageId { get; }
    public string HandlerName { get; }
    public string? FailurePayload { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public InboxState State { get; private set; }
    public int RetryCount { get; private set; }

    public InboxMessage(Guid id, Guid outboxMessageId, string handlerName, DateTimeOffset createdAt)
    {
        Id = id;
        OutboxMessageId = outboxMessageId;
        HandlerName = handlerName;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        State = InboxState.Pending;
        RetryCount = 0;
    }

    public Result Start(DateTimeOffset timeStamp)
    {
        if (State != InboxState.Pending && State != InboxState.ReadyToRetry)
        {
            return Result.Error("Message state must be pending or ready to retry.");
        }

        if (State == InboxState.ReadyToRetry)
        {
            RetryCount++;
        }

        State = InboxState.Running;
        UpdatedAt = timeStamp;
        return Result.Success();
    }

    public Result Complete(DateTimeOffset timeStamp)
    {
        if (State != InboxState.Running)
        {
            return Result.Error("Message state must be running.");
        }

        State = InboxState.Finished;
        UpdatedAt = timeStamp;
        return Result.Success();
    }

    public Result Fail(string failurePayload, DateTimeOffset timeStamp)
    {
        if (State != InboxState.Running)
        {
            return Result.Error("Message state must be running.");
        }

        FailurePayload = failurePayload;
        State = RetryCount < InternalInboxOptions.MaxInboxRetryCount ? InboxState.ReadyToRetry : InboxState.Failed;
        UpdatedAt = timeStamp;
        return Result.Success();
    }
}
