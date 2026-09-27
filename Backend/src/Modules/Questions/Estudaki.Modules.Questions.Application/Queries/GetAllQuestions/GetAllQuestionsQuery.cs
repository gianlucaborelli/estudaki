using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetAllQuestions;

public record GetAllQuestionsQuery : IQuery<List<QuestionSitemapDto>>;
