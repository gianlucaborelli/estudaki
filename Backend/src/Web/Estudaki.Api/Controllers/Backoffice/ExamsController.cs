using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models.DTOs;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetImageListByPublicNoticeId;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;
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
    
    [HttpGet("{publicNoticeId}/contents/images")]
    public async Task<IActionResult> GetImagensFromPublicNoticeId([FromRoute] string publicNoticeId)
    {
        var query = new GetImageListByPublicNoticeIdQuery(publicNoticeId);
        var result = await _queryDispatcher.DispatchAsync<GetImageListByPublicNoticeIdQuery, List<string>>(query);        
        return Ok(result);
    }

    [HttpPost("{publicNoticeId}/contents/images")]
    public async Task<IActionResult> UploadImagensFromPublicNoticeId([FromRoute] string publicNoticeId, [FromForm] IFormFile? file)
    {
        var imageList = new List<UploadFileDto>();        

        var uploadFileDto = await UploadFileDto.CreateAsync(file);
        imageList.Add(uploadFileDto);
        
        var command = new UploadQuestionImagesCommand(imageList, publicNoticeId);
        var result = await _commandDispatcher.DispatchAsync<UploadQuestionImagesCommand, CommandResult>(command);        
        return Ok(result);
    }

    public sealed class UploadImageRequest
    {
        public IFormFile? File { get; set; }
    }

    [HttpGet("{publicNoticeId}/contents/questions-support")]
    public async Task<IActionResult> GetQuestionSupportByPublicNoticeId([FromRoute]string publicNoticeId, [FromQuery]GetQuestionSupportsByPublicNoticeIdQuery query)
    {
        query = query with
        {
            PublicNoticeId = publicNoticeId
        };
        var result = await _queryDispatcher.DispatchAsync<GetQuestionSupportsByPublicNoticeIdQuery, PagedResult<QuestionSupportDto>>(query);
        return Ok(result);
    }

    [HttpGet("{publicNoticeId}/exams/{examId}/questions")]
    public async Task<IActionResult> GetQuestionsByExamId([FromRoute] string publicNoticeId, [FromRoute] string examId, [FromQuery] GetQuestionsByExamIdQuery query)
    {
        query = query with
        {
            ExamId = examId
        };
        var result = await _queryDispatcher.DispatchAsync<GetQuestionsByExamIdQuery, PagedResult<QuestionDto>>(query);        
        return Ok(result);
    }    
}
