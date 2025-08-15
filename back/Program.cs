using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PackageDbContext>(opt => opt.UseInMemoryDatabase("Packages"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "PackageAPI";
    config.Title = "PackageAPI v1";
    config.Version = "v1";
});

var allowedOrigin = "http://localhost:3000";

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOnly", policy =>
    {
        policy.WithOrigins(allowedOrigin)
            .AllowAnyHeader()
            .WithMethods("GET","POST","PUT","PATCH","DELETE","OPTIONS")
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "PackageAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}
app.UseCors("FrontendOnly");

app.MapPackageEndpoints();

app.Run();