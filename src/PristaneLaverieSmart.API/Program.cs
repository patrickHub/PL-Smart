using PristaneLaverieSmart.Infrastructure;
using System.Text.Json.Serialization;
using PristaneLaverieSmart.Application.Features.Machines.Queries;
using PristaneLaverieSmart.Application.Features.Machines.Commands;
using PristaneLaverieSmart.API.Middleware;
using FluentValidation;
using PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;
using MediatR;
using PristaneLaverieSmart.Application.Common.Behaviors;
using PristaneLaverieSmart.Application.Features.Bookings.Query;
using PristaneLaverieSmart.Application.Features.Bookings.Commands;
using PristaneLaverieSmart.API.Contracts;
using PristaneLaverieSmart.Application.Features.Machines.Queries.GetAllMachineAudits;
using Serilog;
using Serilog.Context;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .ReadFrom.Services(services)
       .Enrich.FromLogContext()
       .Enrich.WithEnvironmentName()
       .Enrich.WithProcessId()
       .Enrich.WithThreadId()
       .WriteTo.Console();
});

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

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
});

builder.Services.AddTransient<ExceptionHandlingMiddleware>(); // register our middleware
builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateMachineCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateMachineCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PristaneLaverieSmartDbContext>("database");

//builder.Services.AddHostedService<BookingStatusMonitor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PristaneLaverieSmartDbContext>();
    db.Database.Migrate();   // ✅ creates DB + tables + applies migrations
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("ui");
app.UseMiddleware<ExceptionHandlingMiddleware>(); // enable our middleware
app.Use(async (context, next) => // pushes correlation id into Serilog LogContex
{
    var cid = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString() ?? "";
    using (LogContext.PushProperty("CorrelationId", cid))
    {
        await next();
    }
});
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapHealthChecks("/health"); // will return 200 if DB is reachable

app.MapGet("/api/machines", async (IMediator mediator, CancellationToken ct) =>
{
    var machineDtos = await mediator.Send(new GetAllMachinesQuery(), ct);
    return Results.Ok(machineDtos);
});

app.MapPost("api/machines", async(CreateMachineCommand machineCommand, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(machineCommand, ct);
    return Results.Created($"/api/machine/{id}", new {id});
});

app.MapPost("/api/machines/{id:guid}/status", async (
    Guid id,
    SetMachineStatusRequest body,
    IMediator mediator,
    CancellationToken ct) =>
{
    await mediator.Send(new SetMachineStatusCommand(id, body.Status), ct);
    return Results.NoContent();
});

app.MapGet("/api/machines/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var machine = await mediator.Send(new GetMachineByIdQuery(id), ct);
    return Results.Ok(machine);
});

app.MapGet("/api/bookings", async (IMediator mediator, CancellationToken ct) =>
{
    var bookings = await mediator.Send(new GetAllBookingQuerry(), ct);
    return Results.Ok(bookings);
});

app.MapPost("/api/bookings", async (CreateBookingCommand bookingCommand, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(bookingCommand, ct);
    return Results.Created($"/api/bookings/{id}", new { id });
});

app.MapPost("/api/bookings/{id:guid}/cancel", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    await mediator.Send(new CancelBookingCommand(id), ct);
    return Results.NoContent();
});

app.MapPost("/api/bookings/{id:guid}/complete", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    await mediator.Send(new CompleteBookingCommand(id), ct);
    return Results.NoContent();
});

app.MapGet("/api/machines/{id:guid}/audits", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var audits = await mediator.Send(new GetMachineAuditsQuery(id), ct);
    return Results.Ok(audits);
});


app.Run();

public partial class Program{ }