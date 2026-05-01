using Estudaki.Modules.Questions.Application.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public partial class EditPublicNoticeModalBase : ComponentBase
    {
        [CascadingParameter]
        protected IMudDialogInstance Dialog { get; set; } = default!;

        protected MudForm Form = default!;
        protected bool IsUploading { get; set; } = false;
        

        [Parameter]
        public PublicNoticeDto? OriginalNoticePublic { get; set; }
        protected PublicNoticeDto EditedPublicNotice { get; set; } = new PublicNoticeDto();

        protected override void OnParametersSet()
        {
            if (OriginalNoticePublic != null)
            {
                EditedPublicNotice = PublicNoticeDto.Clone(OriginalNoticePublic);
            }
        }

        protected async Task SavePublicNotice()
        {
            // TODO: Lógica para salvar o edital
        }
    }
}
