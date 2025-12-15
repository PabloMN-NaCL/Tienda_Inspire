using System.Security.Principal;

var builder = DistributedApplication.CreateBuilder(args);


//Pgadmin
var database = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("postgres") 
    .WithHostPort(5431)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var postgresdb = database.AddDatabase("servertienda");
var catalogDb = database.AddDatabase("catalogdb");
var ordersDb = database.AddDatabase("ordersdb");


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


//MicroServices
var myService = builder.AddProject<Projects.TiendaInspireIdentity>("tiendainspireidentity")
    .WaitFor(postgresdb)
    .WaitFor(rabbit)
    .WithReference(rabbit)
    .WithReference(postgresdb)
    .WithReference(redis);


var catalogService = builder.AddProject<Projects.TiendaInspire_Catalog>("tiendainspirecatalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

var ordersService = builder.AddProject<Projects.TiendaInspire_Orders>("tiendainspireorders")
      .WithReference(ordersDb)
    .WithReference(rabbit)
    .WithReference(catalogService)
    .WaitFor(ordersDb)
    .WaitFor(rabbit);


builder.AddProject<Projects.TiendaAspire_ApiGateway>("tiendainspire-apigateway")
    .WithReference(redis)
    .WithReference(myService)
    .WithReference(catalogService)
    .WithReference(ordersService)
    .WaitFor(redis)
    .WaitFor(myService)
    .WaitFor(catalogService)
    .WaitFor(ordersService);



builder.AddProject<Projects.TiendInspire_Notificaciones>("tiendinspire-notificaciones")
    .WaitFor(rabbit)
    .WithReference(rabbit)
    .WithEnvironment("Email__SmtpHost", mailServer.GetEndpoint("smtp").Property(EndpointProperty.Host))
    .WithEnvironment("Email__SmtpPort", mailServer.GetEndpoint("smtp").Property(EndpointProperty.Port));




builder.Build().Run();
