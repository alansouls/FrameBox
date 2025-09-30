using Ardalis.Result;

namespace FrameBox.Core.Events.Interfaces;

public interface IDomainEventSerializer
{
    ValueTask<string> SerializeAsync(IEvent domainEvent, CancellationToken cancellationToken);

    ValueTask<Result<T>> DeserializeAsync<T>(string serializedDomainEvent, CancellationToken cancellationToken) where T : IEvent;

    ValueTask<Result<IEvent>> DeserializeAsync(string serializedDomainEvent, Type domainEventType, CancellationToken cancellationToken);
}
