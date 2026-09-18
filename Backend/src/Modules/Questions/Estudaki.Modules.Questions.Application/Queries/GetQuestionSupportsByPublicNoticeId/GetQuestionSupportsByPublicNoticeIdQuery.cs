using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;

public record GetQuestionSupportsByPublicNoticeIdQuery(string PublicNoticeId) : PagedQuery, IQuery<PagedResult<QuestionSupportDto>>;
