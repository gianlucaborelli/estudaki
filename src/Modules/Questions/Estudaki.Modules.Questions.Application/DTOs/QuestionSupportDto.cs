using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class QuestionSupportDto
{
    public string Id { get; set; } = string.Empty;
    public string? PublicNoticeId { get; set; }
    public List<ContentBlock> Contents { get; set; } = [];

    public static QuestionSupportDto Clone(QuestionSupportDto questionSupport) {         
        return new QuestionSupportDto
        {
            Id = questionSupport.Id,
            PublicNoticeId = questionSupport.PublicNoticeId,
            Contents = questionSupport.Contents
                        .Select<ContentBlock, ContentBlock>(c => c switch
                        {
                            ParagraphBlock p => new ParagraphBlock
                            {
                                Inlines = p.Inlines,
                                Title = p.Title,
                                Order = p.Order
                            },

                            ImageBlock i => new ImageBlock
                            {
                                Key = i.Key,
                                Title = i.Title,
                                Source = i.Source,
                                Description = i.Description,
                                Order = i.Order
                            },

                            _ => throw new NotSupportedException()
                        }).ToList(),
        };
    }
}
