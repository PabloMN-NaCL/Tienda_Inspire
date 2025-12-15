using MassTransit;
using System;
using TiendInspire.Notificaciones;
using TiendInspire.Notificaciones.Email;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMassTransit(x =>
{
    
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderCanceledConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("rabbitmq");

        if (!string.IsNullOrEmpty(connectionString))
        {
            cfg.Host(new Uri(connectionString));
        }

        cfg.UseMessageRetry(r => r.Intervals(
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(30)));

        
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

host.Run();