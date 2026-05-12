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

        // Buscar todos os exames dos PublicNotices relacionados
        var examIds = questions
            .SelectMany(q => q.Exams)
            .Select(qe => qe.ExamId)
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

        var allQuestionSupportIds = questions
            .Where(q => q.QuestionSupports != null && q.QuestionSupports.Any())
            .SelectMany(q => q.QuestionSupports)
            .Distinct()
            .ToList();

        var questionSupports = allQuestionSupportIds.Any()
            ? await _questionSupportRepository.GetByIds(allQuestionSupportIds)
            : [];

        var dtos = questions.SelectMany(question =>
        {
            // Para cada questão, criar um DTO para cada exame associado
            // (ou apenas o primeiro se preferir mostrar uma única vez)
            var firstExam = question.Exams.FirstOrDefault();
            if (firstExam == null || !examDict.TryGetValue(firstExam.ExamId, out var examData))
            {
                return Enumerable.Empty<QuestionDto>();
            }

            return new[]
            {
                question.ToDto(
                    examData.PublicNotice,
                    examData.Exam,
                    firstExam,
                    questionSupports
                )
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
