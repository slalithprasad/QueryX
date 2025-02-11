using System;

namespace QueryX.Models.Database;

public class TableSchema
{
    public string? TableName { get; set; }
    public string? SchemaName { get; set; }
    public List<ColumnSchema>? Columns { get; set; }
}