using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Comunications.Application.Commands.CreateContactMessage;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Pages.Features.ContactMessage.Component
{
    public partial class ContactMessageModalBase: ComponentBase
    {
        [CascadingParameter]
        protected IMudDialogInstance MudDialog { get; set; } = default!;

        [Inject]
        protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

        [Inject]
        protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        
        protected MudForm Form { get; set; } = default!;        
        protected bool Success { get; set; }        
        protected string[] Errors { get; set; } = [];
        protected string Name { get; set; } = string.Empty;
        protected string Email { get; set; } = string.Empty;
        protected string Message { get; set; } = string.Empty;
        protected bool CanBeReplied { get; set; } = true;


        protected async Task Submit() {
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
                Errors = [];
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Success = false;
                Errors = result.Errors.Select(e => e.ErrorMessage).ToArray();
            }
        }

        protected void Cancel() => MudDialog.Cancel();
    }
}
