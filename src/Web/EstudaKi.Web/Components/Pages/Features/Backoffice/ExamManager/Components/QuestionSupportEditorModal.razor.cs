using Estudaki.Modules.Questions.Application.DTOs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionSupportEditorModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public PublicNoticeDto? Notice { get; set; }
    [Parameter]
    public QuestionSupportDto? QuestionSupport { get; set; }
    [Parameter]
    public bool IsNew {  get; set; }

    protected override void OnParametersSet()
    {
        if (QuestionSupport != null)
        {
            QuestionSupport = QuestionSupportDto.Clone(QuestionSupport);
        }
    }

    protected void Save()
    {
        // Salvar as alterações e fechar o modal, retornando o QuestionSupport atualizado
        if (IsNew)
        {
            // Cria novo
        }
        else
        {
            // Update existente
        }
        Dialog.Close(DialogResult.Ok(QuestionSupport));
    }
}