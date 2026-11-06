using ErrorHandling.Files;
using ErrorHandling.Middlewares;
using ErrorHandling.Repositories;
using ErrorHandling.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(
    option =>
    {
        option.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path.Value;

            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        };
    }
);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped(sp => new SqlConnection(builder.Configuration.GetConnectionString("ecommerce")));

// Repository
builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

// Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageStorage, LocalProductImageStorage>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
