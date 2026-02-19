using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;
using PristaneLaverieSmart.Infrastructure.Persistence.Repository;
using PristaneLaverieSmart.Application.Features.Machines.Queries;
using PristaneLaverieSmart.Application.Features.Machines.Commands;

namespace PristaneLaverieSmart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection addInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PristaneLaverieSmartDbContext>(opt =>opt.UseSqlite(connectionString));
        services.AddScoped<IMachineRepository, MachineRepository>();
        services.AddScoped<GetAllMachinesHandler>();
        services.AddScoped<CreateMachineHandler>();

        return services;
    }
}