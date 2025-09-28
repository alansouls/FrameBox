# InboxOutboxSample.Migrations

## Purpose

InboxOutboxSample.Migrations is a database migration project that manages database schema changes for the InboxOutboxSample application. It contains Entity Framework Core migrations to create and update the database schema, including tables for payments, outbox messages, inbox messages, and any other data structures needed by the application.

## Key Features

- **Database Schema Management**: Creates and updates database tables
- **Entity Framework Migrations**: Version-controlled database schema changes
- **Initialization Support**: Sets up the database from scratch
- **Production Ready**: Handles schema migrations in production environments
- **Aspire Integration**: Runs as part of the Aspire application startup sequence

## How to Use

### 1. Running Migrations

#### Through Aspire (Recommended)
Migrations run automatically when starting the AppHost:

```bash
cd InboxOutboxSample.AppHost
dotnet run
```

The migrations service will run before other services start, ensuring the database is ready.

#### Manual Migration
You can also run migrations manually:

```bash
cd InboxOutboxSample.Migrations
dotnet run
```

### 2. Adding New Migrations

When you change entities or add new tables:

```bash
# Navigate to a project with DbContext (like ApiService)
cd InboxOutboxSample.ApiService

# Add a new migration
dotnet ef migrations add MigrationName --startup-project ../InboxOutboxSample.Migrations

# Apply the migration
dotnet ef database update --startup-project ../InboxOutboxSample.Migrations
```

### 3. Database Schema

The migrations create the following tables:

#### Business Tables
- **Payments**: Core business entity table
- **PaymentEvents**: Domain events related to payments

#### FrameBox Tables
- **OutboxMessages**: Stores messages for reliable publishing
- **InboxMessages**: Handles incoming message deduplication

#### Audit Tables
- **AuditLogs**: System audit trail (if configured)

### 4. Environment Configuration

Different connection strings for different environments:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=payments_dev;Username=postgres;Password=password"
  }
}
```

## Migration Management

### Best Practices

#### Migration Naming
- Use descriptive names: `AddPaymentTable`, `UpdateOutboxIndexes`
- Include issue/feature numbers when relevant
- Use consistent naming patterns across the project

#### Migration Content
- **Forward Migrations**: Create tables, add columns, create indexes
- **Rollback Support**: Ensure migrations can be safely rolled back
- **Data Preservation**: Handle data migration carefully in production

#### Production Considerations
- Test migrations on a copy of production data
- Plan for downtime if required for breaking changes
- Use migration bundles for production deployments
- Monitor migration execution time and performance

### Migration Types

#### Initial Migration
Creates the base schema with all core tables:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Payments",
        columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Amount = table.Column<decimal>(type: "numeric", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
        });
}
```

#### Schema Updates
Add new columns or modify existing structures:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "Status", 
        table: "Payments",
        type: "varchar(50)",
        nullable: false,
        defaultValue: "Pending");
}
```

#### Index Migrations
Optimize query performance:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateIndex(
        name: "IX_Payments_CreatedAt",
        table: "Payments",
        column: "CreatedAt");
}
```

## Configuration

### Database Connection
The migrations project uses the same connection string as other services:

```json
{
  "ConnectionStrings": {
    "payments-db": "Connection string matching other services"
  }
}
```

### Migration Options
Configure migration behavior:

```json
{
  "Migration": {
    "AutomaticMigrationsEnabled": false,
    "CommandTimeout": 300,
    "RetryCount": 3
  }
}
```

## Coding Standards

### Migration Code Quality
- **Idempotent Operations**: Ensure migrations can be run multiple times safely
- **Explicit Schemas**: Define column types, sizes, and constraints explicitly
- **Error Handling**: Handle migration failures gracefully
- **Performance Aware**: Consider impact on large tables

### Entity Framework Conventions
- Use Fluent API for complex configurations
- Follow EF Core naming conventions
- Configure appropriate indexes for query performance
- Use value converters for complex types

### Database Design
- **Normalization**: Follow appropriate normalization principles
- **Constraints**: Implement foreign keys, unique constraints, check constraints
- **Indexes**: Create indexes for commonly queried columns
- **Data Types**: Use appropriate database-specific data types

### Code Organization
- Keep migrations in chronological order
- Use meaningful migration file names
- Group related changes in single migrations when possible
- Document complex migrations with comments

### Testing Migrations
- Test migrations on development databases
- Verify rollback scenarios work correctly
- Test with realistic data volumes
- Validate performance impact of schema changes