using FrameBox.Core.Events.Defaults;

namespace InboxOutboxSample.Shared.Domain;

public record NonDomainEvent(string Hello) : FrameEvent;