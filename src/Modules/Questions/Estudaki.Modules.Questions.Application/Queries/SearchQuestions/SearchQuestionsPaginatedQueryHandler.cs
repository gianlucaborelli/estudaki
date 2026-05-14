using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.SearchQuestions;

public class SearchQuestionsPaginatedQueryHandler : IQueryHandler<SearchQuestionsPaginatedQuery, PagedResult<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public SearchQuestionsPaginatedQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IQuestionSupportRepository questionSupportRepository)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _questionSupportRepository = questionSupportRepository;
    }

    public async Task<PagedResult<QuestionDto>> HandleAsync(SearchQuestionsPaginatedQuery query, CancellationToken cancellationToken = default)
    {
        var (questions, totalItems) = await _questionRepository.FindQuestionsPaginatedAsync(query.SearchParameters);

        if (!questions.Any())
        {
            return new PagedResult<QuestionDto>
            {
                Items = [],
                PageNumber = query.SearchParameters.CurrentPage,
                PageSize = query.SearchParameters.PageSize,
                TotalItems = 0
            };
        }

        // Buscar todos os question supports necessários
        var allQuestionSupportIds = questions
            .Where(q => q.QuestionSupports != null && q.QuestionSupports.Any())
            .SelectMany(q => q.QuestionSupports)
            .Distinct()
            .ToList();

        var allPublicNotice = questions
            .Where(q => q.Exams != null && q.Exams.Any())
            .SelectMany(q => q.Exams)
            .Select(e => e.PublicNoticeId)
            .Distinct()
            .ToList();

        var publicNoticeSupports = allPublicNotice.Any()
            ? await _publicNoticeRepository.GetByIds(allPublicNotice)
            : [];

        var questionSupports = allQuestionSupportIds.Any()
            ? await _questionSupportRepository.GetByIds(allQuestionSupportIds)
            : [];

        var dtos = questions.SelectMany(question =>
        {
            var firstExam = question.Exams.FirstOrDefault();
            
            if (firstExam == null)
                return Enumerable.Empty<QuestionDto>();

            var publicNoticeSupport = publicNoticeSupports.FirstOrDefault(p => p.Id == firstExam.PublicNoticeId);

            return new[]
            {
                question.ToDto(publicNoticeSupport, firstExam, questionSupports)
            };
        })
        .Where(dto => dto != null)
        .ToList();

        return new PagedResult<QuestionDto>
        {
            Items = dtos!,
            PageNumber = query.SearchParameters.CurrentPage,
            PageSize = query.SearchParameters.PageSize,
            TotalItems = totalItems
        };
    }
}
