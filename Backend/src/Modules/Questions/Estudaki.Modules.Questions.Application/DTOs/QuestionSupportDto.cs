using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class QuestionSupportDto
{
    public string Id { get; set; } = string.Empty;
    public string? PublicNoticeId { get; set; }
    public string? Content { get; set; }

    public static QuestionSupportDto Clone(QuestionSupportDto questionSupport) {         
        return new QuestionSupportDto
        {
            Id = questionSupport.Id,
            PublicNoticeId = questionSupport.PublicNoticeId,
            Content = questionSupport.Content,
        };
    }    
}