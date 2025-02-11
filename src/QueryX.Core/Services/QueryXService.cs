using QueryX.Core.Interfaces;
using QueryX.Models.Database;
using QueryX.QueryGenerator.Interfaces;
using QueryX.Sql.Interfaces;

namespace QueryX.Core.Services;

public class QueryXService(ISqlQueryGeneratorService queryGenerator, ISqlRepository repository) : IQueryXService
{
    private readonly ISqlQueryGeneratorService _queryGenerator = queryGenerator;
    private readonly ISqlRepository _repository = repository;

    public async Task<(string, IEnumerable<object>)> QueryAsync(string? prompt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prompt);

        IEnumerable<TableSchema> schema = await _repository.GetSqlTablesWithSchemaAsync();

        string query = await _queryGenerator.GenerateSqlQueryAsync(prompt, schema, cancellationToken).ConfigureAwait(false);

        IEnumerable<object> data = await _repository.QueryAsync(query, cancellationToken).ConfigureAwait(false);

        return (query, data);
    }
}