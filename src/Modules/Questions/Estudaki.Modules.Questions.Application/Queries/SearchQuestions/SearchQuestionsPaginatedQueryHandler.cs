using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
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
    private readonly IStorageService _storageService;

    public SearchQuestionsPaginatedQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IQuestionSupportRepository questionSupportRepository,
        IStorageService storageService)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _questionSupportRepository = questionSupportRepository;
        _storageService = storageService;
    }

    public async Task<PagedResult<QuestionDto>> HandleAsync(SearchQuestionsPaginatedQuery query, CancellationToken cancellationToken = default)
    {
        var (questionsWithExam, totalItems) = await _questionRepository.FindQuestionsPaginatedAsync(query.SearchParameters);

        if (!questionsWithExam.Any())
        {
            return new PagedResult<QuestionDto>
            {
                Items = [],
                PageNumber = query.SearchParameters.CurrentPage,
                PageSize = query.SearchParameters.PageSize,
                TotalItems = 0
            };
        }

        var examIds = questionsWithExam.Keys
            .Select(eq => eq.ExamId)
            .Distinct()
            .ToList();

        var publicNotices = new List<Domain.Entities.PublicNotice>();
        foreach (var examId in examIds)
        {
            var publicNotice = await _publicNoticeRepository.GetPublicNoticeByExamId(examId);
            if (publicNotice != null)
            {
                publicNotices.Add(publicNotice);
            }
        }

        var examDict = publicNotices
            .SelectMany(pn => pn.Exams.Select(exam => new { ExamId = exam.Id, Exam = exam, PublicNotice = pn }))
            .ToDictionary(x => x.ExamId, x => (x.Exam, x.PublicNotice));

        var allQuestionSupportIds = questionsWithExam.Values
            .Where(q => q.QuestionSupports != null && q.QuestionSupports.Any())
            .SelectMany(q => q.QuestionSupports)
            .Distinct()
            .ToList();

        var questionSupports = allQuestionSupportIds.Any()
            ? await _questionSupportRepository.GetByIds(allQuestionSupportIds)
            : [];

        var dtos = questionsWithExam.Select(kvp =>
        {
            var examQuestion = kvp.Key;
            var question = kvp.Value;

            if (!examDict.TryGetValue(examQuestion.ExamId, out var examData))
            {
                return null;
            }

            return question.ToDto(
                examData.PublicNotice,
                examData.Exam,
                examQuestion,
                questionSupports
            );
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
