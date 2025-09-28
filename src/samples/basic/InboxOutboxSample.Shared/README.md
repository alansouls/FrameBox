# InboxOutboxSample.Shared

## Purpose

InboxOutboxSample.Shared is a shared library that contains common code, data models, event handlers, and utilities used across multiple projects in the InboxOutboxSample application. It promotes code reuse, maintains consistency, and ensures that shared domain concepts are defined in a single location.

## Key Features

- **Shared Data Models**: Common DTOs, entities, and value objects
- **Event Handlers**: FrameBox event handlers for domain events
- **Domain Events**: Event definitions for the payment domain
- **Common Utilities**: Shared helper classes and extensions
- **Database Context**: Shared Entity Framework DbContext configuration

## How to Use

### 1. Reference the Shared Library

Add a project reference in dependent projects:

```xml
<ProjectReference Include="../InboxOutboxSample.Shared/InboxOutboxSample.Shared.csproj" />
```

### 2. Data Models

Use shared data models across services:

```csharp
using InboxOutboxSample.Shared.Models;

public class PaymentService
{
    public async Task<Payment> CreatePayment(CreatePaymentRequest request)
    {
        var payment = new Payment(request.Amount);
        // Business logic
        return payment;
    }
}
```

### 3. Event Handlers

Register shared event handlers:

```csharp
using InboxOutboxSample.Shared.Handlers.Extensions;

// In service startup
builder.Services.AddHandlers(); // Registers all event handlers from this assembly
```

### 4. Database Context

Use the shared DbContext:

```csharp
using InboxOutboxSample.Shared.Data;

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
```

## Shared Components

### Data Models

#### Domain Entities
- **Payment**: Core payment entity with business logic
- **PaymentEvent**: Domain events related to payments
- **AuditLog**: System audit trail entities

#### DTOs and Requests
- **CreatePaymentRequest**: Payment creation request model
- **PaymentResponse**: Payment API response model
- **PaymentSummary**: Dashboard summary model

#### Value Objects
- **Money**: Monetary value with currency
- **PaymentStatus**: Enumeration of payment states
- **AuditInfo**: Audit trail information

### Event Handlers

#### Domain Event Handlers
- **PaymentCreatedHandler**: Handles payment creation events
- **PaymentProcessedHandler**: Handles payment processing events
- **PaymentFailedHandler**: Handles payment failure events

#### Integration Event Handlers
- **ExternalPaymentHandler**: Handles events from external systems
- **NotificationHandler**: Handles user notification events
- **AuditHandler**: Handles audit logging events

### Database Configuration

#### DbContext Setup
```csharp
public class MyDbContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentEvent> PaymentEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Shared entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentConfiguration).Assembly);
    }
}
```

#### Entity Configurations
- **PaymentConfiguration**: EF Core configuration for Payment entity
- **AuditConfiguration**: Audit trail table configuration
- **EventConfiguration**: Domain event table configuration

## Extensions and Utilities

### Service Registration Extensions

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // Register all event handlers from this assembly
        return services.AddEventHandlersFromAssemblyContaining<PaymentCreatedHandler>();
    }

    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        // Register shared utility services
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IPaymentValidator, PaymentValidator>();
        return services;
    }
}
```

### Common Utilities

- **DateTimeProvider**: Testable date/time abstraction
- **IdGenerator**: Consistent ID generation across services
- **ValidationExtensions**: Common validation helpers
- **LoggingExtensions**: Structured logging helpers

## Domain Logic

### Business Rules

Shared business rules and validation:
- Payment amount validation
- Currency validation
- Business rule enforcement
- Domain event creation

### Event Definitions

```csharp
public record PaymentCreated(
    Guid PaymentId,
    decimal Amount,
    DateTime CreatedAt) : IDomainEvent;

public record PaymentProcessed(
    Guid PaymentId,
    DateTime ProcessedAt) : IDomainEvent;
```

## Coding Standards

### Shared Library Design
- **Minimal Dependencies**: Keep dependencies to essential packages only
- **Backward Compatibility**: Maintain API compatibility between versions
- **Clear Abstractions**: Use interfaces for testability and flexibility
- **Version Management**: Use semantic versioning for shared library updates

### Data Model Conventions
- **Immutable Models**: Prefer record types and immutable classes
- **Validation Attributes**: Use data annotations for basic validation
- **Null Safety**: Enable nullable reference types and handle nulls appropriately
- **Serialization**: Ensure models serialize correctly for APIs and messaging

### Event Handler Patterns
- **Idempotent Operations**: Handle duplicate events gracefully
- **Error Handling**: Use Result patterns for operations that can fail
- **Async Operations**: Use async/await with proper CancellationToken support
- **Logging**: Include structured logging in all handlers

### Code Organization
- **Logical Grouping**: Organize code by domain area (Models, Handlers, Data)
- **Namespace Consistency**: Use consistent namespace patterns
- **File Naming**: Use descriptive file names that match class names
- **Interface Segregation**: Keep interfaces focused and single-purpose

### Testing Considerations
- **Test Utilities**: Include test helpers for common scenarios
- **Mock-Friendly**: Design abstractions that are easy to mock
- **Test Data Builders**: Provide builders for test data creation
- **Integration Testing**: Consider how shared components will be tested