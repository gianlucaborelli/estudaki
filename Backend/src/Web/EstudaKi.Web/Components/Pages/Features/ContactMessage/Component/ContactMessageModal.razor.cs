using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Comunications.Application.Commands.CreateContactMessage;
using EstudaKi.Web.Components.Layout.Components;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.ContactMessage.Component
{   
    public partial class ContactMessageModalBase: ValidatableComponentBase
    {
        [CascadingParameter]
        protected IMudDialogInstance MudDialog { get; set; } = default!;
        
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

        [Inject]
        protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        
        protected MudForm Form { get; set; } = default!;        
        protected bool Success { get; set; }
        protected string Name { get; set; } = string.Empty;
        protected string Email { get; set; } = string.Empty;
        protected string Message { get; set; } = string.Empty;
        protected bool CanBeReplied { get; set; } = true;


        protected async Task Submit() {
            ClearValidationErrors();

            var result = await CommandDispatcher.DispatchAsync<CreateContactMessageCommand, ValidationResult>(new CreateContactMessageCommand
            (
                Name,
                Email,
                Message,
                CanBeReplied,
                HttpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
            ));

            if(result.IsValid)
            {
                Success = true;
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add("Mensagem enviada com sucesso!", Severity.Success);
            }
            else
            {
                Success = false;
                ProcessValidationErrors(result);
                await Form.ValidateAsync();
                Snackbar.Add("Falha ao enviar a mensagem!", Severity.Error);
            }
        }

        protected void Cancel() => MudDialog.Cancel();
    }
}
