using Estudaki.Commons.Core.CQRS;

namespace Estudaki.Modules.Questions.Application.Queries.GetImageListByPublicNoticeId;

public record GetImageListByPublicNoticeIdQuery(string PublicNoticeId) : IQuery<List<string>>;
