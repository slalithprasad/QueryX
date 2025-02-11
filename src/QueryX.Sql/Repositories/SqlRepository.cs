using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using QueryX.Models.Enums;
using QueryX.Models.Configuration;
using QueryX.Sql.Interfaces;
using QueryX.Models.Database;

namespace QueryX.Sql.Repositories;

public class SqlRepository(QueryXConfiguration configuration) : ISqlRepository
{

    public async Task<IEnumerable<object>> QueryAsync(string query, CancellationToken cancellationToken = default)
    {
        var dataSourceConfig = configuration.DataSourceConfigurations
            .FirstOrDefault(x => x.DataSource == DataSourceType.MSSql);

        if (dataSourceConfig == null || string.IsNullOrEmpty(dataSourceConfig.ConnectionString))
        {
            throw new InvalidOperationException("MSSql connection string is missing or not configured properly.");
        }

        using IDbConnection db = new SqlConnection(dataSourceConfig.ConnectionString);

        var commandDefinition = new CommandDefinition(query, commandTimeout: dataSourceConfig.QueryTimeoutSeconds, cancellationToken: cancellationToken);

        return await db.QueryAsync<object>(commandDefinition).ConfigureAwait(false);
    }

    public async Task<IEnumerable<TableSchema>> GetSqlTablesWithSchemaAsync(CancellationToken cancellationToken = default)
    {
        var dataSourceConfig = configuration.DataSourceConfigurations
            .FirstOrDefault(x => x.DataSource == DataSourceType.MSSql);

        if (dataSourceConfig == null || string.IsNullOrEmpty(dataSourceConfig.ConnectionString))
        {
            throw new InvalidOperationException("MSSql connection string is missing or not configured properly.");
        }

        var query = @"
        SELECT 
            TABLE_SCHEMA AS SchemaName,
            TABLE_NAME AS TableName,
            COLUMN_NAME AS ColumnName,
            DATA_TYPE AS DataType
        FROM INFORMATION_SCHEMA.COLUMNS
        ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION";

        using IDbConnection db = new SqlConnection(dataSourceConfig.ConnectionString);

        var result = (await db.QueryAsync<ColumnSchemaRaw>(new CommandDefinition(query, cancellationToken: cancellationToken)))
        .ToList();

        return result
        .Where(r => !string.IsNullOrWhiteSpace(r.SchemaName) && !string.IsNullOrWhiteSpace(r.TableName))
        .GroupBy(r => (r.SchemaName?.Trim().ToLower(), r.TableName?.Trim().ToLower()))
        .Select(g => new TableSchema
        {
            SchemaName = g.First().SchemaName,
            TableName = g.First().TableName,
            Columns = g.Select(c => new ColumnSchema
            {
                ColumnName = c.ColumnName,
                DataType = c.DataType
            }).ToList()
        }).ToList();
    }

    private class ColumnSchemaRaw
    {
        public string? SchemaName { get; set; }
        public string? TableName { get; set; }
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
    }
}
