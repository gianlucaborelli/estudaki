using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("questions")]
public class Question : Entity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    
    public bool IsPublished { get; set; } = false;  
    public string Type { get; set; } = QuestionType.MultipleChoice;
    public string MainArea { get; set; } = string.Empty;
    public string[] SubAreas { get; set; } = [];
    public List<string> QuestionSupports { get; set; } = [];
    public List<ContentBlock> QuestionContents { get; set; } = [];
    public List<Choice>? Choices { get; set; }

    /// <summary>
    /// Indica se a questão foi anulada em todos os exames onde aparece.
    /// Se anulada, esta propriedade afeta todos os exames relacionados.
    /// </summary>
    public bool IsNullified { get; set; } = false;

    /// <summary>
    /// Informações sobre os exames aos quais esta questão pertence.
    /// Desnormalizado para melhor performance em queries.
    /// </summary>
    public List<QuestionExam> Exams { get; set; } = [];
}










