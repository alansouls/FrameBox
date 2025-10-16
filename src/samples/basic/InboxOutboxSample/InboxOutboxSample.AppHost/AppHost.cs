var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var paymentsDb = builder.AddPostgres("payments-db")
    .WithPgAdmin()
    .WithDataVolume();

var username = builder.AddParameter("rabbitmq-management-username", secret: true);
var password = builder.AddParameter("rabbitmq-management-password", secret: true);

//var rabbitMq = builder.AddRabbitMQ("rabbitmq", username, password)
//    .WithManagementPlugin();

var azureServiceBus = builder.AddAzureServiceBus("servicebus")
    .RunAsEmulator(container =>
    {
        container.WithConfigurationFile(path: "ResourcesConfig/servicebus-config.json");
    });

var migrationsService = builder.AddProject<Projects.InboxOutboxSample_Migrations>("migrations")
    .WithReference(paymentsDb)
    .WaitFor(paymentsDb);

var apiService = builder.AddProject<Projects.InboxOutboxSample_ApiService>("apiservice")
    .WithReference(paymentsDb)
    //.WithReference(rabbitMq)
    .WithReference(azureServiceBus)
    //.WaitFor(rabbitMq)
    .WaitFor(azureServiceBus)
    .WaitForCompletion(migrationsService)
    .WithHttpHealthCheck("/health");

var dashboard = builder.AddProject<Projects.InboxOutboxSample_Dashboard>("dashboard")
    .WithReference(paymentsDb)
    .WaitForCompletion(migrationsService)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
