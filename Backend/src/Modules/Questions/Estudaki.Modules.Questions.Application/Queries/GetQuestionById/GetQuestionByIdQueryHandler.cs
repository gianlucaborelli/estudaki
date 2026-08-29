using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionById;

public class GetQuestionByIdQueryHandler : IQueryHandler<GetQuestionByIdQuery, QuestionDto?>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public GetQuestionByIdQueryHandler(
        IQuestionRepository questionRepository,
        IQuestionSupportRepository questionSupportRepository)
    {
        _questionRepository = questionRepository;
        _questionSupportRepository = questionSupportRepository;
    }

    public async Task<QuestionDto?> HandleAsync(GetQuestionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetById(query.Id);

        if (question == null)
            return null;

        // Pegar o primeiro exame associado à questão
        var questionExam = question.Exams.FirstOrDefault();
        if (questionExam == null)
            return null;

        var questionSupports = question.QuestionSupports != null && question.QuestionSupports.Any()
            ? await _questionSupportRepository.GetByIds(question.QuestionSupports)
            : null;

        return question.ToDto(questionExam, questionSupports);
    }
}
