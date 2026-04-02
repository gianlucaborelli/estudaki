using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;

namespace EstudaKi.Web.ViewModels.Mappers;

public static class PublicNoticeViewModelMapper
{
    public static PublicNoticeViewModel ToViewModel(this PublicNoticeDto dto)
    {
        return new PublicNoticeViewModel
        {
            Id = dto.Id,
            Number = dto.Number,
            Year = dto.Year,
            ExamPhase = dto.ExamPhase,
            ExamBoard = dto.ExamBoard,
            Position = dto.Position,
            ExamBookletUrl = dto.ExamBookletUrl,
            AnswerKeyUrl = dto.AnswerKeyUrl,
            CreatedAt = dto.CreatedAt
        };
    }

    public static PublicNoticeViewModel ToViewModel(this PublicNotice entity)
    {
        return new PublicNoticeViewModel
        {
            Id = entity.Id,
            Number = entity.Number,
            Year = entity.Year,
            ExamPhase = entity.ExamPhase,
            ExamBoard = entity.ExamBoard,
            Position = entity.Position,
            CreatedAt = entity.CreatedAt
        };
    }
}
