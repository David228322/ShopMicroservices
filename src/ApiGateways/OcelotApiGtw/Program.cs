using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(configure =>
{
    configure.AddConfiguration(builder.Configuration.GetSection("Logging"));
    configure.AddConsole();
    configure.AddDebug();
});
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
builder.Services.AddOcelot().AddCacheManager(x =>
{
    x.WithDictionaryHandle();
});
var app = builder.Build();
await app.UseOcelot();

app.Run();
