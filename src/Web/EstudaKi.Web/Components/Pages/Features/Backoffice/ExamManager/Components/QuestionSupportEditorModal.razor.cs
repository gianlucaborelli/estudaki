using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionSupportEditorModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    [Parameter]
    public PublicNoticeDto? Notice { get; set; }
    [Parameter]
    public QuestionSupportDto? QuestionSupport { get; set; }
    [Parameter]
    public bool IsNew {  get; set; }

    protected override void OnParametersSet()
    {
        if (QuestionSupport != null)
        {
            QuestionSupport = QuestionSupportDto.Clone(QuestionSupport);
        }
    }

    protected async Task Save()
    {
        if(QuestionSupport == null) return;

        var validationResult = new ValidationResult();
        if (IsNew)
        {
            var newQuestionSupport = new CreateQuestionSupportCommand(QuestionSupport, Notice!.Id!);
            validationResult = await CommandDispatcher
                .DispatchAsync<CreateQuestionSupportCommand, ValidationResult>(newQuestionSupport);
        }
        else
        {
            var updateQuestionSupport = new UpdateQuestionSupportCommand(QuestionSupport);
            validationResult = await CommandDispatcher
                .DispatchAsync<UpdateQuestionSupportCommand, ValidationResult>(updateQuestionSupport);
        }

        if (!validationResult.IsValid)
        {
            Snackbar.Add("Erro ao salvar o suporte de questão. Verifique os dados e tente novamente.", Severity.Error);
            return;
        }

        Snackbar.Add("Suporte de questão salvo com sucesso.", Severity.Success);
        Dialog.Close(DialogResult.Ok(QuestionSupport));
    }
}