using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.SearchQuestions;

public class SearchQuestionsPaginatedQueryHandler : IQueryHandler<SearchQuestionsPaginatedQuery, PagedResult<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public SearchQuestionsPaginatedQueryHandler(
        IQuestionRepository questionRepository,
        IQuestionSupportRepository questionSupportRepository)
    {
        _questionRepository = questionRepository;
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

        var questionSupports = allQuestionSupportIds.Any()
            ? await _questionSupportRepository.GetByIds(allQuestionSupportIds)
            : [];

        // Converter para DTO usando dados desnormalizados
        var dtos = questions.SelectMany(question =>
        {
            // Para cada questão, criar um DTO para cada exame associado
            // (ou apenas o primeiro se preferir mostrar uma única vez)
            var firstExam = question.Exams.FirstOrDefault();
            if (firstExam == null)
            {
                return Enumerable.Empty<QuestionDto>();
            }

            return new[]
            {
                question.ToDto(firstExam, questionSupports)
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
