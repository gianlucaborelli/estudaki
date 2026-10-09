using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands.CreateQuestionIssue;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionById;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estudaki.Api.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class QuestionsController (ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : Controller
{
    public ICommandDispatcher CommandDispatcher = commandDispatcher;
    public IQueryDispatcher QueryDispatcher = queryDispatcher;

    [HttpGet]
    public async Task<IActionResult> GetQuestions([FromQuery] FilterParameters query)
    {
        var queryRequest = new SearchQuestionsPaginatedQuery(query);
        var result = await QueryDispatcher.DispatchAsync<SearchQuestionsPaginatedQuery, PagedResult<QuestionDto>>(queryRequest);
        return Ok(result);
    }

    [HttpGet("parameters")]
    public async Task<IActionResult> GetParameters([FromQuery] FilterParameters query)
    {
        var queryRequest = new GetFilterParametersQuery(query);
        var result = await QueryDispatcher.DispatchAsync<GetFilterParametersQuery, FilterParameters>(queryRequest);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestionById(string id)
    {
        var queryRequest = new GetQuestionByIdQuery(id);
        var result = await QueryDispatcher.DispatchAsync<GetQuestionByIdQuery, QuestionDto?>(queryRequest);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id}/issues")]
    public async Task<IActionResult> CreateIssue([FromRoute] string id, [FromBody] CreateQuestionIssueCommand command)
    {
        command.QuestionId = id;
        var result = await CommandDispatcher.DispatchAsync<CreateQuestionIssueCommand, CommandResult>(command);
        return result.Success ? Ok(result.Data) : BadRequest(result.ValidationResult.Errors);
    }
}
