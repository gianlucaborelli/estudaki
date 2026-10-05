using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;

public record GetQuestionsByPublicNoticeIdQuery(string PublicNoticeId) : IQuery<List<QuestionDto>>;
