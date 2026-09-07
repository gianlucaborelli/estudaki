using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Identity.Application.Commands.Login;
using Estudaki.Modules.Identity.Application.Commands.Register;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Estudaki.Api.Controllers;

[Route("api/[controller]")]
public class IdentityController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : ControllerBase
{
    public ICommandDispatcher CommandDispatcher = commandDispatcher;
    public IQueryDispatcher QueryDispatcher = queryDispatcher;

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await CommandDispatcher.DispatchAsync<LoginCommand, LoginCommandResult>(command);
        return Ok(result);
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await CommandDispatcher.DispatchAsync<RegisterUserCommand, ValidationResult>(command);
        return Ok(result);
    }
}
