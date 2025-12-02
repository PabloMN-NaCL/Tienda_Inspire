using System.Security.Principal;

var builder = DistributedApplication.CreateBuilder(args);

//var cache = builder.AddRedisContainer("cache");


var database = builder.AddPostgres("postgres").WithLifetime(ContainerLifetime.Persistent);
var postgresdb = database.AddDatabase("servertienda");

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("redis-data-identity")
    .WithRedisInsight();

var rabbit = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume("rabbitmq");



var myService = builder.AddProject<Projects.TiendaInspireIdentity>("tiendainsprireidentity")
    .WaitFor(postgresdb)
    .WaitFor(rabbit)
    .WithReference(rabbit)
    .WithReference(postgresdb);
   

builder.AddProject<Projects.TiendaAspire_ApiGateway>("tiendainspire-apigateway")
    .WithReference(redis)
    .WithReference(myService)
    .WaitFor(redis)
    .WaitFor(myService);

//builder.AddProject<Projects.TiendaInspireFront>("webfrontend")
//    .WaitFor(cache)
//    .WithReference(myService);

builder.AddProject<Projects.TiendInspire_Notificaciones>("tiendinspire-notificaciones");

//builder.AddProject<Projects.TiendaInspireFront>("webfrontend")
//    .WaitFor(cache)
//    .WithReference(myService);

builder.Build().Run();
