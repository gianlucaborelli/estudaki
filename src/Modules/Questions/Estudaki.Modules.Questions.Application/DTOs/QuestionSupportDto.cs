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

    public string GetSupportPreview()
    {
        if (!Contents.Any()) return string.Empty;

        var previewTexts = new List<string>();
        const int maxLength = 200; // Máximo de caracteres no preview
        var currentLength = 0;

        foreach (var block in Contents.OrderBy(c => c.Order))
        {
            if (currentLength >= maxLength) break;

            if (block is ParagraphBlock paragraph)
            {
                foreach (var inline in paragraph.Inlines)
                {
                    if (currentLength >= maxLength) break;

                    if (inline is TextInline textInline && !string.IsNullOrWhiteSpace(textInline.Text))
                    {
                        var remainingLength = maxLength - currentLength;
                        var textToAdd = textInline.Text.Length <= remainingLength
                            ? textInline.Text
                            : textInline.Text.Substring(0, remainingLength) + "...";

                        previewTexts.Add(textToAdd);
                        currentLength += textToAdd.Length;
                    }
                    else if (inline is ImageInline imageInline)
                    {
                        previewTexts.Add($"[Imagem: {imageInline.Key}]");
                        currentLength += 20;
                    }
                }
            }
            else if (block is ImageBlock imageBlock)
            {
                previewTexts.Add($"[Imagem: {imageBlock.Key}]");
                currentLength += 20;
            }
        }

        return string.Join(" ", previewTexts).Trim();
    }
}
