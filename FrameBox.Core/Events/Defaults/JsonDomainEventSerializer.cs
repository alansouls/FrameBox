using Ardalis.Result;
using FrameBox.Core.Events.Interfaces;
using System.Text.Json;

namespace FrameBox.Core.Events.Defaults;

internal class JsonDomainEventSerializer : IDomainEventSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public ValueTask<Result<T>> DeserializeAsync<T>(string serializedDomainEvent, CancellationToken cancellationToken) where T : IDomainEvent
    {
        if (string.IsNullOrWhiteSpace(serializedDomainEvent))
        {
            return ValueTask.FromResult<Result<T>>(Result.Error("Serialized domain event cannot be null or empty."));
        }
        try
        {
            var domainEvent = JsonSerializer.Deserialize<T>(serializedDomainEvent, _serializerOptions);

            if (domainEvent == null)
            {
                return ValueTask.FromResult<Result<T>>(Result.CriticalError("Deserialized domain event is null."));
            }

            return ValueTask.FromResult(Result.Success(domainEvent));
        }
        catch (JsonException ex)
        {
            return ValueTask.FromResult<Result<T>>(Result.Error($"Failed to deserialize domain event: {ex.Message}"));
        }
    }

    public ValueTask<Result<IDomainEvent>> DeserializeAsync(string serializedDomainEvent, Type domainEventType, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(serializedDomainEvent))
        {
            return ValueTask.FromResult<Result<IDomainEvent>>(Result.Error("Serialized domain event cannot be null or empty."));
        }
        try
        {
            var domainEvent = JsonSerializer.Deserialize(serializedDomainEvent, domainEventType, _serializerOptions);

            if (domainEvent == null)
            {
                return ValueTask.FromResult<Result<IDomainEvent>>(Result.CriticalError("Deserialized domain event is null."));
            }

            return ValueTask.FromResult(Result.Success((domainEvent as IDomainEvent)!));
        }
        catch (JsonException ex)
        {
            return ValueTask.FromResult<Result<IDomainEvent>>(Result.Error($"Failed to deserialize domain event: {ex.Message}"));
        }
    }

    public ValueTask<string> SerializeAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent == null)
        {
            throw new ArgumentNullException(nameof(domainEvent), "Domain event cannot be null.");
        }

        var json = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _serializerOptions);
        return ValueTask.FromResult(json);
    }
}
