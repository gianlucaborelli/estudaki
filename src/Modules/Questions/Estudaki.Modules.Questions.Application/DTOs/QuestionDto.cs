using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class QuestionDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string? PublicNoticeId { get; set; }
    public string ExamId { get; set; } = string.Empty;
    public string? PublicNoticeNumber { get; set; }    
    public int Year { get; set; }
    public string? ExaminerOrganization { get; set; }
    public string? ContractingOrganization { get; set; }    
    public string ExamCategory { get; set; } = ExamCategories.PublicServiceExam;
    public string Phase { get; set; } = string.Empty;
    public List<string> Positions { get; set; } = [];
    public string Area { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string PublicNoticeFileUrl { get; set; } = string.Empty;
    public string ExamBookletUrl { get; set; } = string.Empty;
    public string AnswerKeyUrl { get; set; } = string.Empty;    
    public bool? IsNullified { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionType { get; set; } = string.Empty;    
    public string MainArea { get; set; } = string.Empty;
    public string[] SubAreas { get; set; } = [];    
    public List<ContentBlock> QuestionContents { get; set; } = [];
    public List<QuestionSupportDto> QuestionSupports { get; set; } = [];
    public List<Choice>? Choices { get; set; }
    public DateTime CreatedAt { get; set; }

    public static QuestionDto Create(PublicNoticeDto publicNotice, Exam exam)
    {
        var question = new QuestionDto 
        {
            ExamId = exam.Id,
            PublicNoticeId = publicNotice.Id,
            PublicNoticeNumber = publicNotice.Number,
            Year = publicNotice.Year,
            ExaminerOrganization = publicNotice.ExaminerOrganization,
            ContractingOrganization = publicNotice.ContractingOrganization,
            ExamCategory = publicNotice.ExamCategory ?? ExamCategories.PublicServiceExam,
            Phase = exam.Phase,
            Positions = new List<string> { exam.Position },
            Area = exam.Area,
            EducationLevel = exam.EducationLevel,
            PublicNoticeFileUrl = publicNotice.FileUrl ?? string.Empty,
            ExamBookletUrl = exam.ExamBookletUrl ?? string.Empty,
            AnswerKeyUrl = exam.AnswerKeyUrl ?? string.Empty
        };
        return question;
    }

    public static QuestionDto Clone(QuestionDto original)
    {
        return new QuestionDto
        {
            QuestionId = original.QuestionId,
            PublicNoticeId = original.PublicNoticeId,
            ExamId = original.ExamId,
            PublicNoticeNumber = original.PublicNoticeNumber,
            Year = original.Year,
            ExaminerOrganization = original.ExaminerOrganization,
            ContractingOrganization = original.ContractingOrganization,
            ExamCategory = original.ExamCategory,
            Phase = original.Phase,
            Positions = original.Positions.ToList(),
            Area = original.Area,
            EducationLevel = original.EducationLevel,
            PublicNoticeFileUrl = original.PublicNoticeFileUrl,
            ExamBookletUrl = original.ExamBookletUrl,
            AnswerKeyUrl = original.AnswerKeyUrl,
            IsNullified = original.IsNullified,
            QuestionNumber = original.QuestionNumber,
            QuestionType = original.QuestionType,
            MainArea = original.MainArea,
            SubAreas = (string[])original.SubAreas.Clone(),
            QuestionSupports = original.QuestionSupports
                        .Select(s => new QuestionSupportDto
                        {
                            Contents = s.Contents,
                            Id = s.Id,
                            PublicNoticeId = s.PublicNoticeId
                        }).ToList(),
            QuestionContents = original.QuestionContents
                        .Select<ContentBlock, ContentBlock>(c => c switch
                        {
                            ParagraphBlock p => new ParagraphBlock
                            {
                                Inlines = p.Inlines,
                                Title = p.Title,
                                Source = p.Source,
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

            Choices = original.Choices?
                        .Select(c => 
                            new Choice
                            {
                                Option = c.Option,
                                Content = c.Content,
                                IsCorrect = c.IsCorrect
                            }).ToList(),
            CreatedAt = original.CreatedAt
        };
    }

    /// <summary>
    /// Retorna um preview/resumo da questão para exibição em listas ou cards.
    /// Extrai os primeiros caracteres do conteúdo da questão.
    /// </summary>
    /// <param name="maxLength">Tamanho máximo do preview (padrão: 200 caracteres)</param>
    /// <returns>Texto resumido do conteúdo da questão</returns>
    public string GetQuestionPreview(int maxLength = 200)
    {
        if (!QuestionContents.Any()) return "Questão sem conteúdo disponível.";

        var previewTexts = new List<string>();
        var currentLength = 0;

        foreach (var block in QuestionContents.OrderBy(c => c.Order))
        {
            if (currentLength >= maxLength) break;

            if (block is ParagraphBlock paragraph)
            {
                // Adicionar título se existir
                if (!string.IsNullOrWhiteSpace(paragraph.Title) && currentLength < maxLength)
                {
                    var titleLength = Math.Min(paragraph.Title.Length, maxLength - currentLength);
                    previewTexts.Add(paragraph.Title.Substring(0, titleLength));
                    currentLength += titleLength;

                    if (currentLength < maxLength)
                    {
                        previewTexts.Add(" ");
                        currentLength++;
                    }
                }

                // Processar inlines
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
                        var imageText = $"[Imagem]";
                        if (currentLength + imageText.Length <= maxLength)
                        {
                            previewTexts.Add(imageText);
                            currentLength += imageText.Length;
                        }
                    }
                }
            }
            else if (block is ImageBlock imageBlock)
            {
                var imageText = $"[Imagem]";
                if (currentLength + imageText.Length <= maxLength)
                {
                    previewTexts.Add(imageText);
                    currentLength += imageText.Length;
                }
            }
        }

        var preview = string.Join(" ", previewTexts).Trim();

        // Remover múltiplos espaços
        while (preview.Contains("  "))
        {
            preview = preview.Replace("  ", " ");
        }

        return string.IsNullOrWhiteSpace(preview) ? "Questão sem texto disponível." : preview;
    }
}
