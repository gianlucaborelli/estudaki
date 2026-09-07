using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Identity.Domain.Interfaces;
using FluentValidation.Results;

namespace Estudaki.Modules.Identity.Application.Commands.Register;

public class RegisterUserCommandHandler : CommandHandler, ICommandHandler<RegisterUserCommand, ValidationResult>
{
    private readonly IAuthenticationService _authenticationService;

    public RegisterUserCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    public async Task<ValidationResult> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _authenticationService.Register(command.Name, command.Email, command.Password, cancellationToken);

        if (result.Success)
            return new ValidationResult();

        AddError(result.Message ?? "An error occurred while registering the user.");

        return ValidationResult;
    }
}
