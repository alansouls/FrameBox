using Ardalis.Result;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Enums;
using FrameBox.Core.Inbox.Options;
using System.Text.Json;
using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Inbox.Models;

public class InboxMessage : IMessage
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private record InboxMessageData(
        Guid Id,
        Guid EventId,
        string EventName,
        string EventPayload,
        string HandlerName,
        string? FailurePayload,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt,
        InboxState State,
        int RetryCount,
        Guid? ProcessId);

    public Guid Id { get; }
    public Guid EventId { get; }

    public string EventName { get; }

    public string EventPayload { get; }

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
        EventId = data.EventId;
        EventName = data.EventName;
        EventPayload = data.EventPayload;
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
        var data = JsonSerializer.Deserialize<InboxMessageData>(utf8Data, SerializerOptions) ??
                   throw new InvalidOperationException("Failed to deserialize InboxMessageData");

        return new InboxMessage(data);
    }

    public byte[] ToJson()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this, SerializerOptions);
    }

    public InboxMessage(Guid id, Guid eventId, string eventName, string eventPayload, string handlerName, DateTimeOffset createdAt)
    {
        Id = id;
        EventId = eventId;
        EventName = eventName;
        EventPayload = eventPayload;
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
        if (State == InboxState.Finished)
        {
            return Result.Error("Message state cannot be finished.");
        }

        FailurePayload = failurePayload;
        State = RetryCount < InternalInboxOptions.MaxRetryCount ? InboxState.ReadyToRetry : InboxState.Failed;
        UpdatedAt = timeStamp;
        return Result.Success();
    }

    public async Task<IEvent> GetEventAsync(IDomainEventSerializer eventSerializer, IEventRegistry eventRegistry, CancellationToken cancellationToken)
    {
        var eventType = eventRegistry.GetEventType(EventName);
        
        if (eventType is null)
        {
            throw new InvalidOperationException($"Event type '{EventName}' is not registered.");
        }
        
        var domainEvent = await eventSerializer.DeserializeAsync(EventPayload, eventType, cancellationToken);
        
        if (!domainEvent.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to deserialize event of type '{EventName}'.");
        }
        
        return domainEvent.Value;
    }
}