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
            ExaminerOrganization = notice.ExaminerOrganization,
            ContractingOrganization = notice.ContractingOrganization,
            ExamCategory = notice.ExamCategory,
            IsReviewed = notice.IsReviewed,
            IsPublished = notice.IsPublished,
            FileUrl = notice.GetQuestionFolder(storageService),
            Exams = notice.Exams,
            CreatedAt = notice.CreatedAt
        };
    }

    public static PublicNotice ToEntity(this PublicNoticeDto noticeDto)
    {
        return new PublicNotice
        {
            Id = noticeDto.Id,
            Number = noticeDto.Number,
            Year = noticeDto.Year,
            ExaminerOrganization = noticeDto.ExaminerOrganization,
            ContractingOrganization = noticeDto.ContractingOrganization,
            ExamCategory = noticeDto.ExamCategory!,
            IsReviewed = noticeDto.IsReviewed,
            IsPublished = noticeDto.IsPublished,
            Exams = noticeDto.Exams,
            FileUrl= noticeDto.FileUrl!,
            CreatedAt = noticeDto.CreatedAt
        };
    }

    public static List<PublicNoticeDto> ToDtoList(this List<PublicNotice> notices, IStorageService storageService)
    {
        return notices.Select(notice => notice.ToDto(storageService)).ToList();
    }
}
