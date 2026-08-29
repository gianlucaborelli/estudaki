using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class EditExamModalBase : ComponentBase
{
    [CascadingParameter]
    protected IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

    [Parameter]
    public Exam? OriginalExam { get; set; }
    protected Exam EditedExam { get; set; } = new Exam();
    protected MudForm Form { get; set; } = default!;
    protected bool IsUploading { get; set; }

    protected override void OnParametersSet()
    {
        if (OriginalExam != null)
        {
            EditedExam = OriginalExam.Clone();
        }
    }

    protected async Task SaveAsync()
    {
        await Form.ValidateAsync();
        if (Form.IsValid)
        {
            try
            {
                IsUploading = true;

                var editCommand = new UpdateExamCommand(EditedExam);

                var result = await CommandDispatcher.DispatchAsync<UpdateExamCommand, ValidationResult>(editCommand);

                if (!result.IsValid)
                {
                    Snackbar.Add($"Erro ao salvar");
                    return;
                }

                Snackbar.Add("Salvo com sucesso");
                Dialog.Close(DialogResult.Ok(EditedExam));
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error saving exam: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsUploading = false;
            }
        }
    }
}
