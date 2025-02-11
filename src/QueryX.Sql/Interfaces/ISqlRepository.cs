using QueryX.Models.Database;

namespace QueryX.Sql.Interfaces;

public interface ISqlRepository
{
    Task<IEnumerable<TableSchema>> GetSqlTablesWithSchemaAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> QueryAsync(string query, CancellationToken cancellationToken = default);
}