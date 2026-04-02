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
    private readonly IStorageService _storageService;
    private readonly StorageSettings _storageSettings;

    public GetQuestionByIdQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IStorageService storageService,
        StorageSettings storageSettings)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _storageService = storageService;
        _storageSettings = storageSettings;
    }

    public async Task<QuestionDto?> HandleAsync(GetQuestionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.GetById(query.Id);

        if (question == null)
            return null;

        var publicNotice = !string.IsNullOrEmpty(question.PublicNoticeId)
            ? await _publicNoticeRepository.GetById(question.PublicNoticeId)
            : null;

        return question.ToDto(publicNotice, _storageService, _storageSettings);
    }
}
