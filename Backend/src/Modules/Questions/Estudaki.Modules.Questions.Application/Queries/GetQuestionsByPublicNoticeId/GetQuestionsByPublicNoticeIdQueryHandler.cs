using System;
using System.Collections.Generic;
using System.Text;
using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;

public class GetQuestionsByPublicNoticeIdQueryHandler : IQueryHandler<GetQuestionsByPublicNoticeIdQuery, List<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public GetQuestionsByPublicNoticeIdQueryHandler(
        IQuestionRepository questionRepository, 
        IPublicNoticeRepository publicNoticeRepository, 
        IQuestionSupportRepository questionSupportRepository)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _questionSupportRepository = questionSupportRepository;
    }

    public async Task<List<QuestionDto>> HandleAsync(GetQuestionsByPublicNoticeIdQuery query, CancellationToken cancellationToken = default)
    {
        var publicNotice = await _publicNoticeRepository.GetById(query.PublicNoticeId);
        var questionSupports = await _questionSupportRepository.GetByPublicNoticeId(query.PublicNoticeId);
        var questions = await _questionRepository.GetByPublicNoticeId(query.PublicNoticeId);

        var mappedQuestions = questions.Select(q => q.ToDto(publicNotice)).ToList();

        return mappedQuestions;
    }
}
