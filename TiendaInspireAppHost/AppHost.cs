var builder = DistributedApplication.CreateBuilder(args);

//var cache = builder.AddRedisContainer("cache");


var database = builder.AddPostgres("postgres").WithLifetime(ContainerLifetime.Persistent);
var postgresdb = database.AddDatabase("servertienda");


var myService = builder.AddProject<Projects.TiendaInspireIdentity>("tiendainsprireidentity")
    .WaitFor(postgresdb)
    .WithReference(postgresdb);

//builder.AddProject<Projects.TiendaInspireFront>("webfrontend")
//    .WaitFor(cache)
//    .WithReference(myService);

builder.Build().Run();
