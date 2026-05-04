using Estudaki.Commons.Core.CQRS.Dispatchers;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using EstudaKi.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public class InlineImageEditorBase : ComponentBase
    {
        [Inject] private IDialogService Dialog { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        [Parameter] public ImageInline Value { get; set; } = new ImageInline();
        [CascadingParameter(Name = "PublicNotice")]
        protected PublicNoticeDto? PublicNotice { get; set; }

        protected async Task OpenImageSelectorForInline()
        {
            if (PublicNotice == null) return;

            var parameters = new DialogParameters<ImageSelectorModal>
        {
            { c => c.PublicNotice, PublicNotice }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
            var dialog = await Dialog.ShowAsync<ImageSelectorModal>("Selecionar Imagem", parameters, options);
            var result = await dialog.Result;

            if (result is not null && !result.Canceled && result.Data is string selectedImageKey)
            {
                if (Value is ImageInline imageInline)
                {
                    imageInline.Key = selectedImageKey;
                    StateHasChanged();
                }
            }
        }
    }
}
