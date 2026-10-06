using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("question_supports")]
public class QuestionSupport : Entity
{
    public string? PublicNoticeId { get; set; } = string.Empty;
    public string? Content { get; set; }
}
