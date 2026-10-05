namespace Estudaki.Commons.Core.CQRS;

/// <summary>
/// Interface for queries that return a result.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
public interface IQuery<out TResult>
{
}
