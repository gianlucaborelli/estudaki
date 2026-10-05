namespace Estudaki.Modules.Questions.Application.DTOs;

public record QuestionSitemapDto
{
    public string QuestionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
