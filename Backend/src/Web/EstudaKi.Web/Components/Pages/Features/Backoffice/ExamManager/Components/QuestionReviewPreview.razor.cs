using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

/// <summary>
/// Exibidor simplificado e somente leitura de uma questão, usado nas telas de
/// revisão/comparação (aprovação de revisão por IA), sem os controles de
/// resposta, cópia ou download presentes no exibidor padrão de questões.
/// </summary>
public class QuestionReviewPreviewBase : ComponentBase
{
    [Parameter]
    public QuestionDto? Value { get; set; }

    protected static string RenderBlockText(ContentBlock block)
        => block switch
        {
            ParagraphBlock p => RenderInlineText(p.Inlines),
            ImageBlock i => i.Description ?? i.Title ?? string.Empty,
            _ => string.Empty
        };

    protected static string RenderInlineText(IEnumerable<InlineContent>? inlines)
    {
        if (inlines is null) return string.Empty;

        return string.Join(" ", inlines.Select(inline => inline switch
        {
            TextInline t => t.Text,
            ImageInline img => img.Alt ?? string.Empty,
            _ => string.Empty
        }));
    }
}
