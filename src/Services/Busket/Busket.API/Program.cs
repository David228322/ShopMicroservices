using Basket.API.Repositories;
using Busket.API.GrpcServices;
using Discount.Grpc.Protos;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// General config
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddAutoMapper(typeof(Program));

// Redis config
builder.Services.AddStackExchangeRedisCache(x => x.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString"));
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// Grpc config
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(x =>
{
    var grpcAddress = builder.Configuration.GetValue<string>("GrpcSettings:DiscountUrl");
    x.Address = new Uri(grpcAddress);
});

builder.Services.AddScoped<DiscountGrpcService>();

// Rabbit config
builder.Services.AddMassTransit(options =>
{
    options.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
