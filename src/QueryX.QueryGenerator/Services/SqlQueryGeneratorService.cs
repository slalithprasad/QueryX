using Microsoft.SemanticKernel;
using QueryX.Models.Configuration;
using QueryX.Models.Database;
using QueryX.QueryGenerator.Interfaces;

namespace QueryX.QueryGenerator.Services;

public class SqlQueryGeneratorService(QueryXConfiguration configuration) : ISqlQueryGeneratorService
{
    private readonly Kernel _kernel = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(configuration.OpenAIModel, configuration.OpenAIUri, configuration.OpenAIKey).Build();

    public async Task<string> GenerateSqlQueryAsync(string? prompt, IEnumerable<TableSchema>? schema, CancellationToken cancellationToken = default)
    {
        string message = $@"
        You are a SQL expert. Convert the following natural language query into an optimized SQL statement.
    
        Database Schema:
        {FormatDatabaseSchema(schema)}
    
        Rules:
        - Use JOINs when necessary.
        - Ensure the query is efficient.
        - Never delete or modify data.
        - Do not include explanations, JSON, Markdown, or any extra formatting like ```sql``` etc.
        - Return only the raw SQL query as plain text.
    
        User Query: {prompt}";

        var result = await _kernel.InvokePromptAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.GetValue<string>()?.Trim() ?? "Error generating SQL";
    }

    private static string FormatDatabaseSchema(IEnumerable<TableSchema>? schema)
    {
        if (schema is null || !schema.Any())
        {
            return string.Empty;
        }

        return string.Join(Environment.NewLine, schema
            .Where(table => table?.Columns != null && table.Columns.Count > 0)
            .Select(table =>
                $"[{table!.SchemaName}].[{table.TableName}] ({string.Join(", ", table.Columns!.Select(col => col.ColumnName))})"));
    }

}
