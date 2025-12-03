using Microsoft.AspNetCore.RateLimiting;
using TiendaAspire.ApiGateway.Extensions;
using RedisRateLimiting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddRedisClient("redis");

builder.Services.AddYarpReverseProxy(builder.Configuration);

builder.Services.AddRateLimiter(rateLimiterOptions =>
{

    // open policy for public endpoints (100 req/min)
    rateLimiterOptions.AddPolicy("open", context =>
    {
        var redis = context.RequestServices.GetRequiredService<IConnectionMultiplexer>();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RedisRateLimitPartition.GetFixedWindowRateLimiter(
            $"ip:{ipAddress}",
            _ => new RedisFixedWindowRateLimiterOptions
            {
                ConnectionMultiplexerFactory = () => redis,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
    });
});
builder.Services.AddAuthorization();

builder.Services.AddGatewayCors();

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();


app.UseRateLimiter();
//Yarp
app.MapReverseProxy();

app.MapControllers();

app.Run();
