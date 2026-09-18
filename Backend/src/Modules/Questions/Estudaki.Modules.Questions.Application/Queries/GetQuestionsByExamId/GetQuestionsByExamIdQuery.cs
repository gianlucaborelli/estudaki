using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;

public record GetQuestionsByExamIdQuery(string ExamId) : PagedQuery, IQuery<PagedResult<QuestionDto>>;