namespace Estudaki.Modules.Questions.Application.DTOs;

public class PublicNoticeDto
{
    public string Id { get; set; } = string.Empty;
    public string? Number { get; set; }
    public int Year { get; set; }
    public string? ExamPhase { get; set; }
    public string? ExamBoard { get; set; }
    public string? Position { get; set; }
    
    // URLs completas para consumo pelo frontend
    public string? ExamBookletUrl { get; set; }
    public string? AnswerKeyUrl { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
