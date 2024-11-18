using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TenantProvider>();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseInMemoryDatabase("catalog-api")
);

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
var url = $"http://0.0.0.0:{port}";
var target = Environment.GetEnvironmentVariable("TARGET") ?? "World";

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();

app.MapGet("/", () => $"Hello {target}!");

app.MapPost("/create", async ([FromBody] CreateCatalogRequestModel request, AppDbContext dbContext, TenantProvider tenantProvider) => 
{
    var catalogEntity = new CatalogEntity 
    {
        Name = request.Name,
        TenantId = tenantProvider.TenantId
    };

    await dbContext.Catalogs.AddAsync(catalogEntity);
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/get", async (AppDbContext dbContext) => 
{
    var catalogEntities = await dbContext.Catalogs.ToListAsync();
    return Results.Ok(catalogEntities);
});

app.Run(url);
