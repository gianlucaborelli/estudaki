using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionUnifyModalBase: ComponentBase
{
    [CascadingParameter] public IMudDialogInstance DialogInstance { get; set; } = default!;    

    [Parameter] public string PublicNoticeId { get; set; } = default!;

    [Inject]protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
    [Inject] protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
    protected HashSet<QuestionDto> SelectedQuestions { get; set; } = default!;

    protected List<QuestionDto> Questions { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var getQuestionByPublicNoticeId = new GetQuestionsByPublicNoticeIdQuery(PublicNoticeId);
        var questions = await QueryDispatcher.DispatchAsync<GetQuestionsByPublicNoticeIdQuery, List<QuestionDto>>(getQuestionByPublicNoticeId);
        Questions = questions;
    }

    protected async Task Save()
    {
        var questionUnify = new UnifyQuestionCommand(
            SelectedQuestions.Select(q => q.QuestionId).ToList()
        );
        
        var result = await CommandDispatcher.DispatchAsync<UnifyQuestionCommand, ValidationResult>(questionUnify);

        if(result.IsValid)
        {
            DialogInstance.Close(DialogResult.Ok(true));
        }
    }
}
