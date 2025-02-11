namespace QueryX.Core.Interfaces;

public interface IQueryXService
{
    Task<(string, IEnumerable<object>)> QueryAsync(string? prompt, CancellationToken cancellationToken = default);
}