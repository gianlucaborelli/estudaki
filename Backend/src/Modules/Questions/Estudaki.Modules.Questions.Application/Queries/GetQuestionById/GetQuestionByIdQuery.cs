using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionById;

public record GetQuestionByIdQuery(string Id) : IQuery<QuestionDto?>;
