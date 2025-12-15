using Asp.Versioning;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TiendaInspireIdentity;
using TiendaInspireIdentity.Data;
using TiendaInspireIdentity.Extensions;
using TiendaInspireIdentity.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
builder.Configuration.AddUserSecrets(typeof(Program).Assembly, true);

//A�adir servicios 
//Token service?
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddEndpointsApiExplorer();

//DbContext 
builder.AddNpgsqlDbContext<ApplicationDbContext>("servertienda");

//Generate OpenApi services
builder.Services.AddOpenApi();
//builder.Services.AddOpenApi("v1", options =>
//{
//    options.ConfigureDocumentInfo(
//        "TiendaInspire Identity API V1",
//        "v1",
//        "Authentication API using Minimal APIs with JWT Bearer authentication");
//    options.AddJwtBearerSecurity();
//    options.FilterByApiVersion("v1");
//});

//builder.Services.AddOpenApi("v2", options =>
//{
//    options.ConfigureDocumentInfo(
//        "TiendaInspire Identity API V2",
//        "v2",
//        "Authentication API using Controllers with JWT Bearer authentication");
//    options.AddJwtBearerSecurity();
//    options.FilterByApiVersion("v2");
//});

//Habilitar versionado
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

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



builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("rabbitmq");

        if (!string.IsNullOrEmpty(connectionString))
        {
            cfg.Host(new Uri(connectionString));
        }

        cfg.ConfigureEndpoints(context);
    });
});


//CORS para seguridad TODO
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {

            // En producci�n, usa el dominio real de tu frontend.
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Autentificacion con JWT (TODO)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:SecretKey").Value!))
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();


var app = builder.Build();

//Modo desarrollo
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Initial seed for database (Admin name and pass, roles)
    await app.Services.SeedDevelopmentDataAsync();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    app.MapOpenApi();
    //Swagger
    
    app.UseSwaggerUI(options=>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TiendaInspireIdentity v1");
        options.SwaggerEndpoint("/openapi/v2.json", "TiendaInspireIdentity v2");
    });
}

app.UseCors();


app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapDefaultEndpoints();

app.MapControllers();



app.Run();
