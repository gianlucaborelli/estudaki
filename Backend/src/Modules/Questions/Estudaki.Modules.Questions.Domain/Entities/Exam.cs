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
    /// <summary>
    /// Identificador único do exame.
    /// <example>507f1f77bcf86cd799439011</example>
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// Fase do exame (ex: objetiva, discursiva, prática).
    /// <example>Objetiva</example>
    /// </summary>
    public string Phase { get; set; } = string.Empty;
    
    /// <summary>
    /// Cargo ou posição para o qual o exame se destina.
    /// <example>Médico</example>
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Area de expecialização do cargo.
    /// <example>Pediatra, Psiquiatra</example>
    /// </summary>
    public string Area { get; set; } = string.Empty;
    
    /// <summary>
    /// Nível de escolaridade exigido para o cargo.
    /// <example>Superior</example>
    /// </summary>
    public string EducationLevel { get; set; } = string.Empty;
    
    /// <summary>
    /// URL do caderno de prova.
    /// <example>https://exemplo.com.br/caderno-prova.pdf</example>
    /// </summary>
    public string ExamBookletUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// URL do gabarito oficial.
    /// <example>https://exemplo.com.br/gabarito-oficial.pdf</example>
    /// </summary>
    public string AnswerKeyUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Lista de itens do gabarito com as respostas corretas.
    /// </summary>
    public List<AnswerKeyItem> AnswerKeyItems { get; set; } = new List<AnswerKeyItem>();
}
