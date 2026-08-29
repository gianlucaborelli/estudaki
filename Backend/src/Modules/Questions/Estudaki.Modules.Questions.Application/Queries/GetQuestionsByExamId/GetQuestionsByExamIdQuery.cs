using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;

public record GetQuestionsByExamIdQuery(string ExamId) : IQuery<List<QuestionDto>>;