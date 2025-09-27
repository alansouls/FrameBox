using FrameBox.Core.Extensions;
using FrameBox.MessageBroker.RabbitMQ.Common.Extensions;
using FrameBox.Storage.EFCore.Common.Extensions;
using InboxOutboxSample.ApiService.Domain;
using InboxOutboxSample.Shared.Data;
using InboxOutboxSample.Shared.Handlers.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFrameBoxCore();
builder.Services.AddOutboxEntityFrameworkCoreStorage<MyDbContext>();
builder.Services.AddInboxEntityFrameworkCoreStorage<MyDbContext>();
builder.Services.AddRabbitMQMessageBroker(builder.Configuration);
builder.Services.AddRabbitMQListener(builder.Configuration);
builder.Services.AddDbContext<MyDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("payments-db")).UseAsOutboxStorage(serviceProvider);
});
builder.EnrichNpgsqlDbContext<MyDbContext>();

builder.Services.AddHandlers();

builder.AddRabbitMQClient("rabbitmq");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var api = app.MapGroup("api");

var v1 = api.MapGroup("v1");

var payments = v1.MapGroup("payments");

payments.MapPost("/", async (CreatePayment dto, MyDbContext context, CancellationToken cancellationToken) =>
{
    var payment = new Payment(dto.Amount);

    await context.AddAsync(payment, cancellationToken);

    await context.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/v1/payments/{payment.Id}", payment);
})
.WithName("CreatePayment");

app.MapDefaultEndpoints();
app.Run();

record CreatePayment(decimal Amount);
