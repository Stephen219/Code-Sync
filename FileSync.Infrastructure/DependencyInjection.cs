using FileSync.Domain.Interfaces;
using FileSync.Infrastructure.Email;
using FileSync.Infrastructure.Persistence;
using FileSync.Infrastructure.Persistence.Repositories;
using FileSync.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileSync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string fileStoragePath,
        string baseUrl)
    {
        // Database
        //services.AddDbContext<AppDbContext>(options =>
            //options.UseSqlite(connectionString));
            services.AddDbContext<AppDbContext>(options =>
            {
                var dbPath = Path.Combine(
                    AppContext.BaseDirectory,
                    "filesync.db"
                );

                options.UseSqlite(
                    $"Data Source={dbPath}",
                    sqlite => sqlite.CommandTimeout(5)
                );
            });




        services.AddScoped<ISpaceRepository, SpaceRepository>();
        services.AddScoped<IFileDropRepository, DropRepository>();

        
        services.AddSingleton<IFileStorageService>(
            new LocalFileStorageService(fileStoragePath));

     
        services.AddSingleton<IEmailService>(provider =>
            new EmailService(
                provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<EmailService>>(),
                baseUrl));



        return services;
    }
}
