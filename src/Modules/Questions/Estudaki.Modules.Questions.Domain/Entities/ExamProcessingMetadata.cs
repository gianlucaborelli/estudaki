using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Questions.Domain.Entities;

[CollectionName("exam_processing_metadata")]
public class ExamProcessingMetadata : Entity
{
    /// <summary>
    /// ID do PublicNotice relacionado (relacionamento N:1)
    /// </summary>
    public string PublicNoticeId { get; set; } = string.Empty;

    /// <summary>
    /// Nome do arquivo PDF original processado
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único da prova gerado durante o processamento
    /// (geralmente Path.GetFileNameWithoutExtension(fileName))
    /// </summary>
    public string ProvaId { get; set; } = string.Empty;

    /// <summary>
    /// Diretório onde as imagens extraídas foram salvas
    /// </summary>
    public string ImageStorageDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Lista de caminhos completos das imagens extraídas
    /// </summary>
    public List<string> ExtractedImagePaths { get; set; } = new();

    /// <summary>
    /// Texto completo extraído do PDF (para referência e reprocessamento)
    /// </summary>
    public string ExtractedText { get; set; } = string.Empty;

    /// <summary>
    /// Número total de questões identificadas no PDF
    /// </summary>
    public int TotalQuestionsFound { get; set; }

    /// <summary>
    /// Número total de imagens extraídas
    /// </summary>
    public int TotalImagesExtracted { get; set; }

    /// <summary>
    /// Data e hora do processamento
    /// </summary>
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica se o processamento foi bem-sucedido
    /// </summary>
    public bool ProcessingSuccessful { get; set; } = true;

    /// <summary>
    /// Mensagem de erro, se houver
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Versão do serviço de processamento utilizado
    /// </summary>
    public string ProcessingServiceVersion { get; set; } = "1.0";
}
