using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Identity.Domain.Entities;

namespace Estudaki.Modules.Identity.Domain.Interfaces;

/// <summary>
/// Interface para serviço de autenticação de usuários com Cookies.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica um usuário com email e senha, gerando cookie httpOnly.
    /// </summary>
    Task<ServiceResponse<LoginResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout de um usuário, removendo seu cookie de autenticação.
    /// </summary>
    Task LogoutAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    Task<ServiceResponse> Register(string name, string email, string password, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado de login com informações do usuário autenticado.
/// </summary>
public class LoginResult
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public List<string> Roles { get; set; } = new();
}
