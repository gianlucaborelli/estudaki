using Estudaki.Commons.Core.CQRS;
using FluentValidation;

namespace Estudaki.Modules.Identity.Application.Commands.Login;

/// <summary>
/// Command para autenticar um usuário com email e senha.
/// </summary>
public record LoginCommand(string Email, string Password) : ICommand<CommandResult>;

public record LoginCommandResult(
    string UserId,
    string Email,
    string Name,
    List<string> Roles
);
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.")
            .EmailAddress()
            .WithMessage("Email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória.")
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres.");
    }
}
