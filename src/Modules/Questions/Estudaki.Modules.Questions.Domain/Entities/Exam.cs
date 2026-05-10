using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Estudaki.Modules.Questions.Domain.Entities;

/// <summary>
/// Documento filho que representa um exame/cargo dentro de um edital.
/// Armazenado como parte do documento PublicNotice2.
/// </summary>
public class Exam
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Phase { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string ExamBookletUrl { get; set; } = string.Empty;
    public string AnswerKeyUrl { get; set; } = string.Empty;
    public List<AnswerKeyItem> AnswerKeyItems { get; set; } = new List<AnswerKeyItem>();
}
