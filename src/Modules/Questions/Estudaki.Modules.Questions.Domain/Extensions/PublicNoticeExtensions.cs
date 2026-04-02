using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Extensions;

public static class PublicNoticeExtensions
{
    /// <summary>
    /// Obtém a URL completa do caderno de questões a partir do código armazenado
    /// </summary>
    public static string GetExamFolder(this PublicNotice notice, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/{notice.Year}/{notice.ExamBoard}/{notice.Id}";
    }

    public static string GetQuestionFolder(this PublicNotice notice, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/{notice.Year}/{notice.ExamBoard}/{notice.Id}/{notice.Id}.pdf";
    }

    public static string GetAnswerKeyFolder(this PublicNotice notice, IStorageService storageService)
    {
        return $"{storageService.GetFileUrl()}/{notice.Year}/{notice.ExamBoard}/{notice.Id}/{notice.Id}-answer-key.pdf";
    }
}
