using System.Reflection.PortableExecutable;
using PristaneLaverieSmart.Infrastructure;
using System.Text.Json.Serialization;
using PristaneLaverieSmart.Application.Features.Machines.Queries;
using PristaneLaverieSmart.Application.Features.Machines.Commands;
using PristaneLaverieSmart.API.Middleware;
using FluentValidation;
using PristaneLaverieSmart.Application.Queries.GetAllBookings;
using PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;
using MediatR;
using PristaneLaverieSmart.Application.Common.Behaviors;
using PristaneLaverieSmart.Application.Features.Bookings.Query;


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

builder.Services.AddTransient<ExceptionHandlingMiddleware>(); // register our middleware
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateMachineCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateMachineCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("ui");
app.UseMiddleware<ExceptionHandlingMiddleware>(); // enable our middleware

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


app.Run();