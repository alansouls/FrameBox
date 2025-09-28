# FrameBox.Storage.EFCore

## Purpose

FrameBox.Storage.EFCore provides Entity Framework Core implementation for the FrameBox framework's storage abstraction. This library enables persistent storage of outbox and inbox messages using Entity Framework Core, supporting various database providers and ensuring transactional consistency with your business data.

## Key Features

- **EF Core Integration**: Native Entity Framework Core implementation for outbox and inbox storage
- **Database Provider Support**: Works with any EF Core supported database provider (SQL Server, PostgreSQL, MySQL, etc.)
- **Transactional Consistency**: Ensures outbox/inbox messages are stored in the same transaction as business data
- **Migration Support**: Includes database migrations for outbox and inbox tables
- **Configuration Extensions**: Easy setup through EF Core configuration extensions

## How to Use

### 1. Install the Package

Add the FrameBox.Storage.EFCore package to your project:

```xml
<PackageReference Include="FrameBox.Storage.EFCore" />
```

### 2. Configure DbContext

Configure your DbContext to use FrameBox outbox and inbox storage:

```csharp
using FrameBox.Storage.EFCore.Common.Extensions;

// In Program.cs
builder.Services.AddDbContext<MyDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(connectionString)
           .UseAsOutboxStorage(serviceProvider);
});

// Register storage services
builder.Services.AddOutboxEntityFrameworkCoreStorage<MyDbContext>();
builder.Services.AddInboxEntityFrameworkCoreStorage<MyDbContext>();
```

### 3. Configure DbContext Class

Your DbContext should include outbox and inbox configurations:

```csharp
public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    // Your business entities
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // FrameBox will automatically configure outbox and inbox tables
        // when using the storage extensions
    }
}
```

### 4. Database Migrations

Generate and apply migrations to create the outbox and inbox tables:

```bash
dotnet ef migrations add AddFrameBoxTables
dotnet ef database update
```

### 5. Usage in Business Logic

Use the DbContext normally - outbox messages are automatically handled:

```csharp
public async Task CreatePayment(CreatePaymentRequest request)
{
    var payment = new Payment(request.Amount);
    
    // Add business entity
    context.Payments.Add(payment);
    
    // Outbox messages are automatically saved when SaveChanges is called
    await context.SaveChangesAsync();
    
    // Messages in outbox will be processed and published by the outbox dispatcher
}
```

## Configuration Options

The library provides several configuration options:

- **Table Names**: Customize outbox and inbox table names
- **Schema Configuration**: Specify database schema for FrameBox tables  
- **Indexing**: Configure database indexes for optimal performance
- **Retention Policies**: Set up automatic cleanup of processed messages

## Database Schema

The library creates the following tables:

### Outbox Table
- Id (Primary Key)
- EventType (Message type)
- EventData (Serialized message data)
- CreatedAt (Timestamp)
- ProcessedAt (Processing timestamp)
- IsProcessed (Processing status)

### Inbox Table  
- Id (Primary Key)
- MessageId (Unique message identifier)
- EventType (Message type)
- EventData (Serialized message data)
- CreatedAt (Timestamp)
- ProcessedAt (Processing timestamp)
- IsProcessed (Processing status)

## Coding Standards

### C# Conventions
- Use C# 9.0+ features including nullable reference types
- Follow standard .NET naming conventions (PascalCase for public members, camelCase for private)
- Use `var` for local variables when the type is obvious
- Implement proper async/await patterns with CancellationToken support

### Entity Framework Best Practices
- Use proper entity configuration through Fluent API
- Implement value converters for complex types
- Use appropriate database column types and constraints
- Configure indexes for optimal query performance

### Architecture Patterns
- Follow repository and unit of work patterns through EF Core DbContext
- Use dependency injection for DbContext and services
- Implement proper transaction handling
- Use configuration objects for storage settings

### Code Organization
- Group functionality in Common, Outbox, and Inbox folders
- Use extension methods for EF Core configuration
- Keep entity configurations separate from DbContext
- Use descriptive names for tables, columns, and indexes

### Error Handling
- Handle database connection failures gracefully
- Use proper exception handling for EF Core operations
- Log database errors with appropriate context
- Implement retry policies for transient failures
- Handle concurrent access scenarios appropriately