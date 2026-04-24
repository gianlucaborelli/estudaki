using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Extensions;

public static class PublicNoticeExtensions
{
    /// <summary>
    /// Obtém a pasta completa para os arquivos do edital
    /// </summary>
    public static string GetExamFolder(this PublicNotice notice, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/files/exams/{notice.Year}/{notice.ExamBoard}/{notice.Id}";
    }

    /// <summary>
    /// Obtém o caminho completo do caderno de questões
    /// </summary>
    public static string GetQuestionFolder(this PublicNotice notice, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/files/exams/{notice.Year}/{notice.ExamBoard}/{notice.Id}/{notice.Id}.pdf";
    }

    /// <summary>
    /// Obtém o caminho completo do gabarito
    /// </summary>
    public static string GetAnswerKeyFolder(this PublicNotice notice, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/files/exams/{notice.Year}/{notice.ExamBoard}/{notice.Id}/{notice.Id}-answer-key.pdf";
    }

    /// <summary>
    /// Obtém o nome do arquivo do caderno de questões (para upload)
    /// </summary>
    public static string GetExamFileName(this PublicNotice notice)
    {
        return $"files/exams/{notice.Year}/{notice.ExamBoard}/{notice.Id}/{notice.Id}.pdf";
    }

    /// <summary>
    /// Obtém o nome do arquivo do gabarito (para upload)
    /// </summary>
    public static string GetAnswerKeyFileName(this PublicNotice notice)
    {
        return $"files/exams/{notice.Year}/{notice.ExamBoard}/{notice.Id}/{notice.Id}-answer-key.pdf";
    }

    /// <summary>
    /// Obtém o caminho da pasta de imagens no S3 para este edital
    /// </summary>
    public static string GetImagesFolder(this PublicNotice notice)
    {
        return $"files/exams/{notice.Year}/{notice.ExamBoard}/{notice.Id}/images";
    }

    /// <summary>
    /// Obtém a URL completa de uma imagem
    /// </summary>
    /// <param name="notice">Edital</param>
    /// <param name="imageKey">Nome do arquivo da imagem COM extensão (ex: "abc-123.png")</param>
    /// <param name="storageService">Serviço de storage</param>
    /// <returns>URL completa da imagem</returns>
    public static string GetImageUrl(this PublicNotice notice, string imageKey, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/{notice.GetImagesFolder()}/{imageKey}";
    }
}
