using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Extensions;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class PublicNoticeMapper
{
    public static PublicNoticeDto ToDto(this PublicNotice notice, IStorageService storageService, StorageSettings s3Settings)
    {
        return new PublicNoticeDto
        {
            Id = notice.Id,
            Number = notice.Number,
            Year = notice.Year,
            ExamPhase = notice.ExamPhase,
            ExamBoard = notice.ExamBoard,
            Position = notice.Position,
            ExamBookletUrl = notice.GetQuestionFolder(storageService),
            AnswerKeyUrl = notice.GetAnswerKeyFolder(storageService),
            CreatedAt = notice.CreatedAt
        };
    }
}
