using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionById;

public class GetQuestionByIdQueryHandler : IQueryHandler<GetQuestionByIdQuery, QuestionWithNoticeDto?>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;

    public GetQuestionByIdQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
    }

    public async Task<QuestionWithNoticeDto?> HandleAsync(GetQuestionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetByIdAsync(query.Id);

        if (question == null)
            return null;

        var dto = new QuestionWithNoticeDto
        {
            Question = question
        };

        if (!string.IsNullOrEmpty(question.PublicNoticeId))
        {
            dto.PublicNotice = await _publicNoticeRepository.GetByIdAsync(question.PublicNoticeId);
        }

        return dto;
    }
}
