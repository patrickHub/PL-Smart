using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Infrastructure.Persistence.DbContext;
using PristaneLaverieSmart.Infrastructure.Persistence.Repository;

namespace PristaneLaverieSmart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection addInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PristaneLaverieSmartDbContext>(opt =>opt.UseSqlite(connectionString));
        services.AddScoped<IMachineRepository, MachineRepository>();

        return services;
    }
}