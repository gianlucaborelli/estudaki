using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;
using Estudaki.Modules.Questions.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estudaki.Api.Controllers.Backoffice;

[Controller]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Backoffice")]
public class ExamsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;

    [HttpGet]
    public async Task<IActionResult> GetPublicNotices([FromQuery] GetPublicNoticeListQuery query)
    {       
        var result = await _queryDispatcher.DispatchAsync<GetPublicNoticeListQuery, PagedResult<PublicNoticeDto>>(query);
        return Ok(result);
    }

    [HttpGet("{publicNoticeId}")]
    public async Task<IActionResult> GetPublicNoticeById([FromRoute] string publicNoticeId)
    {
        var query = new GetPublicNoticeByIdQuery(publicNoticeId);
        var result = await _queryDispatcher.DispatchAsync<GetPublicNoticeByIdQuery, PublicNoticeDto?>(query);
        if(result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("{publicNoticeId}/exam/{examId}/questions")]
    public async Task<IActionResult> GetQuestionsByExamId([FromRoute] string publicNoticeId, [FromRoute] string examId)
    {
        var query = new GetQuestionsByExamIdQuery(examId);
        var result = await _queryDispatcher.DispatchAsync<GetQuestionsByExamIdQuery, List<QuestionDto>>(query);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
