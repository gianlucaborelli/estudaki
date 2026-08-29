using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;

public record GetQuestionSupportsByPublicNoticeIdQuery(string PublicNoticeId) : IQuery<List<QuestionSupportDto>>;
