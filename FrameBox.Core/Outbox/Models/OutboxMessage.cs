using Ardalis.Result;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Outbox.Enums;

namespace FrameBox.Core.Outbox.Models;

public class OutboxMessage : IMessage
{
    private const string MessageStateMustBePending = "Message state must be pending.";
    private const string MessageStateMustBeSending = "Message state must be sending.";

    public Guid Id => EventId;
    public string Type => EventType;
    public Guid EventId { get; }
    public string EventType { get; }
    public string Payload { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public OutboxState State { get; private set; }
    public Guid? ProcessId { get; private set; }

    public OutboxMessage(Guid eventId, string eventType, string payload, DateTimeOffset createdAt)
    {
        EventId = eventId;
        EventType = eventType;
        Payload = payload;

        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        State = OutboxState.Pending;
    }

    public Result SetAsSending(DateTimeOffset timeStamp)
    {
        if (State != OutboxState.Pending)
        {
            return Result.Error(MessageStateMustBePending);
        }

        State = OutboxState.Pending;
        UpdatedAt = timeStamp;

        return Result.Success();
    }

    public Result SetAsPending(DateTimeOffset timeStamp)
    {
        if (State != OutboxState.Sending)
        {
            return Result.Error(MessageStateMustBeSending);
        }

        State = OutboxState.Pending;
        UpdatedAt = timeStamp;
        return Result.Success();
    }

    public Result SetAsSent(DateTimeOffset timeStamp)
    {
        if (State != OutboxState.Sending)
        {
            return Result.Error(MessageStateMustBeSending);
        }

        State = OutboxState.Sent;
        UpdatedAt = timeStamp;
        return Result.Success();
    }
}
