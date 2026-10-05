using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Comunications.Application.Commands.CreateContactMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estudaki.Api.Controllers;

[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public ContactsController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactMessageCommand command)
    {
        var result = await _commandDispatcher.DispatchAsync<CreateContactMessageCommand, CommandResult>(command);

        return result.ValidationResult.IsValid ? Ok() : BadRequest(result.ValidationResult);
    }
}
