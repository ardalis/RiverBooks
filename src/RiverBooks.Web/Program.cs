using FastEndpoints;
using FastEndpoints.Swagger;
using RiverBooks.Books;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

// Add Module Services
builder.Services.AddBookServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();
