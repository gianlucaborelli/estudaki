using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.Commands;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Queries;
using EstudaKi.Web.Components.Pages.Features.Backoffice.AIPromptManager.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.AIPromptManager;

public class AIPromptManagerBase : ComponentBase
{
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected IDialogService Dialog { get; set; } = default!;
    [Inject]
    protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

    protected List<AIPromptDto> Prompts { get; set; } = [];
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadPromptsAsync();
    }

    private async Task LoadPromptsAsync()
    {
        IsLoading = true;
        try
        {
            Prompts = await QueryDispatcher.DispatchAsync<GetAllAIPromptsQuery, List<AIPromptDto>>(new GetAllAIPromptsQuery());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading AI prompts: {ex.Message}");
            Snackbar.Add("Erro ao carregar os prompts de IA.", Severity.Error);
            Prompts = [];
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task OpenCreateDialogAsync()
    {
        var parameters = new DialogParameters();

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await Dialog.ShowAsync<AIPromptEditorModal>("Novo Prompt", parameters, options);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
            await LoadPromptsAsync();
    }

    protected async Task OpenEditDialogAsync(AIPromptDto prompt)
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(AIPromptEditorModal.OriginalPrompt), prompt);

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await Dialog.ShowAsync<AIPromptEditorModal>("Editar Prompt", parameters, options);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
            await LoadPromptsAsync();
    }

    protected async Task DeleteAsync(AIPromptDto prompt)
    {
        bool? confirmed = await Dialog.ShowMessageBoxAsync(
            "Atenção",
            $"Deseja realmente excluir o prompt \"{prompt.Name}\"?",
            yesText: "Excluir", cancelText: "Cancelar");

        if (confirmed != true) return;

        var command = new DeleteAIPromptCommand(prompt.Id);
        var result = await CommandDispatcher.DispatchAsync<DeleteAIPromptCommand, AIPromptCommandResult>(command);

        if (result.IsValid)
        {
            Snackbar.Add("Prompt excluído com sucesso!", Severity.Success);
            await LoadPromptsAsync();
        }
        else
        {
            foreach (var error in result.ValidationResult.Errors)
                Snackbar.Add(error.ErrorMessage, Severity.Error);
        }
    }
}
