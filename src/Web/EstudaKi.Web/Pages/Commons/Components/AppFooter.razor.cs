using EstudaKi.Web.Pages.Features.ContactMessage.Component;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Pages.Commons.Components
{
    public class AppFooterBase : ComponentBase
    {
        [Inject]
        protected IDialogService Dialog { get; set; } = default!;

        protected Task OpenContactModal()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                BackdropClick = true
            };

            return Dialog.ShowAsync<ContactMessageModal>("Contato", options);
        }
    }
}
