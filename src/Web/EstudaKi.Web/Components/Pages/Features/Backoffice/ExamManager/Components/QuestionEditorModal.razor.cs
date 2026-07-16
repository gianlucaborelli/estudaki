using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionEditorModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected ILogger<QuestionEditorModalBase> Logger { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

    [Parameter]
    public List<QuestionSupportDto> AvailableQuestionSupports { get; set; } = [];

    [Parameter]
    public QuestionDto? Question { get; set; }

    [Parameter]
    public PublicNoticeDto? PublicNotice { get; set; }

    public QuestionDto EditedQuestion { get; set; } = new QuestionDto();

    // Enums
    protected enum InlineType { Text, Image, Math, Chemical }

    protected override void OnParametersSet()
    {
        if (Question != null)
        {
            EditedQuestion = QuestionDto.Clone(Question);
            Logger.LogDebug("Editor de questão inicializado com a questão: {QuestionId}", Question.QuestionId);
        }        
    }

    protected async Task Save()
    {
        if (!string.IsNullOrEmpty(EditedQuestion.QuestionId))
        {
            var command = new UpdateQuestionCommand(EditedQuestion);
            var result = await CommandDispatcher.DispatchAsync<UpdateQuestionCommand, ValidationResult>(command);

            if (result.IsValid)
            {
                Logger.LogInformation("Questão {QuestionId} atualizada com sucesso", EditedQuestion.QuestionId);
                Snackbar.Add("Questão atualizada com sucesso!", Severity.Success);
                Dialog.Close(DialogResult.Ok(EditedQuestion));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Logger.LogWarning("Erro de validação ao salvar questão {QuestionId}: {Error}", EditedQuestion.QuestionId, error.ErrorMessage);
                    Snackbar.Add(error.ErrorMessage, Severity.Error);
                }
            }
        }
        else
        {
            var command = new AddNewQuestionIntoExamCommand(EditedQuestion);
            var result = await CommandDispatcher.DispatchAsync<AddNewQuestionIntoExamCommand, ValidationResult>(command);
            if (result.IsValid)
            {
                Logger.LogInformation("Nova questão criada com sucesso: {QuestionId}", EditedQuestion.QuestionId);
                Snackbar.Add("Nova questão criada com sucesso!", Severity.Success);
                Dialog.Close(DialogResult.Ok(EditedQuestion));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Logger.LogWarning("Erro de validação ao criar nova questão: {Error}", error.ErrorMessage);
                    Snackbar.Add(error.ErrorMessage, Severity.Error);
                }
            }
        }
        
    }   
    
    protected void ToggleQuestionSupportLink(QuestionSupportDto support, bool isChecked)
    {
        if (isChecked)
        {
            EditedQuestion.QuestionSupports ??= new List<QuestionSupportDto>();
            if (!EditedQuestion.QuestionSupports.Contains(support))
            {
                EditedQuestion.QuestionSupports.Add(support);
            }
        }
        else
        {
            var supportInList = EditedQuestion.QuestionSupports.FirstOrDefault(x => x.Id == support.Id);
            if (supportInList != null)
            {
                EditedQuestion.QuestionSupports.Remove(supportInList);
            }
        }

        StateHasChanged();
    }

    // ==================== MÉTODOS DE GERENCIAMENTO DE ALTERNATIVAS ====================

    protected void AddChoice()
    {
        EditedQuestion.Choices ??= new List<Choice>();

        var nextOption = EditedQuestion.Choices.Count > 0
            ? ((char)(EditedQuestion.Choices.Last().Option?[0] ?? 'A' + 1)).ToString()
            : "A";

        EditedQuestion.Choices.Add(new Choice
        {
            Option = nextOption,
            IsCorrect = false,
            Content = new List<InlineContent> { new TextInline() }
        });

        StateHasChanged();
    }

    protected void AddChoiceText(Choice choice)
    {
        var textInline = new TextInline();
        choice.Content.Add(textInline);
    }

    protected void RemoveChoice(Choice choice)
    {
        if (EditedQuestion?.Choices == null || choice == null) return;        
        EditedQuestion.Choices.Remove(choice);
        StateHasChanged();
    }       
}
