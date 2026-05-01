using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class InlineTextEditorBase : ComponentBase
{
    [Inject]
    protected ILogger<InlineTextEditorBase> Logger { get; set; } = default!;
    [Parameter]
    public TextInline Value { get; set; } = new TextInline();
   
}
