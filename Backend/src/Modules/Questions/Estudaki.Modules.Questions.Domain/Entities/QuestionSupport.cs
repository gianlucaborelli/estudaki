using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("question_supports")]
public class QuestionSupport : Entity
{
    public string? PublicNoticeId { get; set; } = string.Empty;
    public List<ContentBlock> Contents{ get; set; } = [];
}
