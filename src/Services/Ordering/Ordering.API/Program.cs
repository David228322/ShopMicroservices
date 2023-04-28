using Ordering.Infrastructure;
using Ordering.Application;
using Ordering.Infrastructure.Persistence;
using Ordering.API.Extensions;
using MassTransit;
using EventBus.Messages.Common;
using EventBus.Messages.Events;
using Ordering.API.EventBusConsumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

// Rabbit config
builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<BasketCheckoutConsumer>();

    options.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});
builder.Services.AddScoped<BasketCheckoutConsumer>();

var app = builder.Build();

app.MigrateDatabase<OrderContext>((context, service) =>
{
    var logger = service.GetService<ILogger<OrderSeedContext>>();
    OrderSeedContext.SeedAsync(context, logger).Wait();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
