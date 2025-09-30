using Ardalis.Result;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Outbox.Enums;
using FrameBox.Core.Outbox.Options;
using System.Text.Json;

namespace FrameBox.Core.Outbox.Models;

public class OutboxMessage : IMessage
{
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private record OutboxMessageData(Guid EventId, string EventType, string Payload, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, OutboxState State, Guid? ProcessId);

    private const string MessageStateMustBeFailed = "Message state must be failed.";
    private const string MessageStateMustBeSent = "Message state must be sent.";
    private const string MaxRetryCountReached = "Max retry count reached.";

    public Guid Id => EventId;
    public string Type => EventType;
    public Guid EventId { get; }
    public string EventType { get; }
    public string Payload { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public OutboxState State { get; private set; }
    public int RetryCount { get; private set; }
    public Guid? ProcessId { get; private set; }

    private OutboxMessage(OutboxMessageData data)
    {
        EventId = data.EventId;
        EventType = data.EventType;
        Payload = data.Payload;
        CreatedAt = data.CreatedAt;
        UpdatedAt = data.UpdatedAt;
        State = data.State;
        ProcessId = data.ProcessId;
    }

    public static OutboxMessage FromJson(ReadOnlySpan<byte> utf8Data)
    {
        var data = JsonSerializer.Deserialize<OutboxMessageData>(utf8Data, _serializerOptions) ?? throw new InvalidOperationException("Failed to deserialize OutboxMessageData");

        return new OutboxMessage(data);
    }

    public byte[] ToJson()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this, _serializerOptions);
    }

    public OutboxMessage(Guid eventId, string eventType, string payload, DateTimeOffset createdAt)
    {
        EventId = eventId;
        EventType = eventType;
        Payload = payload;

        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        State = OutboxState.Sent;
    }

    public Result SetAsSent(DateTimeOffset timeStamp)
    {
        if (State != OutboxState.Failed)
        {
            return Result.Error(MessageStateMustBeFailed);
        }

        if (RetryCount >= InternalOutboxOptions.MaxRetryCount)
        {
            return Result.Error(MaxRetryCountReached);
        }

        State = OutboxState.Sent;
        UpdatedAt = timeStamp;
        RetryCount++;

        return Result.Success();
    }

    public Result SetAsReceived(DateTimeOffset timeStamp)
    {
        if (State != OutboxState.Sent)
        {
            return Result.Error(MessageStateMustBeSent);
        }

        State = OutboxState.Received;
        UpdatedAt = timeStamp;

        return Result.Success();
    }

    public Result SetAsFailed(DateTimeOffset timeStamp)
    {
        if (State != OutboxState.Sent)
        {
            return Result.Error(MessageStateMustBeSent);
        }

        State = OutboxState.Failed;
        UpdatedAt = timeStamp;

        return Result.Success();
    }

    public async Task<IEvent> ToDomainEvent(IDomainEventSerializer domainEventSerializer, CancellationToken cancellationToken)
    {
        var eventType = System.Type.GetType(EventType) ?? throw new InvalidOperationException("Type not found!");
        var result = await domainEventSerializer.DeserializeAsync(Payload, eventType, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw new InvalidOperationException($"Failed to deserialize domain event of type {EventType}: {string.Join(", ", result.Errors)}");
    }
}
