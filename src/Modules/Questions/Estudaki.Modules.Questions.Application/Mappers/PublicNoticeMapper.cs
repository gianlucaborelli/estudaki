using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Extensions;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class PublicNoticeMapper
{
    public static PublicNoticeDto ToDto(this PublicNotice notice, IStorageService storageService)
    {
        return new PublicNoticeDto
        {
            Id = notice.Id,
            Number = notice.Number,
            Year = notice.Year,
            ExamPhase = notice.ExamPhase,
            ExamBoard = notice.ExamBoard,
            ExamRequester = notice.ExamRequester,
            ExamCategory = notice.ExamCategory,
            IsReviewed = notice.IsReviewed,
            IsPublished = notice.IsPublished,
            Position = notice.Position,
            ExamBookletUrl = notice.GetQuestionFolder(storageService),
            AnswerKeyUrl = notice.GetAnswerKeyFolder(storageService),
            CreatedAt = notice.CreatedAt,
            HasAttachments = notice.HasAttachments
        };
    }

    public static PublicNotice ToEntity(this PublicNoticeDto noticeDto)
    {
        return new PublicNotice
        {
            Id = noticeDto.Id,
            Number = noticeDto.Number,
            Year = noticeDto.Year,
            ExamPhase = noticeDto.ExamPhase,
            ExamBoard = noticeDto.ExamBoard,
            ExamRequester = noticeDto.ExamRequester,
            ExamCategory = noticeDto.ExamCategory!,
            HasAttachments = noticeDto.HasAttachments,
            IsReviewed = noticeDto.IsReviewed,
            IsPublished = noticeDto.IsPublished,
            Position = noticeDto.Position,
            CreatedAt = noticeDto.CreatedAt
        };
    }

    public static List<PublicNoticeDto> ToDtoList(this List<PublicNotice> notices, IStorageService storageService)
    {
        return notices.Select(notice => notice.ToDto(storageService)).ToList();
    }
}
