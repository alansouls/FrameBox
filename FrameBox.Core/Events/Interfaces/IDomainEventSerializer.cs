using Ardalis.Result;

namespace FrameBox.Core.Events.Interfaces;

public interface IDomainEventSerializer
{
    ValueTask<string> SerializeAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);

    ValueTask<Result<T>> DeserializeAsync<T>(string serializedDomainEvent, CancellationToken cancellationToken) where T : IDomainEvent;

    ValueTask<Result<IDomainEvent>> DeserializeAsync(string serializedDomainEvent, Type domainEventType, CancellationToken cancellationToken);
}
