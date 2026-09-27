using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetAllQuestions;

public class GetAllQuestionsQueryHandler : IQueryHandler<GetAllQuestionsQuery, List<QuestionSitemapDto>>
{
    private readonly IQuestionRepository _questionRepository;

    public GetAllQuestionsQueryHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<List<QuestionSitemapDto>> HandleAsync(GetAllQuestionsQuery query, CancellationToken cancellationToken = default)
    {
        var publishedQuestions = await _questionRepository.GetPublishedQuestionsForSitemapAsync();

        if (!publishedQuestions.Any())
        {
            return [];
        }

        var dtos = publishedQuestions
            .Select(q => new QuestionSitemapDto
            {
                QuestionId = q.QuestionId,
                CreatedAt = q.CreatedAt
            })
            .ToList();

        return dtos;
    }
}

