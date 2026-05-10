using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class QuestionMapper
{
    public static QuestionDto ToDto(
        this Question question, 
        PublicNotice publicNotice, 
        Exam exam,
        ExamQuestion examQuestion,
        List<QuestionSupport>? questionSupports)
    {
        return new QuestionDto
        {
            QuestionId = question.Id,
            PublicNoticeId = publicNotice.Id,
            ExamId = exam.Id,
            PublicNoticeNumber = publicNotice.Number,
            Year = publicNotice.Year,
            ExaminerOrganization = publicNotice.ExaminerOrganization,
            ContractingOrganization = publicNotice.ContractingOrganization,
            ExamCategory = publicNotice.ExamCategory,
            Phase = exam.Phase,
            Position = exam.Position,
            Area = exam.Area,
            EducationLevel = exam.EducationLevel,
            PublicNoticeFileUrl = publicNotice.FileUrl,
            IsNullified = examQuestion.IsNullified,
            QuestionNumber = examQuestion.QuestionNumber,
            QuestionType = question.Type,
            MainArea = question.MainArea,
            SubAreas = question.SubAreas,
            QuestionContents = question.QuestionContents,
            QuestionSupports = questionSupports?
                    .Where(qs => question.QuestionSupports.Contains(qs.Id))
                    .Select(qs => qs.ToDto())
                    .ToList() ?? [],
            Choices = question.Choices,
            CreatedAt = question.CreatedAt
        };
    }    
}
