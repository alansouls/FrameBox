# FrameBox.MessageBroker.RabbitMQ

## Purpose

FrameBox.MessageBroker.RabbitMQ provides RabbitMQ implementation for the FrameBox framework's message broker abstraction. This library enables reliable message publishing and consumption using RabbitMQ as the underlying message broker, with support for outbox pattern integration and message listening.

## Key Features

- **RabbitMQ Integration**: Native RabbitMQ client implementation for message publishing and consumption
- **Outbox Integration**: Seamless integration with FrameBox.Core outbox pattern
- **Message Listening**: Background services for consuming messages from RabbitMQ queues
- **Configuration Support**: Flexible configuration through .NET configuration system
- **Connection Management**: Automatic connection handling and resilience

## How to Use

### 1. Install the Package

Add the FrameBox.MessageBroker.RabbitMQ package to your project:

```xml
<PackageReference Include="FrameBox.MessageBroker.RabbitMQ" />
```

### 2. Configure RabbitMQ Connection

Add RabbitMQ connection configuration to your `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  }
}
```

### 3. Register Services

Register RabbitMQ services in your DI container:

```csharp
using FrameBox.MessageBroker.RabbitMQ.Common.Extensions;

// In Program.cs
builder.Services.AddRabbitMQMessageBroker(builder.Configuration);
builder.Services.AddRabbitMQListener(builder.Configuration);

// For .NET Aspire integration
builder.AddRabbitMQClient("rabbitmq");
```

### 4. Publishing Messages

Messages are automatically published through the outbox pattern when using FrameBox.Core:

```csharp
// Messages saved to outbox are automatically published to RabbitMQ
await context.SaveChangesAsync(); // Triggers outbox processing
```

### 5. Consuming Messages

The RabbitMQ listener automatically processes messages and routes them to registered event handlers.

## Configuration Options

The library supports various RabbitMQ configuration options:

- **Connection Settings**: Host, port, credentials, virtual host
- **Queue Configuration**: Queue names, durability, auto-delete settings  
- **Exchange Configuration**: Exchange names, types, routing keys
- **Consumer Settings**: Prefetch count, auto-acknowledgment, retry policies

## Coding Standards

### C# Conventions
- Use C# 9.0+ features including nullable reference types
- Follow standard .NET naming conventions (PascalCase for public members, camelCase for private)
- Use `var` for local variables when the type is obvious
- Implement proper async/await patterns with CancellationToken support

### RabbitMQ Best Practices
- Use durable queues and persistent messages for reliability
- Implement proper error handling and dead letter queues
- Use connection pooling and channel management
- Handle connection failures gracefully with retry logic

### Architecture Patterns
- Follow dependency injection patterns
- Use configuration objects for RabbitMQ settings
- Implement proper dispose patterns for connections and channels
- Use background services for message consumption

### Code Organization
- Group functionality in Common folder with Extensions, Options, and Services subfolders
- Use extension methods for service registration
- Keep configuration classes separate from implementation
- Use descriptive names for queues, exchanges, and routing keys

### Error Handling
- Log all connection and message processing errors
- Implement circuit breaker patterns for connection resilience  
- Use structured exceptions with RabbitMQ-specific context
- Handle message acknowledgment properly to avoid message loss