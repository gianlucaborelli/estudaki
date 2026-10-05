using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Identity.Domain.Entities;
using Estudaki.Modules.Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Estudaki.Modules.Identity.Adapter.Services;

/// <summary>
/// Implementação do serviço de autenticação de usuários com Cookies.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    /// <summary>
    /// Autentica um usuário com email e senha.
    /// </summary>
    public async Task<ServiceResponse<LoginResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return ServiceResponse<LoginResult>.Fail("Email or password is empty");
                
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return ServiceResponse<LoginResult>.Fail("Email or password invalid");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return ServiceResponse<LoginResult>.Fail("Email or password invalid");

        if (!user.EmailConfirmed)
            return ServiceResponse<LoginResult>.Fail("Email is not confirmed");

        await _signInManager.SignInAsync(user, isPersistent: true);

        var roles = await _userManager.GetRolesAsync(user);

        var login = new LoginResult
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Name = user.Name ?? string.Empty,
            Roles = roles.ToList()
        };

        return ServiceResponse<LoginResult>.Ok(login);
    }

    /// <summary>
    /// Logout de um usuário, removendo sua sessão de cookies.
    /// </summary>
    public async Task LogoutAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        // Fazer logout via SignInManager (remove os cookies)
        await _signInManager.SignOutAsync();
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    public async Task<ServiceResponse> Register(string name, string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return ServiceResponse.Fail("Nome, email e senha são obrigatórios.");

        // Verificar se o usuário já existe
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return ServiceResponse.Fail("Usuário com este email já existe.");

        // Criar novo usuário
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Name = name,
            EmailConfirmed = false // Requerer confirmação de email
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResponse.Fail($"Erro ao criar usuário: {errors}");
        }

        // Atribuir role padrão "User" ao novo usuário
        await _userManager.AddToRoleAsync(user, "User");

        return ServiceResponse.Ok("Usuário registrado com sucesso. Verifique seu email para confirmar a conta.");
    }
}
