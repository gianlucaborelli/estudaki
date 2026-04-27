using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList
{
    public class GetPublicNoticeListQuery : IQuery<List<PublicNoticeDto>>
    {
        public string? ExamPhase { get; set; }
        public string? ExamBoard { get; set; }
        public string? ExamType { get; set; }
        public string? Position { get; set; }
    }
}
