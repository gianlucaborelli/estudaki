using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetExamExtractionList;

public class GetExamExtractionListQueryHandler(IExamExtractionRepository examExtractionRepository) : IQueryHandler<GetExamExtractionListQuery, List<ExamExtractionDto>>
{
    IExamExtractionRepository _examExtractionRepository { get; set; } = examExtractionRepository;

    public async Task<List<ExamExtractionDto>> HandleAsync(GetExamExtractionListQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _examExtractionRepository.GetAll();
        var resultDto = result.Select(extraction => extraction.ToDto()).ToList();
        return resultDto;
    }
}
