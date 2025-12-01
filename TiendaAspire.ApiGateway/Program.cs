using Microsoft.AspNetCore.RateLimiting;
using OrderFlowClase.ApiGateway.Extensions;
using RedisRateLimiting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.AddRedisClient("redis");

builder.Services.AddYarpReverseProxy(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//Yarp
app.MapReverseProxy();

app.Run();
