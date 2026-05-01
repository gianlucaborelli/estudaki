using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public class InlineImageEditorBase : ComponentBase
    {
        [Parameter]
        public ImageInline Value { get; set; } = new ImageInline();

        protected void OpenImageSelectorForInline()
        {
            //Logger.LogDebug("Abrindo seletor de imagens para ImageInline no bloco {BlockIndex}, inline {InlineIndex}", blockIndex, inlineIndex);
        }
    }
}
