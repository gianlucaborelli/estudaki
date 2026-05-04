using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class InlineTextEditorBase : ComponentBase
{
    [Inject]
    protected ILogger<InlineTextEditorBase> Logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public TextInline Value { get; set; } = new TextInline();

    protected string editorId = $"text-editor-{Guid.NewGuid()}";
    protected bool showFormattingMenu = false;
    protected double menuX = 0;
    protected double menuY = 0;
    protected string selectedText = string.Empty;
    protected int selectionStart = 0;
    protected int selectionEnd = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("eval", @"
                window.getTextSelection = function(editorId) {
                    // Buscar o container com o ID
                    const container = document.getElementById(editorId);
                    if (!container) {
                        return { text: '', start: 0, end: 0 };
                    }

                    let textarea = container.querySelector('textarea');
                    if (!textarea) {
                        textarea = container.querySelector('input[type=""text""]');
                    }
                    if (!textarea) {
                        textarea = container.querySelector('input');
                    }
                    // Tentar buscar em profundidade (MudTextField pode ter wrappers)
                    if (!textarea) {
                        textarea = container.querySelector('.mud-input-slot textarea');
                    }
                    if (!textarea) {
                        textarea = container.querySelector('.mud-input-slot input');
                    }

                    if (!textarea) {
                        console.error('Textarea/Input não encontrado. Estrutura do container:', container);
                        return { text: '', start: 0, end: 0 };
                    }

                    const start = textarea.selectionStart || 0;
                    const end = textarea.selectionEnd || 0;
                    const text = textarea.value.substring(start, end);

                    console.log('Seleção capturada:', text, 'Start:', start, 'End:', end);

                    return {
                        text: text,
                        start: start,
                        end: end
                    };
                }
            ");
        }
    }

    protected async Task OnTextSelected(MouseEventArgs e)
    {
        try
        {
            var selection = await JSRuntime.InvokeAsync<TextSelection>("getTextSelection", editorId);

            if (!string.IsNullOrEmpty(selection.Text))
            {
                // Captura a posição do mouse
                menuX = e.ClientX;
                menuY = e.ClientY;

                selectedText = selection.Text;
                selectionStart = selection.Start;
                selectionEnd = selection.End;
                showFormattingMenu = true;
                StateHasChanged();

                Logger.LogDebug("Menu aberto na posição ({X}, {Y}) para seleção: '{Text}'", menuX, menuY, selectedText);
            }
            else
            {
                // Sem seleção - fechar menu
                showFormattingMenu = false;
                StateHasChanged();
                Logger.LogDebug("Nenhuma seleção detectada - menu fechado");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao obter seleção de texto");
        }
    }

    protected void ApplyFormatting(string tag)
    {
        if (string.IsNullOrEmpty(selectedText) || selectionStart == selectionEnd)
            return;

        var beforeSelection = Value.Text.Substring(0, selectionStart);
        var afterSelection = Value.Text.Substring(selectionEnd);

        var formattedText = tag switch
        {
            "bold" => $"<strong>{selectedText}</strong>",
            "italic" => $"<em>{selectedText}</em>",
            "underline" => $"<u>{selectedText}</u>",
            _ => selectedText
        };

        Value.Text = beforeSelection + formattedText + afterSelection;
        showFormattingMenu = false;
        StateHasChanged();

        Logger.LogInformation("Formatação '{Tag}' aplicada ao texto selecionado", tag);
    }

    protected void CloseMenu()
    {
        showFormattingMenu = false;
        StateHasChanged();
    }

    public class TextSelection
    {
        public string Text { get; set; } = string.Empty;
        public int Start { get; set; }
        public int End { get; set; }
    }
}
