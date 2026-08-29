using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList
{
    public class GetPublicNoticeListQuery : IQuery<PagedResult<PublicNoticeDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? SortLabel { get; set; }
        public string? SortDirection { get; set; }        
    }
}
