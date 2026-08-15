namespace Estudaki.Commons.Core.DTOs;

public class ExamExtractionDto
{
    public string Id { get; set; } = string.Empty;
    public string ExamFile { get; set; } = string.Empty;
    public int TotalExamQuestions { get; set; }
    public List<QuestionExtractionDto> Questions { get; set; } = new List<QuestionExtractionDto>();
}

public class QuestionExtractionDto
{
    public int QuestionNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<ChoiceExtractionDto> SingleChoices { get; set; } = new List<ChoiceExtractionDto>();
}

public class ChoiceExtractionDto
{
    public string Option { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
