using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Infrastructure.Persistence;
using ClinicalScheduler.Infrastructure.Persistence.Repositories;
using ClinicalScheduler.Infrastructure.Persistence.Seeders;
using ClinicalScheduler.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalScheduler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}
