using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetExamExtractionList;

public record GetExamExtractionListQuery : IQuery<List<ExamExtractionDto>>;
