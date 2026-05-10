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
    public string Position { get; set; } = string.Empty;
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

    public static QuestionDto Clone(QuestionDto original)
    {
        return new QuestionDto
        {
            QuestionId = original.QuestionId,
            PublicNoticeId = original.PublicNoticeId,
            ExamId = original.ExamId,
            CreatedAt = original.CreatedAt,
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
                            }).ToList()
        };
    }
}
