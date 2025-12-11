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


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();


app.UseRateLimiter();
app.MapDefaultEndpoints();
app.MapReverseProxy();


app.Run();
