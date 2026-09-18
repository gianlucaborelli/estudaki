using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;

public record GetPublicNoticeListQuery(string? Search, string? Category) : PagedQuery, IQuery<PagedResult<PublicNoticeDto>>;

