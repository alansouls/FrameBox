# OutboxInboxExample

## Purpose

OutboxInboxExample is a simple console application that demonstrates the basic concepts of the FrameBox framework's Outbox and Inbox patterns. It provides a minimal, easy-to-understand example of how to implement reliable message processing in a distributed system, perfect for learning the core concepts before moving to more complex scenarios.

## Key Features

- **Console Application**: Simple command-line interface for easy learning
- **Basic Outbox Pattern**: Demonstrates reliable message publishing
- **Basic Inbox Pattern**: Shows idempotent message processing
- **Minimal Dependencies**: Focuses on core concepts without complex infrastructure
- **Educational Example**: Step-by-step demonstration of pattern implementation

## How to Use

### 1. Running the Example

```bash
cd OutboxInboxExample
dotnet run
```

### 2. Example Flow

The console application demonstrates:

1. **Message Creation**: Creating business entities that generate domain events
2. **Outbox Storage**: Storing events in an outbox for reliable publishing
3. **Message Publishing**: Processing outbox messages and publishing them
4. **Inbox Processing**: Receiving messages and processing them idempotently
5. **Duplicate Handling**: Demonstrating how duplicates are handled

### 3. Example Output

```
=== FrameBox Outbox/Inbox Example ===

Creating payment with ID: 12345678-1234-1234-1234-123456789012
- Payment stored in database
- PaymentCreated event stored in outbox

Processing outbox messages...
- Found 1 message in outbox
- Publishing PaymentCreated event
- Message marked as processed

Simulating message receipt...
- Received PaymentCreated event
- Processing through inbox pattern
- Event processed successfully

Simulating duplicate message...
- Received PaymentCreated event (duplicate)
- Duplicate detected in inbox
- Message skipped (idempotent processing)

Example completed successfully!
```

## Example Components

### Domain Model

```csharp
public class Payment
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Payment(decimal amount)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
}

public record PaymentCreated(Guid PaymentId, decimal Amount, DateTime CreatedAt);
```

### Outbox Implementation

```csharp
public class SimpleOutboxService
{
    private readonly List<OutboxMessage> _messages = new();

    public void AddMessage(object domainEvent)
    {
        var message = new OutboxMessage(
            Id: Guid.NewGuid(),
            EventType: domainEvent.GetType().Name,
            EventData: JsonSerializer.Serialize(domainEvent),
            CreatedAt: DateTime.UtcNow
        );
        
        _messages.Add(message);
    }

    public async Task ProcessMessages()
    {
        foreach (var message in _messages.Where(m => !m.IsProcessed))
        {
            // Simulate message publishing
            Console.WriteLine($"Publishing {message.EventType} event");
            
            // Mark as processed
            message.MarkAsProcessed();
        }
    }
}
```

### Inbox Implementation

```csharp
public class SimpleInboxService
{
    private readonly HashSet<string> _processedMessages = new();

    public async Task<bool> ProcessMessage(string messageId, object domainEvent)
    {
        if (_processedMessages.Contains(messageId))
        {
            Console.WriteLine("Duplicate message detected - skipping");
            return false;
        }

        // Process the message
        Console.WriteLine($"Processing {domainEvent.GetType().Name}");
        
        // Simulate business logic
        await Task.Delay(100);
        
        // Mark as processed
        _processedMessages.Add(messageId);
        return true;
    }
}
```

## Learning Objectives

### Understanding Outbox Pattern
- Why we need reliable message publishing
- How to store events with business data in the same transaction
- Background processing of outbox messages
- Handling publishing failures and retries

### Understanding Inbox Pattern
- Achieving idempotent message processing
- Detecting and handling duplicate messages
- Maintaining processing history
- Error handling in message processing

### Distributed System Concepts
- Eventual consistency
- At-least-once delivery guarantees
- Transactional boundaries
- Message ordering considerations

## Next Steps

After understanding this basic example:

1. **Complex Example**: Explore the full InboxOutboxSample with ASP.NET Core, Entity Framework, and RabbitMQ
2. **Real Databases**: Implement with actual database storage instead of in-memory collections
3. **Message Brokers**: Add real message broker integration (RabbitMQ, Azure Service Bus, etc.)
4. **Error Handling**: Implement retry policies, dead letter queues, and comprehensive error handling
5. **Monitoring**: Add logging, metrics, and health checks

## Coding Standards

### Simplicity First
- Keep examples minimal and focused
- Use clear, descriptive variable names
- Include plenty of comments explaining the concepts
- Avoid unnecessary abstractions

### Educational Value
- Show the problem before showing the solution
- Demonstrate both success and failure scenarios
- Include console output to show what's happening
- Use realistic but simple domain examples

### Code Structure
- Organize code in logical sections (Domain, Outbox, Inbox, Main)
- Use meaningful method names that describe what they do
- Keep methods small and focused on single responsibilities
- Include summary comments for all public members

### Error Handling
- Show basic error handling patterns
- Demonstrate graceful degradation
- Include logging for observability
- Keep error handling simple and understandable

### Best Practices
- Use async/await patterns appropriately
- Include cancellation token support where relevant
- Follow standard C# naming conventions
- Use modern C# language features (records, nullable reference types)