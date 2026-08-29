using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Common;

namespace Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;

public record GetFilterParametersQuery(FilterParameters FilterParameters) : IQuery<FilterParameters>;
