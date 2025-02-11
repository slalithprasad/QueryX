using System;
using QueryX.Models.Database;

namespace QueryX.QueryGenerator.Interfaces;

public interface ISqlQueryGeneratorService
{
    Task<string> GenerateSqlQueryAsync(string? prompt, IEnumerable<TableSchema>? schema, CancellationToken cancellationToken = default);
}
