using Basket.API.Repositories;
using Busket.API.GrpcServices;
using Discount.Grpc.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(x => x.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString"));
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(x =>
{
    var grpcAddress = builder.Configuration.GetValue<string>("GrpcSettings:DiscountUrl");
    x.Address = new Uri(grpcAddress);
});

builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
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
