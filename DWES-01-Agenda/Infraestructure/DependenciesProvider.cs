using System.Data;
using DWES_01_Agenda.Entity;
using DWES_01_Agenda.Mappers;
using DWES_01_Agenda.Models;
using DWES_01_Agenda.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;
using DWES_01_Agenda.Service;

namespace DWES_01_Agenda.Infraestructure;

public static class DependenciesProvider
{
    public static IServiceCollection AddDependenciesProvider(this IServiceCollection services, string provider,
        string connectionString)
    {
        switch (provider)
        {
            case "EFCore":
                services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));
                services.AddScoped<IContactoRepository, EfCoreContactoRepository>();
                break;
            default:
                services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));
                services.AddScoped<IContactoRepository, EfCoreContactoRepository>();
                break;
        }
        
        services.AddTransient<IMapper<Contacto, ContactoEntity>, ContactoMapper>();
        
        services.AddMemoryCache();
        
        services.AddScoped<ContactoService>();
        
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        
        return services;
    }
}