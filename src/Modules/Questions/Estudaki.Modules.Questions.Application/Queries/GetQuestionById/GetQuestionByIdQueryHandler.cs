using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionById;

public class GetQuestionByIdQueryHandler : IQueryHandler<GetQuestionByIdQuery, QuestionDto?>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository;
    private readonly IExamQuestionRepository _examQuestionRepository;
    private readonly IStorageService _storageService;

    public GetQuestionByIdQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IQuestionSupportRepository questionSupportRepository,
        IExamQuestionRepository examQuestionRepository,
        IStorageService storageService)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _questionSupportRepository = questionSupportRepository;
        _examQuestionRepository = examQuestionRepository;
        _storageService = storageService;
    }

    public async Task<QuestionDto?> HandleAsync(GetQuestionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetById(query.Id);

        if (question == null)
            return null;

        var examQuestions = await _examQuestionRepository.GetByQuestionId(query.Id);

        if (!examQuestions.Any())
            return null;

        var examQuestion = examQuestions.First();
        var publicNotice = await _publicNoticeRepository.GetPublicNoticeByExamId(examQuestion.ExamId);

        if (publicNotice == null)
            return null;

        var exam = publicNotice.Exams.FirstOrDefault(e => e.Id == examQuestion.ExamId);

        if (exam == null)
            return null;

        var questionSupports = question.QuestionSupports != null && question.QuestionSupports.Any()
            ? await _questionSupportRepository.GetByIds(question.QuestionSupports)
            : null;

        return question.ToDto(publicNotice, exam, examQuestion, questionSupports);
    }
}
