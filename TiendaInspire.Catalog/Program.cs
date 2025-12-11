using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TiendaInspire.Catalog.Data;
using TiendaInspire.Catalog.Services;
using TiendaInspire.Shared.CommonExtensions;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();


builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");


builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

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
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();

    app.MapOpenApi();
}

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
