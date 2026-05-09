using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities2;

[CollectionName("questions2")]
public class Question2 : Entity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    
    public bool IsPublished { get; set; } = false;    
    public int Number { get; set; }
    public string Type { get; set; } = QuestionType.MultipleChoice;
    public string MainArea { get; set; } = string.Empty;
    public string[] SubAreas { get; set; } = [];
    public List<string> QuestionSupports { get; set; } = [];
    public List<ContentBlock> QuestionContents { get; set; } = [];
    public List<Choice>? Choices { get; set; }
}










