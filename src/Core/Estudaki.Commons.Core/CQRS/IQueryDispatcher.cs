namespace Estudaki.Commons.Core.CQRS;

/// <summary>
/// Dispatcher for executing queries.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query and returns a result.
    /// </summary>
    /// <typeparam name="TQuery">The type of query.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the query execution.</returns>
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}
