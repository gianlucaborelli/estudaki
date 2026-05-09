using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities2;

[CollectionName("exams")]
public class Exam : Entity
{
    public string PublicNoticeId { get; set; } = default!;
    public string Phase { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string ExamBookletUrl { get; set; } = string.Empty;
    public string AnswerKeyUrl { get; set; } = string.Empty;
    public List<AnswerKeyItem> AnswerKeyItems { get; set; } = new List<AnswerKeyItem>();
}
