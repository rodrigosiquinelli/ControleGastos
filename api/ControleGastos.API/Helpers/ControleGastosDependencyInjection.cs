using ControleGastos.Application.AutoMapper;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.Services;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Infrastructure;
using ControleGastos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace ControleGastos.API.Helpers;

public static class ControleGastosDependencyInjection
{
    /// Adiciona os serviços da aplicação ao container DI.
    public static IServiceCollection AddControleGastosServices(this IServiceCollection services)
    {
        // Services
        services.AddAutoMapper(config =>
        {
            config.AddProfile<MappingProfile>();
        });

        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<ITotaisService, TotaisService>();

        return services;
    }

    /// Adiciona os serviços de infraestrutura ao container DI.
    public static IServiceCollection AddControleGastosInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ControleGastosDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPessoaRepository, PessoaRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
        services.AddScoped<ITotaisRepository, TotaisRepository>();

        return services;
    }

    /// Adiciona a configuração do Swagger e CORS ao container DI.
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(swagger =>
        {
            // Define os metadados do Swagger UI
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ControleGastos.Api",
                Description = "Teste Controle de Gastos Residenciais."
            });
        });
        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            // Define uma política que permite requisições de qualquer origem
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
        return services;
    }
}
