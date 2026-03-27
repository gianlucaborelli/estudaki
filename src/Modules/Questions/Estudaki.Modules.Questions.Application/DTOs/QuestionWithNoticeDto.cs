using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class QuestionWithNoticeDto
{
    public Question Question { get; set; } = null!;
    public PublicNotice? PublicNotice { get; set; }
}
