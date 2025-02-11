using System;
using QueryX.Models.Enums;

namespace QueryX.Models.Configuration;

public class DataSourceConfiguration
{
    public required DataSourceType DataSource { get; set; }
    public required string ConnectionString { get; set; }
    public required int QueryTimeoutSeconds { get; set; }
}
