using QueryX.Models.Configuration;

namespace QueryX.Models.Configuration;

public class QueryXConfiguration
{
    public required string OpenAIKey { get; set; }
    public required string OpenAIUri { get; set; }
    public required string OpenAIModel { get; set; }
    public required List<DataSourceConfiguration> DataSourceConfigurations { get; set; }
}