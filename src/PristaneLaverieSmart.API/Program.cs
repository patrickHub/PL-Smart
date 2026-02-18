using System.Reflection.PortableExecutable;
using PristaneLaverieSmart.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=pristaneLaverieSmart.db";
builder.Services.addInfrastructure(cs);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("ui", p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .WithOrigins("https://localhost:5001", "http://localhost:5000", "http://localhost:5120")
    );
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("ui");

app.MapGet("/api/machines", async (PristaneLaverieSmart.Application.Abstractions.Persistence.IMachineRepository repo, CancellationToken ct) =>
{
    var machines = await repo.GetAllAsync(ct);
    return Results.Ok(machines.Select(m =>new
    {
        m.Id,
        m.Name,
        m.PricePerCycle,
        m.Status
    }));
});

app.MapPost("api/machines/seed", async(PristaneLaverieSmart.Application.Abstractions.Persistence.IMachineRepository repos, CancellationToken ct) =>
{
    await repos.AddAsync(new PristaneLaverieSmart.Domain.Entities.Machine{Name="Washer #1", PricePerCycle=13.30m}, ct);
    await repos.AddAsync(new PristaneLaverieSmart.Domain.Entities.Machine{Name="Drayer #1", PricePerCycle=1.5m}, ct);
    return Results.Ok();
});

app.Run();