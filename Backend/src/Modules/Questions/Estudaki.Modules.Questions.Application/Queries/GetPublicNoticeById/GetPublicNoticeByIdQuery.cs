using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
public record GetPublicNoticeByIdQuery(string Id) : IQuery<PublicNoticeDto?>;
