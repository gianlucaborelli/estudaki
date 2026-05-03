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
    public QuestionDto EditedQuestion { get; set; } = new QuestionDto();

    // Enums
    protected enum InlineType { Text, Image, Math, Chemical }

    protected override void OnParametersSet()
    {
        if (Question != null)
        {
            EditedQuestion = QuestionDto.Clone(Question);
            Logger.LogDebug("Editor de questão inicializado com a questão: {QuestionId}", Question.Id);
        }        
    }

    protected async Task Save()
    {
        var command = new UpdateQuestionCommand(EditedQuestion);
        var result = await CommandDispatcher.DispatchAsync<UpdateQuestionCommand, ValidationResult>(command);

        if(result.IsValid)
        {
            Logger.LogInformation("Questão {QuestionId} atualizada com sucesso", EditedQuestion.Id);
            Snackbar.Add("Questão atualizada com sucesso!", Severity.Success);
            Dialog.Close(DialogResult.Ok(EditedQuestion));
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Logger.LogWarning("Erro de validação ao salvar questão {QuestionId}: {Error}", EditedQuestion.Id, error.ErrorMessage);
                Snackbar.Add(error.ErrorMessage, Severity.Error);
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
            Content = new List<InlineContent>()
        });

        StateHasChanged();
    }

    protected void RemoveChoice(Choice choice)
    {
        if (EditedQuestion?.Choices == null || choice == null) return;        
        EditedQuestion.Choices.Remove(choice);
        StateHasChanged();
    }
   
    // ==================== MÉTODOS DE GERENCIAMENTO DO MODAL DE SELEÇÃO DE IMAGENS ====================    

    protected async Task OnImageSelectedFromModal(string imageKey)
    {
        //if (EditedQuestion == null || currentEditingBlockIndex < 0 || currentEditingBlockIndex >= EditedQuestion.QuestionContents.Count)
        //{
        //    Logger.LogWarning("Índice de bloco inválido ao selecionar imagem");
        //    return;
        //}

        //var block = EditedQuestion.QuestionContents[currentEditingBlockIndex];

        //if (isEditingImageBlock)
        //{
        //    // Editando um ImageBlock
        //    if (block is ImageBlock imageBlock)
        //    {
        //        imageBlock.Key = imageKey;
        //        Logger.LogInformation("Imagem {Key} selecionada para ImageBlock no bloco {BlockIndex}", imageKey, currentEditingBlockIndex);
        //    }
        //}
        //else
        //{
        //    // Editando um ImageInline dentro de um ParagraphBlock
        //    if (block is ParagraphBlock paragraph &&
        //        currentEditingInlineIndex >= 0 &&
        //        currentEditingInlineIndex < paragraph.Inlines.Count)
        //    {
        //        var inline = paragraph.Inlines[currentEditingInlineIndex];
        //        if (inline is ImageInline imageInline)
        //        {
        //            imageInline.Key = imageKey;
        //            Logger.LogInformation("Imagem {Key} selecionada para ImageInline no bloco {BlockIndex}, inline {InlineIndex}",
        //                imageKey, currentEditingBlockIndex, currentEditingInlineIndex);
        //        }
        //    }
        //}

        //StateHasChanged();
        await Task.CompletedTask;
    }    
}
