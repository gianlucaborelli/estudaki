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
    public string? Statement { get; set; }
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
                            Content = s.Content,
                            Id = s.Id,
                            PublicNoticeId = s.PublicNoticeId
                        }).ToList(),
            Statement = original.Statement,
            Choices = original.Choices?
                        .Select(c => 
                            new Choice
                            {
                                Option = c.Option,
                                Explanation = c.Explanation,
                                IsCorrect = c.IsCorrect
                            }).ToList(),
            CreatedAt = original.CreatedAt
        };
    }    
}
