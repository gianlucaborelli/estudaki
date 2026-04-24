using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class QuestionSupportDto
{
    public string Id { get; set; } = string.Empty;
    public string? PublicNoticeId { get; set; }
    public List<ContentBlock> Contents { get; set; } = [];
}
