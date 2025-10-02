namespace FrameBox.Storage.EFCore.EventContexts.Models;

public class EventContextDbModel
{
    public Guid Id { get; set; }
    
    public IEnumerable<Guid> LinkedEvents { get; set; } = [];
    
    public string DataJson { get; set; } = string.Empty;
}