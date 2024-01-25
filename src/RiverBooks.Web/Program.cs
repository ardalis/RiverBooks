using FastEndpoints;
using RiverBooks.Books;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();

// add modules
builder.Services.AddBooksModule();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Map Modules
app.MapBookEndpoints();

app.UseFastEndpoints();

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class MyResponse
{
    public string FullName { get; set; }
    public string Message { get; set; }
}
public class MyEndpoint : EndpointWithoutRequest<MyResponse>
{
    public override void Configure()
    {
        Get("/hello/world");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        await SendAsync(new()
        {
            FullName = $"Steve",
            Message = "Welcome to FastEndpoints..."
        });
    }
}