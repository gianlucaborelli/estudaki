using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Identity.Application.Commands.Login;
using Estudaki.Modules.Identity.Domain.Interfaces;

namespace Estudaki.Modules.Identity.Application.Commands;

/// <summary>
/// Handler para o comando de login.
/// </summary>
public class LoginCommandHandler : CommandHandler, ICommandHandler<LoginCommand, CommandResult>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    public async Task<CommandResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var result = await _authenticationService.LoginAsync(command.Email, command.Password, cancellationToken);

        if (!result.Success)
        {
            AddError(result.Message!);
            return Result();
        }

        return Result(new LoginCommandResult(
            result.Data.UserId,
            result.Data.Email,
            result.Data.Name,
            result.Data.Roles)
        );
    }
}
