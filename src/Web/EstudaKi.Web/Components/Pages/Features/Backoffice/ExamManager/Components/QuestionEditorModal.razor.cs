using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionEditorModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ILogger<QuestionEditorModalBase> Logger { get; set; } = default!;

    [Parameter]
    public QuestionDto? Question { get; set; }
    public QuestionDto EditedQuestion { get; set; } = new QuestionDto();

    [Parameter]
    public List<QuestionSupport>? AvailableQuestionSupports { get; set; }

    // Enums
    protected enum InlineType { Text, Image, Math, Chemical }
    // Estado do dialog de vincular suportes
    protected bool showLinkSupportDialog = false;

    // Estado do modal de seleção de imagens
    protected bool showImageSelectorModal = false;
    protected int currentEditingBlockIndex = -1;
    protected int currentEditingInlineIndex = -1;
    protected bool isEditingImageBlock = false;

    protected override void OnParametersSet()
    {
        if (Question != null)
        {
            EditedQuestion = QuestionDto.Clone(Question);
            Logger.LogDebug("Editor de questão inicializado com a questão: {QuestionId}", Question.Id);
        }        
    }

    protected void Save()
    {
        Dialog.Close(DialogResult.Ok(EditedQuestion));
        Logger.LogInformation("Questão salva: {QuestionId}", EditedQuestion.Id);
    }






    // ==================== MÉTODOS DE GERENCIAMENTO DE QUESTION SUPPORTS ====================

    protected void OpenLinkSupportDialog()
    {
        showLinkSupportDialog = true;
        Logger.LogDebug("Dialog de vincular textos de apoio aberto");
        StateHasChanged();
    }

    protected void CloseLinkSupportDialog()
    {
        showLinkSupportDialog = false;
        Logger.LogDebug("Dialog de vincular textos de apoio fechado");
        StateHasChanged();
    }

    protected void ToggleQuestionSupport(string supportId)
    {
        // if (Question == null || string.IsNullOrEmpty(supportId)) 
        // {
        //     Logger.LogWarning("Tentativa de vincular/desvincular suporte com ID inválido");
        //     return;
        // }

        // if (Question.QuestionSupports.Contains(supportId))
        // {
        //     Question.QuestionSupports.Remove(supportId);
        //     Logger.LogDebug("QuestionSupport {SupportId} desvinculado da questão. Total: {Count}", supportId, Question.QuestionSupports.Count);
        // }
        // else
        // {
        //     Question.QuestionSupports.Add(supportId);
        //     Logger.LogDebug("QuestionSupport {SupportId} vinculado à questão. Total: {Count}", supportId, Question.QuestionSupports.Count);
        // }

        StateHasChanged();
    }

    protected void UnlinkQuestionSupport(string supportId)
    {
        if (Question == null || string.IsNullOrEmpty(supportId)) return;

        //Question.QuestionSupports.Remove(supportId);
        Logger.LogDebug("QuestionSupport {SupportId} desvinculado da questão", supportId);
        StateHasChanged();
    }

    protected string GetSupportPreview(QuestionSupport support)
    {
        if (support == null || !support.Contents.Any()) return string.Empty;

        var previewTexts = new List<string>();
        const int maxLength = 200; // Máximo de caracteres no preview
        var currentLength = 0;

        foreach (var block in support.Contents.OrderBy(c => c.Order))
        {
            if (currentLength >= maxLength) break;

            if (block is ParagraphBlock paragraph)
            {
                foreach (var inline in paragraph.Inlines)
                {
                    if (currentLength >= maxLength) break;

                    if (inline is TextInline textInline && !string.IsNullOrWhiteSpace(textInline.Text))
                    {
                        var remainingLength = maxLength - currentLength;
                        var textToAdd = textInline.Text.Length <= remainingLength
                            ? textInline.Text
                            : textInline.Text.Substring(0, remainingLength) + "...";

                        previewTexts.Add(textToAdd);
                        currentLength += textToAdd.Length;
                    }
                    else if (inline is ImageInline imageInline)
                    {
                        previewTexts.Add($"[Imagem: {imageInline.Key}]");
                        currentLength += 20;
                    }
                }
            }
            else if (block is ImageBlock imageBlock)
            {
                previewTexts.Add($"[Imagem: {imageBlock.Key}]");
                currentLength += 20;
            }
        }

        return string.Join(" ", previewTexts).Trim();
    }

 

    

    // ==================== MÉTODOS DE GERENCIAMENTO DE ALTERNATIVAS ====================

    protected void AddChoice()
    {
        if (Question == null) return;

        Question.Choices ??= new List<Choice>();

        var nextOption = Question.Choices.Count > 0
            ? ((char)(Question.Choices.Last().Option?[0] ?? 'A' + 1)).ToString()
            : "A";

        Question.Choices.Add(new Choice
        {
            Option = nextOption,
            IsCorrect = false,
            Content = new List<InlineContent>()
        });

        Logger.LogDebug("Alternativa adicionada: {Option}", nextOption);
        StateHasChanged();
    }

    protected void RemoveChoice(int index)
    {
        if (Question?.Choices == null || index < 0 || index >= Question.Choices.Count) return;

        Question.Choices.RemoveAt(index);
        Logger.LogDebug("Alternativa removida no índice {Index}", index);
        StateHasChanged();
    }

    protected void MoveChoiceUp(int index)
    {
        if (Question?.Choices == null || index <= 0 || index >= Question.Choices.Count) return;

        var choice = Question.Choices[index];
        Question.Choices.RemoveAt(index);
        Question.Choices.Insert(index - 1, choice);
        StateHasChanged();
    }

    protected void MoveChoiceDown(int index)
    {
        if (Question?.Choices == null || index < 0 || index >= Question.Choices.Count - 1) return;

        var choice = Question.Choices[index];
        Question.Choices.RemoveAt(index);
        Question.Choices.Insert(index + 1, choice);
        StateHasChanged();
    }

    // ==================== MÉTODOS DE GERENCIAMENTO DE CONTEÚDO DAS ALTERNATIVAS ====================

    protected void AddChoiceInlineContent(int choiceIndex, InlineType type)
    {
        if (Question?.Choices == null || choiceIndex < 0 || choiceIndex >= Question.Choices.Count) return;

        var choice = Question.Choices[choiceIndex];

        if (type == InlineType.Text)
        {
            choice.Content.Add(new TextInline
            {
                Text = "",
                Bold = false,
                Italic = false
            });
        }
        else if (type == InlineType.Image)
        {
            choice.Content.Add(new ImageInline
            {
                Key = $"img-{Guid.NewGuid()}",
                Alt = "",
                Width = 0,
                Height = 0
            });
        }
        else if (type == InlineType.Math)
        {
            choice.Content.Add(new MathInline
            {
                Latex = ""
            });
        }
        else if (type == InlineType.Chemical)
        {
            choice.Content.Add(new ChemicalFormulaInline
            {
                Formula = ""
            });
        }

        Logger.LogDebug("Conteúdo inline adicionado à alternativa {ChoiceIndex}: {Type}", choiceIndex, type);
        StateHasChanged();
    }

    protected void RemoveChoiceInline(int choiceIndex, int inlineIndex)
    {
        if (Question?.Choices == null || choiceIndex < 0 || choiceIndex >= Question.Choices.Count) return;

        var choice = Question.Choices[choiceIndex];
        if (inlineIndex < 0 || inlineIndex >= choice.Content.Count) return;

        choice.Content.RemoveAt(inlineIndex);
        Logger.LogDebug("Conteúdo inline removido da alternativa {ChoiceIndex}, índice {InlineIndex}", choiceIndex, inlineIndex);
        StateHasChanged();
    }

    protected void MoveChoiceInlineUp(int choiceIndex, int inlineIndex)
    {
        if (Question?.Choices == null || choiceIndex < 0 || choiceIndex >= Question.Choices.Count) return;

        var choice = Question.Choices[choiceIndex];
        if (inlineIndex <= 0 || inlineIndex >= choice.Content.Count) return;

        var inline = choice.Content[inlineIndex];
        choice.Content.RemoveAt(inlineIndex);
        choice.Content.Insert(inlineIndex - 1, inline);
        StateHasChanged();
    }

    protected void MoveChoiceInlineDown(int choiceIndex, int inlineIndex)
    {
        if (Question?.Choices == null || choiceIndex < 0 || choiceIndex >= Question.Choices.Count) return;

        var choice = Question.Choices[choiceIndex];
        if (inlineIndex < 0 || inlineIndex >= choice.Content.Count - 1) return;

        var inline = choice.Content[inlineIndex];
        choice.Content.RemoveAt(inlineIndex);
        choice.Content.Insert(inlineIndex + 1, inline);
        StateHasChanged();
    }

    // ==================== MÉTODOS DE GERENCIAMENTO DO MODAL DE SELEÇÃO DE IMAGENS ====================    

    protected async Task OnImageSelectedFromModal(string imageKey)
    {
        if (Question == null || currentEditingBlockIndex < 0 || currentEditingBlockIndex >= Question.QuestionContents.Count)
        {
            Logger.LogWarning("Índice de bloco inválido ao selecionar imagem");
            return;
        }

        var block = Question.QuestionContents[currentEditingBlockIndex];

        if (isEditingImageBlock)
        {
            // Editando um ImageBlock
            if (block is ImageBlock imageBlock)
            {
                imageBlock.Key = imageKey;
                Logger.LogInformation("Imagem {Key} selecionada para ImageBlock no bloco {BlockIndex}", imageKey, currentEditingBlockIndex);
            }
        }
        else
        {
            // Editando um ImageInline dentro de um ParagraphBlock
            if (block is ParagraphBlock paragraph &&
                currentEditingInlineIndex >= 0 &&
                currentEditingInlineIndex < paragraph.Inlines.Count)
            {
                var inline = paragraph.Inlines[currentEditingInlineIndex];
                if (inline is ImageInline imageInline)
                {
                    imageInline.Key = imageKey;
                    Logger.LogInformation("Imagem {Key} selecionada para ImageInline no bloco {BlockIndex}, inline {InlineIndex}",
                        imageKey, currentEditingBlockIndex, currentEditingInlineIndex);
                }
            }
        }

        StateHasChanged();
        await Task.CompletedTask;
    }

    protected async Task CloseImageSelectorModal()
    {
        showImageSelectorModal = false;
        currentEditingBlockIndex = -1;
        currentEditingInlineIndex = -1;
        isEditingImageBlock = false;
        Logger.LogDebug("Modal de seleção de imagens fechado");
        await Task.CompletedTask;
    }
}
