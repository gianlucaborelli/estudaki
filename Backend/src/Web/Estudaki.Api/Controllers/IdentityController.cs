using System.Runtime.InteropServices;
using System.Security.Claims;
using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Identity.Application.Commands.Login;
using Estudaki.Modules.Identity.Application.Commands.Register;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estudaki.Api.Controllers;

[Route("api/[controller]")]
public class IdentityController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, ILogger<IdentityController> logger) : ControllerBase
{
    private readonly ILogger<IdentityController> Logger = logger;
    public ICommandDispatcher CommandDispatcher = commandDispatcher;
    public IQueryDispatcher QueryDispatcher = queryDispatcher;

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await CommandDispatcher.DispatchAsync<LoginCommand, CommandResult>(command);
        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet("Me")]
    public IActionResult Me()
    {
        Logger.LogInformation("User {UserId} is authenticated", User.FindFirstValue(ClaimTypes.NameIdentifier));
        return Ok(new
        {
            IsAuthenticated = true,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await CommandDispatcher.DispatchAsync<RegisterUserCommand, CommandResult>(command);
        return Ok(result);
    }
}
