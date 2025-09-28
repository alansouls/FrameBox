# InboxOutboxSample.ServiceDefaults

## Purpose

InboxOutboxSample.ServiceDefaults provides common .NET Aspire service defaults that are shared across all services in the InboxOutboxSample application. This library standardizes service discovery, health checks, OpenTelemetry observability, and resilience patterns, ensuring consistent behavior across all application services.

## Key Features

- **Service Discovery**: Automatic service registration and discovery configuration
- **Health Checks**: Standard health and aliveness endpoint configuration
- **OpenTelemetry**: Distributed tracing, metrics, and logging setup
- **Resilience**: Standard HTTP client resilience patterns
- **Observability**: Integrated telemetry collection and export

## How to Use

### 1. Service Configuration

Add service defaults to any .NET Aspire service:

```csharp
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (includes service discovery, health checks, telemetry)
builder.AddServiceDefaults();

// Add your other services...
builder.Services.AddMyServices();

var app = builder.Build();

// Map default endpoints (health checks)
app.MapDefaultEndpoints();

app.Run();
```

### 2. Automatic Features

When you call `AddServiceDefaults()`, you automatically get:

- **Service Discovery**: HTTP clients configured with service discovery
- **Resilience**: Standard retry, circuit breaker, and timeout policies
- **Health Checks**: `/health` and `/alive` endpoints
- **OpenTelemetry**: Tracing for HTTP, ASP.NET Core, and custom sources
- **Metrics Collection**: Standard .NET metrics collection
- **Logging**: Structured logging with OpenTelemetry integration

### 3. Health Check Endpoints

The service defaults configure two health check endpoints:

- **/health**: Comprehensive health checks for dependencies
- **/alive**: Simple aliveness check for service status

### 4. HTTP Client Configuration

All HTTP clients automatically receive:
- Service discovery integration
- Standard resilience handlers (retry, circuit breaker, timeout)
- OpenTelemetry instrumentation
- Proper timeout and cancellation handling

## Configuration Options

### OpenTelemetry Configuration

The service defaults configure OpenTelemetry with:
- **Tracing**: HTTP requests, ASP.NET Core activities, custom traces
- **Metrics**: ASP.NET Core metrics, HTTP client metrics, runtime metrics  
- **Resource Detection**: Automatic service name and version detection
- **Exporters**: Configured based on environment (OTLP, Console, etc.)

### Service Discovery Options

Service discovery can be customized:

```csharp
// Uncomment in Extensions.cs to restrict allowed schemes
builder.Services.Configure<ServiceDiscoveryOptions>(options =>
{
    options.AllowedSchemes = ["https"]; // Only allow HTTPS services
});
```

### Health Check Configuration

Health checks are configured with:
- **Readiness checks**: Tagged with "ready" for startup dependencies
- **Liveness checks**: Tagged with "live" for ongoing service health
- **Custom predicates**: Filter which health checks run for each endpoint

## Integration with Aspire Dashboard

When running in the .NET Aspire AppHost, these defaults automatically integrate with:
- **Distributed Tracing**: View traces in the Aspire dashboard
- **Metrics Monitoring**: Real-time metrics visualization
- **Log Aggregation**: Centralized log viewing
- **Health Monitoring**: Service health status in dashboard

## Coding Standards

### .NET Aspire Conventions
- Use the standard Aspire service defaults extension pattern
- Configure health checks with appropriate tags ("ready", "live")
- Use standard OpenTelemetry activity names and tags
- Follow Aspire telemetry naming conventions

### Service Configuration
- Keep service defaults generic and reusable across services
- Use environment-based configuration for telemetry exporters
- Configure sensible defaults that work in development and production
- Use feature flags for optional telemetry features

### Health Check Implementation
- Use meaningful health check names and descriptions
- Configure appropriate timeouts for health checks
- Use tags to categorize readiness vs liveness checks
- Include dependency health checks (database, message broker, etc.)

### Code Organization
- Keep all common service configuration in Extensions.cs
- Use extension methods for each major feature area
- Group related configuration methods together
- Use clear method names that describe their purpose

### Performance Considerations
- Configure appropriate sampling rates for tracing in production
- Use efficient health check implementations
- Set reasonable HTTP client timeouts and retry policies
- Consider the impact of telemetry on application performance