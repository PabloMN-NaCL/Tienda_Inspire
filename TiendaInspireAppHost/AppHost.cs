using System.Security.Principal;

var builder = DistributedApplication.CreateBuilder(args);

//var cache = builder.AddRedisContainer("cache");

//Pgadmin
var database = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("postgres")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var postgresdb = database.AddDatabase("servertienda");


//Redis para identity
var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("redis-data-identity")
    .WithRedisInsight();

//Rabbit mensajeria
var rabbit = builder
    .AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin();

//Email server de pruebas
var mailServer = builder
    .AddContainer("maildev", "maildev/maildev:latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHttpEndpoint(port: 1080, targetPort: 1080, name: "web")
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp");

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


builder.AddProject<Projects.TiendInspire_Notificaciones>("tiendinspire-notificaciones")
    .WaitFor(rabbit)
    .WithReference(rabbit);

//builder.AddProject<Projects.TiendaInspireFront>("webfrontend")
//    .WaitFor(cache)
//    .WithReference(myService);

builder.AddProject<Projects.TiendaInspire_Catalog>("tiendainspire-catalog");

//builder.AddProject<Projects.TiendaInspireFront>("webfrontend")
//    .WaitFor(cache)
//    .WithReference(myService);

builder.Build().Run();
