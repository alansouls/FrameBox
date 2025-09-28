# InboxOutboxSample.AppHost

## Purpose

InboxOutboxSample.AppHost is the .NET Aspire application host that orchestrates the entire InboxOutboxSample application. It defines and configures all the services, databases, and message brokers needed for the sample application, providing a complete distributed application setup with service discovery, health checks, and development-time tooling.

## Key Features

- **.NET Aspire Orchestration**: Manages the lifecycle of all application services
- **Service Discovery**: Automatic service registration and discovery between components
- **Infrastructure Setup**: Configures PostgreSQL database, Redis cache, and RabbitMQ message broker
- **Health Monitoring**: Built-in health checks for all services
- **Development Experience**: Integrated development dashboard and tooling

## How to Use

### 1. Prerequisites

Ensure you have the following installed:
- .NET 9.0 SDK
- Docker Desktop (for running PostgreSQL, Redis, and RabbitMQ)
- .NET Aspire workload: `dotnet workload install aspire`

### 2. Running the Application

Start the application host:

```bash
dotnet run
```

This will:
- Start the Aspire dashboard
- Launch PostgreSQL with pgAdmin
- Start Redis cache
- Launch RabbitMQ with management UI
- Run database migrations
- Start the API service, web application, and dashboard

### 3. Accessing Services

Once running, you can access:
- **Aspire Dashboard**: http://localhost:15888 (development dashboard)
- **API Service**: Available through service discovery
- **Web Application**: Available through service discovery  
- **Dashboard**: Available through service discovery
- **pgAdmin**: Available through the PostgreSQL service
- **RabbitMQ Management**: Available through the RabbitMQ service

### 4. Service Dependencies

The AppHost manages the following dependency chain:
1. Infrastructure services (PostgreSQL, Redis, RabbitMQ)
2. Database migrations (waits for PostgreSQL)
3. API service (waits for database migrations and RabbitMQ)
4. Web applications (wait for database migrations)

## Configuration

The AppHost configures several services with specific requirements:

### Database Configuration
- PostgreSQL with persistent data volumes
- pgAdmin for database management
- Connection string: "payments-db"

### Message Broker Configuration
- RabbitMQ with management plugin
- Parameterized username and password (secrets)
- Connection name: "rabbitmq"

### Service Health Checks
- API service health check on `/health` endpoint
- Automatic service dependency waiting
- Built-in service discovery health monitoring

## Coding Standards

### .NET Aspire Conventions
- Use strongly-typed service references through generated Projects class
- Configure services with meaningful names for service discovery
- Use `WaitFor()` and `WaitForCompletion()` to manage service dependencies
- Configure health checks for all HTTP services

### Resource Configuration
- Use parameterized secrets for sensitive configuration (passwords, API keys)
- Configure persistent volumes for stateful services (databases)
- Use appropriate resource names that match connection strings in dependent services

### Architecture Patterns
- Follow .NET Aspire resource definition patterns
- Use extension methods for complex service configurations
- Group related resource configurations together
- Implement proper service dependency ordering

### Code Organization
- Keep AppHost logic minimal and focused on resource orchestration
- Use descriptive variable names for resources
- Group resource definitions by type (infrastructure, applications, etc.)
- Include comments for complex dependency chains

### Development Experience
- Configure development-time features (pgAdmin, RabbitMQ management)
- Use data volumes for persistent development data
- Enable appropriate management and monitoring tools
- Ensure quick startup and teardown for development cycles