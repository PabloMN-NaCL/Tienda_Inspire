using TiendInspire.Notificaciones;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services

var host = builder.Build();
host.Run();
