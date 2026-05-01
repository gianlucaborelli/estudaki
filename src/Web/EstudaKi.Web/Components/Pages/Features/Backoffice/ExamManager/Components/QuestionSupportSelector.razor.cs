using Estudaki.Modules.Questions.Application.DTOs;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public class QuestionSupportSelectorBase : ComponentBase
    {
        [Parameter]
        public List<QuestionSupportDto> AvailableQuestionSupports { get; set; } = [];

        [Parameter]
        public QuestionDto Question { get; set; } = new QuestionDto();
    }
}
