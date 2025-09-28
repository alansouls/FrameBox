# InboxOutboxSample.Web

## Purpose

InboxOutboxSample.Web is a Blazor Server application that provides a web-based user interface for the InboxOutboxSample demonstration. It showcases how to build interactive web applications that integrate with the FrameBox framework, providing a front-end for monitoring and interacting with the payment processing system.

## Key Features

- **Blazor Server**: Interactive web UI with server-side rendering
- **Real-time Updates**: Server-side interactivity with SignalR
- **Responsive Design**: Bootstrap-based responsive UI components
- **Service Integration**: Connects to API services through service discovery
- **.NET Aspire Integration**: Observability, health checks, and service discovery

## How to Use

### 1. Running the Application

Start the web application through the Aspire AppHost:

```bash
cd InboxOutboxSample.AppHost
dotnet run
```

The web application will be available through the Aspire dashboard.

### 2. Navigation Structure

The application includes standard navigation with:
- **Home**: Welcome page and application overview
- **Counter**: Interactive counter demonstration (Blazor sample)
- **Weather**: Weather forecast display (Blazor sample)

### 3. Features

#### Interactive Components
- Server-side Blazor components with real-time updates
- Form validation and user input handling
- Responsive design that works on desktop and mobile

#### Service Integration
- Calls to the API service for payment operations
- Real-time updates when payments are processed
- Integration with the overall distributed system

### 4. Configuration

The web application uses standard Blazor Server configuration:

```json
{
  "ConnectionStrings": {
    "payments-db": "Database connection for read operations"
  },
  "ServiceDiscovery": {
    "ApiService": "Service discovery endpoint for API calls"
  }
}
```

## Architecture

### Component Structure

```
Components/
├── Layout/          # Layout components (nav, footer, etc.)
│   ├── NavMenu.razor
│   └── MainLayout.razor
├── Pages/           # Page components
│   ├── Counter.razor
│   ├── Weather.razor
│   └── Home.razor
└── Shared/          # Reusable components
```

### Dependencies

The web application depends on:
- **ASP.NET Core Blazor Server**: Core web framework
- **Bootstrap**: UI styling and responsive design
- **Service Discovery**: For calling API services
- **InboxOutboxSample.ServiceDefaults**: Common Aspire configuration

### Data Flow

```
Browser Request → Blazor Server → Component Rendering → SignalR Connection
     ↑                                      ↓
User Interaction ← Real-time Updates ← Service Calls → API Service
```

## UI Components

### Layout Components
- **NavMenu**: Primary navigation with Bootstrap styling
- **MainLayout**: Main page layout with responsive design
- **Header/Footer**: Standard application chrome

### Page Components
- **Home**: Landing page with application information
- **Counter**: Interactive counter with state management
- **Weather**: Data display example with service calls

### Styling
- Bootstrap CSS framework for consistent styling
- Responsive design patterns for mobile compatibility
- Custom CSS for application-specific styling

## Coding Standards

### Blazor Conventions
- Use `.razor` files for components with HTML and C# code
- Implement proper component lifecycle methods (`OnInitializedAsync`, etc.)
- Use `@inject` directive for dependency injection
- Follow Blazor naming conventions for components and parameters

### Component Design
- Keep components focused and single-purpose
- Use proper parameter binding with `[Parameter]`
- Implement event callbacks for parent-child communication
- Use cascading parameters for shared state

### State Management
- Use component state for local UI state
- Implement proper async patterns for data loading
- Handle loading states and error conditions
- Use SignalR for real-time updates when needed

### Performance Patterns
- Use `@key` directive for efficient list rendering
- Implement proper async loading patterns
- Minimize component re-renders with proper state design
- Use streaming rendering for large data sets

### Code Organization
- Group related components in logical folders
- Keep component files focused and readable
- Use partial classes for complex component logic
- Separate concerns between UI and business logic

### Accessibility
- Use semantic HTML elements
- Include proper ARIA labels and attributes
- Ensure keyboard navigation works properly
- Test with screen readers and accessibility tools