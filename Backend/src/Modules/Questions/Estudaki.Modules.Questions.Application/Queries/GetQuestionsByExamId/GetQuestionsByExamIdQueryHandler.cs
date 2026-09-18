using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;

public class GetQuestionsByExamIdQueryHandler(
    IQuestionRepository questionRepository,
    IQuestionSupportRepository questionSupportRepository,
    IPublicNoticeRepository publicNoticeRepository) : IQueryHandler<GetQuestionsByExamIdQuery, PagedResult<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;

    public async Task<PagedResult<QuestionDto>> HandleAsync(GetQuestionsByExamIdQuery query, CancellationToken cancellationToken = default)
    {
        var (questions, totalCount) = await _questionRepository.GetByExamIdPaged(query.ExamId, query.Page, query.PageSize, query.SortLabel, query.SortDirection);

        var publicNotice = await _publicNoticeRepository.GetPublicNoticeByExamId(query.ExamId);
        var questionSupports = await _questionSupportRepository.GetAllByPublicNoticeIdAsync(publicNotice.Id);        

        var questionsDto = new List<QuestionDto>();

        foreach (var question in questions) 
        {
            var questionExam = question.Exams.FirstOrDefault(qe => qe.ExamId == query.ExamId);
            if (questionExam != null)
            {
                questionsDto.Add(question.ToDto(questionExam, questionSupports));
            }
        }

        var pagedResult = new PagedResult<QuestionDto>
        {
            Items = questionsDto,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalCount
        };

        return pagedResult;
    }
}
