using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("exam_questions")]
public class ExamQuestion : Entity
{
    public string ExamId { get; set; } = default!;
    public string QuestionId { get; set; } = default!;
    public bool IsNullified { get; set; } = false;
    public int QuestionNumber { get; set; }
}
