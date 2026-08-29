using EstudaKi.Web.Components.Pages.Features.ContactMessage.Component;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Layout.Components
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
