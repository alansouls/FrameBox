# FrameBox.Core

## Purpose

FrameBox.Core is the foundational library of the FrameBox framework, providing core functionality for implementing reliable message processing patterns including Outbox and Inbox patterns. This library offers event handling, message dispatching, and the core abstractions needed for building robust distributed systems.

## Key Features

- **Outbox Pattern**: Ensures reliable message publishing by storing outbound messages in the same database transaction as business data
- **Inbox Pattern**: Provides idempotent message processing to handle duplicate messages gracefully  
- **Event Handling**: Generic event handler infrastructure with dependency injection support
- **Core Abstractions**: Interfaces and base classes for implementing message processing patterns

## How to Use

### 1. Install the Package

Add the FrameBox.Core package to your project:

```xml
<PackageReference Include="FrameBox.Core" />
```

### 2. Register Services

Register FrameBox.Core services in your DI container:

```csharp
using FrameBox.Core.Extensions;

// In Program.cs or Startup.cs
builder.Services.AddFrameBoxCore();
```

### 3. Add Event Handlers

Register event handlers from your assembly:

```csharp
builder.Services.AddEventHandlersFromAssemblyContaining<MyEventHandler>();
```

### 4. Implement Event Handlers

Create event handlers by implementing `IEventHandler<T>`:

```csharp
public class PaymentCreatedHandler : IEventHandler<PaymentCreated>
{
    public Task Handle(PaymentCreated @event, CancellationToken cancellationToken)
    {
        // Handle the event
        return Task.CompletedTask;
    }
}
```

## Coding Standards

### C# Conventions
- Use C# 9.0+ features including nullable reference types
- Follow standard .NET naming conventions (PascalCase for public members, camelCase for private)
- Use `var` for local variables when the type is obvious
- Prefer explicit interface implementations for core abstractions

### Architecture Patterns
- Follow SOLID principles
- Use dependency injection for all service dependencies
- Implement proper async/await patterns with CancellationToken support
- Use Result patterns for operations that can fail (via Ardalis.Result)

### Code Organization
- Group related functionality in dedicated folders (Outbox, Inbox, Events, Common)
- Use extension methods for service registration
- Keep interfaces in separate files from implementations
- Use descriptive namespaces that reflect the folder structure

### Error Handling
- Use structured exceptions with meaningful messages
- Log errors with appropriate context
- Handle cancellation tokens properly in async operations
- Validate input parameters and throw appropriate exceptions