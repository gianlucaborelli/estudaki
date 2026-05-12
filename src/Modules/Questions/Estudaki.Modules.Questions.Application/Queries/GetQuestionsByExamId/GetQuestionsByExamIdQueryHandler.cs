using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;

public class GetQuestionsByExamIdQueryHandler(
    IQuestionRepository questionRepository,
    IQuestionSupportRepository questionSupportRepository,
    IPublicNoticeRepository publicNoticeRepository) : IQueryHandler<GetQuestionsByExamIdQuery, List<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;

    public async Task<List<QuestionDto>> HandleAsync(GetQuestionsByExamIdQuery query, CancellationToken cancellationToken = default)
    {
        // Buscar questões que contém este examId
        var questions = await _questionRepository.GetByExamId(query.ExamId);

        var publicNotice = await _publicNoticeRepository.GetPublicNoticeByExamId(query.ExamId);
        var questionSupports = await _questionSupportRepository.GetByPublicNoticeId(publicNotice.Id);        

        var questionsDto = new List<QuestionDto>();

        foreach (var question in questions) 
        {
            // Encontrar o QuestionExam correspondente a este examId
            var questionExam = question.Exams.FirstOrDefault(qe => qe.ExamId == query.ExamId);
            if (questionExam != null)
            {
                questionsDto.Add(question.ToDto(questionExam, questionSupports));
            }
        }

        return questionsDto;
    }
}
