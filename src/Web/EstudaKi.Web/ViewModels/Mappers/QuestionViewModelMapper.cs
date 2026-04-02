using Estudaki.Modules.Questions.Application.DTOs;

namespace EstudaKi.Web.ViewModels.Mappers;

public static class QuestionViewModelMapper
{
    public static QuestionViewModel ToViewModel(this QuestionDto dto)
    {
        return new QuestionViewModel
        {
            Id = dto.Id,
            QuestionNumber = dto.QuestionNumber,
            QuestionType = dto.QuestionType,
            MainArea = dto.MainArea,
            SubAreas = dto.SubAreas,
            CreatedAt = dto.CreatedAt,
            QuestionContents = dto.QuestionContents,
            Choices = dto.Choices,
            PublicNotice = dto.PublicNotice?.ToViewModel()
        };
    }

    public static IEnumerable<QuestionViewModel> ToViewModels(this IEnumerable<QuestionDto> dtos)
    {
        return dtos.Select(dto => dto.ToViewModel());
    }

    public static IEnumerable<QuestionViewModel> ToViewModels(this IReadOnlyList<QuestionDto> dtos)
    {
        return dtos.Select(dto => dto.ToViewModel());
    }

    public static IEnumerable<QuestionViewModel> ToViewModels(this List<QuestionDto> dtos)
    {
        return dtos.Select(dto => dto.ToViewModel());
    }
}
