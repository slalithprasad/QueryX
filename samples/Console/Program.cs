using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueryX.Core.Extensions;
using QueryX.Core.Interfaces;
using QueryX.Models.Configuration;
using QueryX.Models.Enums;

var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

QueryXConfiguration configuration = new QueryXConfiguration
{
    OpenAIKey = config["OpenAIKey"]!,
    OpenAIModel = config["OpenAIModel"]!,
    OpenAIUri = config["OpenAIUri"]!,

    DataSourceConfigurations = new List<DataSourceConfiguration>
    {
        new DataSourceConfiguration
        {
            DataSource = DataSourceType.MSSql,
            ConnectionString = config["DbConnectionString"]!,
            QueryTimeoutSeconds = 60
        }
    }
};

var services = new ServiceCollection();
services.AddServices(configuration);

using var serviceProvider = services.BuildServiceProvider();

var queryXService = serviceProvider.GetRequiredService<IQueryXService>();

string? prompt = Console.ReadLine();
var (sqlQuery, result) = await queryXService.QueryAsync(prompt);

Console.WriteLine(@$"SQL Query: {sqlQuery}

Result: 
{JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true })}");