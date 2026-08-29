using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("exam_extractions")]
public class ExamExtraction : Entity
{
    [BsonElement("exam_file")]
    public string ExamFile { get; set; } = string.Empty;

    [BsonElement("total_exam_questions")]
    public int TotalExamQuestions { get; set; }

    [BsonElement("questions")]
    public List<QuestionExtraction> Questions { get; set; } = new List<QuestionExtraction>();
}

public class QuestionExtraction
{
    [BsonElement("question_number")]
    public int QuestionNumber { get; set; }

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("choices")]
    public List<ChoiceExtraction> SingleChoices { get; set; } = new List<ChoiceExtraction>();
}

public class ChoiceExtraction
{
    [BsonElement("option")]
    public string Option { get; set; } = string.Empty;
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;
}
