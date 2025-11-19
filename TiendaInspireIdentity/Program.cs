using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaInspireIdentity.Data;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddSwaggerGen();

builder.Services.AddEndpointsApiExplorer();



//DbContext 
builder.AddNpgsqlDbContext<ApplicationDbContext>("ServerTienda");

// Add ASP.NET Core Identity
//Setting of the login screen
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false; // Set to true in production
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//Yarp
//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//CORS para seguridad TODO
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {

            // En producción, usa el dominio real de tu frontend.
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Autentificacion con JWT (TODO)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Configuración del token: Clave, Emisor, Audiencia, etc.
        // ...
    });

var app = builder.Build();

app.UseSwagger();

//Modo desarrollo
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    app.MapOpenApi();
    //Swagger
    
    app.UseSwaggerUI(options=>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TiendaInspireIdentity v1");
    });
}








builder.Services.AddAuthorization();

//Implementacion CORS
app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//Implementacion de ServicDefaults
app.MapDefaultEndpoints();

app.MapControllers();

//Minimum Api 18-11-25
// Falta inyectar dependecnias de nuavas interfaces que se usan (TODO)
//app.MapAuthGroup();
//app.MapRegisterUser();

//Yarp
//app.MapReverseProxy();

app.Run();
