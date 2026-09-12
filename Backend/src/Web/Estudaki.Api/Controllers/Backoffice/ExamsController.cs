using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
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
    public async Task<IActionResult> GetExams([FromQuery] GetPublicNoticeListQuery query)
    {       
        var result = await _queryDispatcher.DispatchAsync<GetPublicNoticeListQuery, PagedResult<PublicNoticeDto>>(query);
        return Ok(result);
    }
}
