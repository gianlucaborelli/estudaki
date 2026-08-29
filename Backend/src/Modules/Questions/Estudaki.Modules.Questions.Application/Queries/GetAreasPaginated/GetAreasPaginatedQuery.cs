using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.Queries.GetAreasPaginated;

public record GetAreasPaginatedQuery(AreaType Type, string? Name, int PageNumber = 1, int PageSize = 10)
    : IQuery<PagedResult<AreaDto>>;
