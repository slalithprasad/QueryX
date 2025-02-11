using Microsoft.Extensions.DependencyInjection;
using QueryX.Core.Interfaces;
using QueryX.Core.Services;
using QueryX.Models.Configuration;
using QueryX.QueryGenerator.Interfaces;
using QueryX.QueryGenerator.Services;
using QueryX.Sql.Interfaces;
using QueryX.Sql.Repositories;

namespace QueryX.Core.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, QueryXConfiguration config)
    {
        services.AddSingleton(config);

        services.AddSingleton<ISqlQueryGeneratorService, SqlQueryGeneratorService>();
        services.AddSingleton<ISqlRepository, SqlRepository>();
        services.AddSingleton<IQueryXService, QueryXService>();

        return services;
    }
}
