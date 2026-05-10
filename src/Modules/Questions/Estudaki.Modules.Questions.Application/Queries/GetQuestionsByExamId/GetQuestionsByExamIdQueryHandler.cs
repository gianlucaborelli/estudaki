using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;

public class GetQuestionsByExamIdQueryHandler(
    IQuestionRepository questionRepository,
    IExamQuestionRepository examQuestionRepository,
    IQuestionSupportRepository questionSupportRepository,
    IPublicNoticeRepository publicNoticeRepository) : IQueryHandler<GetQuestionsByExamIdQuery, List<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IExamQuestionRepository _examQuestionRepository = examQuestionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;

    public async Task<List<QuestionDto>> HandleAsync(GetQuestionsByExamIdQuery query, CancellationToken cancellationToken = default)
    {
        var examQuestions = await _examQuestionRepository.GetByExamId(query.ExamId);
        var questions = new List<Question>();
        foreach (var examQuestion in examQuestions)
        {
            var question = await _questionRepository.GetById(examQuestion.QuestionId);
            if (question != null)
            {
                questions.Add(question);
            }
        }
        
        var publicNotice = await _publicNoticeRepository.GetPublicNoticeByExamId(query.ExamId);
        var questionSupports = await _questionSupportRepository.GetByPublicNoticeId(publicNotice.Id);        

        var questionsDto = new List<QuestionDto>();

        foreach (var question in questions) 
        { 
            questionsDto.Add(
                question.ToDto(
                    publicNotice, 
                    publicNotice.Exams.FirstOrDefault(e => e.Id == query.ExamId)!, 
                    examQuestions.FirstOrDefault(eq => eq.QuestionId == question.Id)!,
                    questionSupports
                ));
        }

        return questionsDto;
    }
}
