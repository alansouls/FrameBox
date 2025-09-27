using Ardalis.Result;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Enums;
using FrameBox.Core.Inbox.Options;
using System.Text.Json;

namespace FrameBox.Core.Inbox.Models;

public class InboxMessage : IMessage
{
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private record InboxMessageData(Guid Id, Guid OutboxMessageId, string HandlerName, string? FailurePayload, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, InboxState State, int RetryCount, Guid? ProcessId);

    public Guid Id { get; }
    public Guid OutboxMessageId { get; }
    public string HandlerName { get; }
    public string? FailurePayload { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public InboxState State { get; private set; }
    public int RetryCount { get; private set; }
    public Guid? ProcessId { get; private set; }

    private InboxMessage(InboxMessageData data)
    {
        Id = data.Id;
        OutboxMessageId = data.OutboxMessageId;
        HandlerName = data.HandlerName;
        FailurePayload = data.FailurePayload;
        CreatedAt = data.CreatedAt;
        UpdatedAt = data.UpdatedAt;
        State = data.State;
        RetryCount = data.RetryCount;
        ProcessId = data.ProcessId;
    }

    public static InboxMessage FromJson(ReadOnlySpan<byte> utf8Data)
    {
        var data = JsonSerializer.Deserialize<InboxMessageData>(utf8Data, _serializerOptions) ?? throw new InvalidOperationException("Failed to deserialize InboxMessageData");

        return new InboxMessage(data);
    }

    public byte[] ToJson()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this, _serializerOptions);
    }

    public InboxMessage(Guid id, Guid outboxMessageId, string handlerName, DateTimeOffset createdAt)
    {
        Id = id;
        OutboxMessageId = outboxMessageId;
        HandlerName = handlerName;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        State = InboxState.Pending;
        RetryCount = 0;
        ProcessId = null;
    }

    public Result Start(DateTimeOffset timeStamp)
    {
        if (State != InboxState.Pending && State != InboxState.Retrying)
        {
            return Result.Error("Message state must be pending or retrying.");
        }

        if (State == InboxState.Retrying)
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
