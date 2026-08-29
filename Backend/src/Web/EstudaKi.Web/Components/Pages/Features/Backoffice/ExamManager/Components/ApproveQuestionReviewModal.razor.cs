using Estudaki.Modules.Questions.Application.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class ApproveQuestionReviewModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public QuestionDto OriginalQuestion { get; set; } = default!;

    [Parameter]
    public QuestionDto UpdatedQuestion { get; set; } = default!;

    protected void Confirm() => Dialog.Close(DialogResult.Ok(true));

    protected void Cancel() => Dialog.Cancel();
}
