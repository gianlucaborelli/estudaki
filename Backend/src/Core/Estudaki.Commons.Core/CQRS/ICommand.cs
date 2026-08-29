namespace Estudaki.Commons.Core.CQRS;

/// <summary>
/// Marker interface for commands that do not return a result.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Interface for commands that return a result.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the command.</typeparam>
public interface ICommand<out TResult>
{
}
