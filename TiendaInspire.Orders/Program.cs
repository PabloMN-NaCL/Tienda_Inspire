using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TiendaInspire.Orders.Data;
using TiendaInspire.Orders.Services;
using TiendaInspire.Shared.CommonExtensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<OrdersDbContext>("ordersdb");

//Para eventos de rabbit
// Mensajes de envio
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("messaging");

        if (!string.IsNullOrEmpty(connectionString))
        {
            cfg.Host(new Uri(connectionString));
        }

        cfg.ConfigureEndpoints(context);
    });
});

// Add HttpClient for Catalog service
builder.Services.AddHttpClient("catalog", client =>
{
    client.BaseAddress = new Uri("https+http://orderflow-catalog");
});

// JWT Authentication (shared across all microservices)
builder.Services.AddJwtAuthentication(builder.Configuration);

// Register services
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await db.Database.MigrateAsync();

    app.MapOpenApi();
}

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
