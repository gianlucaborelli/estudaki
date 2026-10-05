using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("question_issues")]
public class QuestionIssue : Entity
{
    public string QuestionId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public bool CanBeReplied { get; set; } = false;
    public string Type { get; set; } = QuestionIssueType.Other;
    public string? Description { get; set; }
    public string Status { get; set; } = QuestionIssueStatus.Pending;
    public DateTime CreatedAt { get; set; }

    public static QuestionIssue Create(
        string questionId, 
        string? userName, 
        string? userEmail, 
        bool canBeReplied, 
        string type, 
        string? description)
    {
        return new QuestionIssue
        {
            QuestionId = questionId,
            UserName = userName,
            UserEmail = userEmail,
            CanBeReplied = canBeReplied,
            Type = type,
            Description = description,
            Status = QuestionIssueStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}
