# InboxOutboxSample.ApiService

## Purpose

InboxOutboxSample.ApiService is a RESTful API service that demonstrates the FrameBox outbox pattern implementation. It provides HTTP endpoints for creating payments while ensuring reliable message publishing through the outbox pattern, showcasing how to handle distributed transactions and eventual consistency in a microservices architecture.

## Key Features

- **RESTful API**: HTTP endpoints for payment operations
- **Outbox Pattern**: Reliable message publishing with transactional guarantees
- **Entity Framework Integration**: Persistent storage using PostgreSQL
- **RabbitMQ Integration**: Asynchronous message publishing
- **.NET Aspire Integration**: Service discovery, health checks, and observability
- **OpenAPI Documentation**: Swagger/OpenAPI support for API documentation

## How to Use

### 1. API Endpoints

The service provides the following endpoints:

#### Create Payment
```http
POST /api/v1/payments
Content-Type: application/json

{
  "amount": 100.50
}
```

Response:
```http
HTTP/1.1 201 Created
Location: /api/v1/payments/{id}

{
  "id": "guid",
  "amount": 100.50,
  "createdAt": "2024-01-01T10:00:00Z"
}
```

### 2. Outbox Pattern Flow

When a payment is created:

1. **Business Transaction**: Payment entity is saved to database
2. **Outbox Message**: Message is stored in outbox table in same transaction
3. **Background Processing**: Outbox dispatcher publishes message to RabbitMQ
4. **Event Handling**: Other services can consume the payment created event

### 3. Development Setup

To run the API service locally:

```bash
# Run through Aspire AppHost (recommended)
cd InboxOutboxSample.AppHost
dotnet run

# Or run standalone (requires manual database and RabbitMQ setup)
cd InboxOutboxSample.ApiService
dotnet run
```

### 4. Configuration

The service requires the following configuration:

```json
{
  "ConnectionStrings": {
    "payments-db": "Host=localhost;Database=payments;Username=postgres;Password=password"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## Architecture

### Dependencies

The API service integrates with:
- **FrameBox.Core**: Core outbox and event handling
- **FrameBox.Storage.EFCore**: Entity Framework storage for outbox
- **FrameBox.MessageBroker.RabbitMQ**: RabbitMQ message publishing
- **InboxOutboxSample.Shared**: Shared data models and handlers
- **InboxOutboxSample.ServiceDefaults**: Common Aspire service configuration

### Data Flow

```
HTTP Request → API Controller → Business Logic → EF Core Context
     ↓                                                    ↓
 HTTP Response ← API Response ← Entity Saved ← Database Transaction
                                      ↓
                              Outbox Message Stored
                                      ↓
                            Background Outbox Dispatcher
                                      ↓
                              RabbitMQ Message Published
                                      ↓
                              Event Handlers Process Message
```

### Database Schema

The service uses the following entities:
- **Payments**: Business domain entity
- **OutboxMessages**: FrameBox outbox pattern storage
- **InboxMessages**: FrameBox inbox pattern storage

## API Documentation

When running in development mode, OpenAPI documentation is available:
- Swagger UI: `https://localhost:{port}/swagger`
- OpenAPI JSON: `https://localhost:{port}/openapi/v1.json`

## Coding Standards

### Web API Conventions
- Use minimal APIs with route groups for organization
- Apply proper HTTP status codes (201 for creation, 400 for validation errors)
- Use record types for DTOs and request/response models
- Implement proper async/await patterns with CancellationToken

### Domain Modeling
- Keep business logic in domain entities
- Use value objects for complex data types
- Implement proper entity validation
- Follow domain-driven design principles

### Error Handling
- Use ProblemDetails for consistent error responses
- Handle validation errors gracefully
- Log errors with structured logging
- Use appropriate HTTP status codes

### Performance Patterns
- Use Entity Framework efficiently (avoid N+1 queries)
- Implement proper connection string configuration
- Use async operations for all I/O
- Configure appropriate HTTP client timeouts

### Code Organization
- Group related functionality in domain folders
- Use extension methods for service configuration
- Keep controllers/endpoints focused and minimal
- Separate concerns between API, business logic, and data access

### Testing Considerations
- Design endpoints to be easily testable
- Use dependency injection for testability
- Consider integration testing with test containers
- Mock external dependencies appropriately