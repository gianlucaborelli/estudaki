using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.Commands;
using Estudaki.Modules.Ai.Domain.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.AIPromptManager.Components;

public class AIPromptEditorModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

    [Parameter]
    public AIPrompt? OriginalPrompt { get; set; }

    protected bool IsEditMode => OriginalPrompt is not null;
    protected bool IsSaving { get; set; }

    protected string Name { get; set; } = string.Empty;
    protected string? Description { get; set; }
    protected string Content { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        if (OriginalPrompt is not null)
        {
            Name = OriginalPrompt.Name;
            Description = OriginalPrompt.Description;
            Content = OriginalPrompt.Content;
        }
    }

    protected void Cancel() => Dialog.Cancel();

    protected async Task Save()
    {
        IsSaving = true;
        try
        {
            var result = IsEditMode
                ? await CommandDispatcher.DispatchAsync<UpdateAIPromptCommand, AIPromptCommandResult>(
                    new UpdateAIPromptCommand(OriginalPrompt!.Id, Content, Description))
                : await CommandDispatcher.DispatchAsync<CreateAIPromptCommand, AIPromptCommandResult>(
                    new CreateAIPromptCommand(Name, Content, Description));

            if (result.IsValid)
            {
                Snackbar.Add("Prompt salvo com sucesso!", Severity.Success);
                Dialog.Close(DialogResult.Ok(true));
            }
            else
            {
                foreach (var error in result.ValidationResult.Errors)
                    Snackbar.Add(error.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            IsSaving = false;
        }
    }
}
