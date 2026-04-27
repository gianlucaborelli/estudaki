using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;

public class GetQuestionsByPublicNoticeIdQueryHandler(
    IQuestionRepository questionRepository,
    IQuestionSupportRepository questionSupportRepository,
    IPublicNoticeRepository publicNoticeRepository,
    IStorageService storageService) : IQueryHandler<GetQuestionsByPublicNoticeIdQuery, List<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IStorageService _storageService = storageService;
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;

    public async Task<List<QuestionDto>> HandleAsync(GetQuestionsByPublicNoticeIdQuery query, CancellationToken cancellationToken = default)
    {
        var questions = await _questionRepository.GetByPublicNoticeId(query.PublicNoticeId);
        var questionSupports = await _questionSupportRepository.GetByPublicNoticeId(query.PublicNoticeId);
        var publicNotice = await _publicNoticeRepository.GetById(query.PublicNoticeId);

        return questions.ToDtoList(publicNotice, questionSupports, _storageService);
    }
}
