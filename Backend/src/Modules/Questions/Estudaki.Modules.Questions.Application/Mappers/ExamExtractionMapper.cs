using Estudaki.Commons.Core.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Application.Mappers;

internal static class ExamExtractionMapper
{
    public static ExamExtractionDto ToDto(this ExamExtraction examExtraction)
    {
        return new ExamExtractionDto
        {
            Id = examExtraction.Id,
            ExamFile = examExtraction.ExamFile,
            TotalExamQuestions = examExtraction.TotalExamQuestions,
            Questions = examExtraction.Questions.ConvertAll(q => q.ToDto())
        };
    }

    public static QuestionExtractionDto ToDto(this QuestionExtraction questionExtraction)
    {
        return new QuestionExtractionDto
        {
            QuestionNumber = questionExtraction.QuestionNumber,
            Content = questionExtraction.Content,
            SingleChoices = questionExtraction.SingleChoices.ConvertAll(c => c.ToDto())
        };
    }

    public static ChoiceExtractionDto ToDto(this ChoiceExtraction examExtractionDto)
    {
        return new ChoiceExtractionDto
        {
            Option = examExtractionDto.Option,
            Content = examExtractionDto.Content
        };
    }
}
