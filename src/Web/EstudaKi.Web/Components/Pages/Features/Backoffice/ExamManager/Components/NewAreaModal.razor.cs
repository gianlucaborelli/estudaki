using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class NewAreaModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

    [Parameter]
    public AreaType Type { get; set; }

    protected string Name { get; set; } = string.Empty;
    protected bool IsSaving { get; set; }

    protected string Title => Type == AreaType.Area ? "Nova Área Principal" : "Nova Subárea";

    protected void Cancel() => Dialog.Cancel();

    protected async Task OnNameKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Save();
        }
    }

    protected async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Snackbar.Add("Informe o nome da área.", Severity.Warning);
            return;
        }

        IsSaving = true;

        var command = new CreateAreaCommand(Name.Trim(), Type);
        var result = await CommandDispatcher.DispatchAsync<CreateAreaCommand, AreaCommandResult>(command);

        IsSaving = false;

        if (result.IsValid && result.Area != null)
        {
            Snackbar.Add("Área criada com sucesso!", Severity.Success);
            Dialog.Close(DialogResult.Ok(result.Area));
        }
        else
        {
            foreach (var error in result.ValidationResult.Errors)
            {
                Snackbar.Add(error.ErrorMessage, Severity.Error);
            }
        }
    }
}
