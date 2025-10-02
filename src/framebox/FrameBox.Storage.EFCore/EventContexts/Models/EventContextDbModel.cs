namespace FrameBox.Storage.EFCore.EventContexts.Models;

public class EventContextDbModel
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public List<EventContextEventLink> LinkedEvents { get; set; } = [];
    
    public string DataJson { get; set; } = string.Empty;
}

public class EventContextEventLink
{
    public Guid EventContextId { get; set; }

    public Guid EventId { get; set; }
}