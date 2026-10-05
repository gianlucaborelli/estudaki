using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Extensions;

public static class PublicNoticeExtensions
{
    /// <summary>
    /// Obtém a pasta completa para os arquivos do edital
    /// </summary>
    public static string GetExamFolder(this PublicNotice notice)
    {
        return $"files/exams/{notice.Year}/{notice.ExaminerOrganization}/{notice.Id}";
    }

    public static string BuildExamFilePath(this PublicNotice notice, string examId)
    {
        var folder = notice.GetExamFolder();
        return $"{folder}/{examId}.pdf";
    }

    public static string BuildAnswerKeyPath(this PublicNotice notice, string examId)
    {
        var folder = notice.GetExamFolder();
        return $"{folder}/{examId}-answer-key.pdf";
    }

    /// <summary>
    /// Obtém o caminho da pasta de imagens no S3 para este edital
    /// </summary>
    public static string GetImagesFolder(this PublicNotice notice)
    {   
        return $"files/exams/{notice.Year}/{notice.ExaminerOrganization}/{notice.Id}/images";        
    }
}
