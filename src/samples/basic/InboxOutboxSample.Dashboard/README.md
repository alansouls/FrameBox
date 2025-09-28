# InboxOutboxSample.Dashboard

## Purpose

InboxOutboxSample.Dashboard is a Blazor Server application that provides a monitoring and management dashboard for the InboxOutboxSample system. It offers real-time visibility into payment processing, outbox message status, inbox message handling, and overall system health, serving as an operational dashboard for the distributed system.

## Key Features

- **Real-time Monitoring**: Live updates of system metrics and message processing
- **Payment Tracking**: View payment history and status
- **Message Visibility**: Monitor outbox and inbox message processing
- **System Health**: Health check status for all system components
- **Interactive Dashboard**: Blazor Server with responsive UI components

## How to Use

### 1. Running the Dashboard

Start the dashboard through the Aspire AppHost:

```bash
cd InboxOutboxSample.AppHost
dotnet run
```

The dashboard will be available through the Aspire dashboard service listing.

### 2. Dashboard Features

#### Payment Monitoring
- View all payments created in the system
- Track payment processing status
- Monitor payment volumes and trends

#### Message Processing
- **Outbox Messages**: View pending and processed outbound messages
- **Inbox Messages**: Monitor incoming message processing
- **Processing Status**: Real-time status of message handlers

#### System Health
- Service health status indicators
- Database connection status
- Message broker connection status
- Performance metrics and alerts

### 3. Real-time Updates

The dashboard automatically updates with:
- New payments as they are created
- Message processing status changes
- Health check status updates
- System performance metrics

## Architecture

### Component Structure

```
Components/
├── Layout/          # Dashboard layout components
├── Pages/           # Dashboard pages
│   ├── Payments/    # Payment monitoring pages
│   ├── Messages/    # Message tracking pages
│   └── Health/      # System health pages
└── Shared/          # Reusable dashboard components
```

### Data Access

The dashboard reads data directly from:
- **Payment Database**: Read-only access to payment data
- **Outbox Tables**: Monitor outbox message processing
- **Inbox Tables**: Track inbox message handling
- **Health Check APIs**: System health monitoring

### Dependencies

- **ASP.NET Core Blazor Server**: Web framework
- **Entity Framework Core**: Database access
- **Bootstrap**: UI framework
- **SignalR**: Real-time updates
- **InboxOutboxSample.ServiceDefaults**: Common configuration

## Dashboard Sections

### Overview Page
- System status summary
- Key performance indicators
- Recent activity summary
- Alert notifications

### Payments Dashboard
- Payment list with search and filtering
- Payment details and history
- Payment volume charts
- Failed payment tracking

### Message Monitoring
- Outbox message queue status
- Inbox message processing status
- Message retry and error tracking
- Message throughput metrics

### Health Dashboard
- Service health status grid
- Database connection status
- Message broker health
- Performance metrics and trends

## Configuration

The dashboard requires database access configuration:

```json
{
  "ConnectionStrings": {
    "payments-db": "Read connection to payments database"
  },
  "Dashboard": {
    "RefreshInterval": "00:00:05",
    "PageSize": 50,
    "EnableRealTimeUpdates": true
  }
}
```

## Coding Standards

### Dashboard Design Principles
- **Real-time First**: Prioritize live data over cached data
- **Performance Aware**: Efficient queries and minimal data transfer
- **User-Friendly**: Clear, intuitive interface design
- **Responsive**: Works well on desktop and mobile devices

### Blazor Server Patterns
- Use SignalR hubs for real-time updates
- Implement efficient data loading patterns
- Handle connection failures gracefully
- Use streaming for large datasets

### Data Visualization
- Use appropriate chart types for different data
- Implement responsive charts that work on all devices
- Provide tooltips and interactive elements
- Use consistent color schemes and branding

### Performance Optimization
- Implement pagination for large data sets
- Use efficient database queries with proper indexing
- Cache frequently accessed data appropriately
- Minimize SignalR message frequency

### Code Organization
- Group dashboard components by functional area
- Use view models to separate data concerns
- Implement proper error handling and loading states
- Use dependency injection for data services

### Security Considerations
- Implement read-only database access
- Use appropriate authentication and authorization
- Validate all user inputs and filters
- Protect sensitive data in displays

### Monitoring and Alerting
- Include proper logging for dashboard operations
- Implement health checks for the dashboard itself
- Monitor dashboard performance and errors
- Provide clear error messages to users