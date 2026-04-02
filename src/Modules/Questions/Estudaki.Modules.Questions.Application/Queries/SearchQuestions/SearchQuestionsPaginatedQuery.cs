using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;

namespace Estudaki.Modules.Questions.Application.Queries.SearchQuestions;

public record SearchQuestionsPaginatedQuery(SearchParameters SearchParameters) : IQuery<PageResult<QuestionDto>>;
