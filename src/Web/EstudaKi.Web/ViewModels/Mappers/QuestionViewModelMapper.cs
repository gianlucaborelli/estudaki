using Estudaki.Modules.Questions.Application.DTOs;

namespace EstudaKi.Web.ViewModels.Mappers;

public static class QuestionViewModelMapper
{
    public static QuestionViewModel ToViewModel(this QuestionWithNoticeDto dto)
    {
        return new QuestionViewModel
        {
            Id = dto.Question.Id,
            QuestionNumber = dto.Question.QuestionNumber,
            QuestionType = dto.Question.QuestionType,
            MainArea = dto.Question.MainArea,
            SubAreas = dto.Question.SubAreas,
            CreatedAt = dto.Question.CreatedAt,
            QuestionContents = dto.Question.QuestionContents,
            Choices = dto.Question.Choices,
            PublicNotice = dto.PublicNotice?.ToViewModel()
        };
    }

    public static IEnumerable<QuestionViewModel> ToViewModels(this IEnumerable<QuestionWithNoticeDto> dtos)
    {
        return dtos.Select(dto => dto.ToViewModel());
    }

    public static IEnumerable<QuestionViewModel> ToViewModels(this IReadOnlyList<QuestionWithNoticeDto> dtos)
    {
        return dtos.Select(dto => dto.ToViewModel());
    }

    public static IEnumerable<QuestionViewModel> ToViewModels(this List<QuestionWithNoticeDto> dtos)
    {
        return dtos.Select(dto => dto.ToViewModel());
    }
}
