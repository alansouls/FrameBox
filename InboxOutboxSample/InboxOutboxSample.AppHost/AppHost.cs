var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var paymentsDb = builder.AddPostgres("payments-db")
    .WithPgAdmin()
    .WithDataVolume();

var username = builder.AddParameter("rabbitmq-management-username", secret: true);
var password = builder.AddParameter("rabbitmq-management-password", secret: true);

var rabbitMq = builder.AddRabbitMQ("rabbitmq", username, password)
    .WithManagementPlugin();

var migrationsService = builder.AddProject<Projects.InboxOutboxSample_Migrations>("migrations")
    .WithReference(paymentsDb)
    .WaitFor(paymentsDb);

var apiService = builder.AddProject<Projects.InboxOutboxSample_ApiService>("apiservice")
    .WithReference(paymentsDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WaitForCompletion(migrationsService)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
