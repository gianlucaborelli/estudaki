using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.AI;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionReviewResultsModalBase : ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
    [Inject]
    protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public string PublicNoticeId { get; set; } = string.Empty;

    protected List<QuestionReviewResult> Results { get; set; } = [];
    protected List<QuestionDto> Questions { get; set; } = [];
    protected bool IsLoading { get; set; } = true;

    protected QuestionReviewResult? SelectedResult { get; set; }
    protected QuestionDto? SelectedQuestionDto { get; set; }
    protected QuestionReviewSelectionState? SelectedState { get; set; }

    private readonly Dictionary<string, QuestionReviewSelectionState> _selections = [];

    protected int SuccessCount => Results.Count(r => r.Success);
    protected int DefectCount => Results.Count(r => r.Success && r.Review?.HasDefects == true);

    protected override async Task OnInitializedAsync()
    {
        await LoadResultsAsync();
    }

    private async Task LoadResultsAsync()
    {
        IsLoading = true;
        try
        {
            var questionsQuery = new GetQuestionsByPublicNoticeIdQuery(PublicNoticeId);
            var questionsTask = QueryDispatcher.DispatchAsync<GetQuestionsByPublicNoticeIdQuery, List<QuestionDto>>(questionsQuery);

            var command = new ReviewQuestionsByPublicNoticeIdCommand(PublicNoticeId);
            var resultsTask = CommandDispatcher
                .DispatchAsync<ReviewQuestionsByPublicNoticeIdCommand, List<QuestionReviewResult>>(command);

            await Task.WhenAll(questionsTask, resultsTask);

            Questions = await questionsTask;
            Results = await resultsTask;

            if (Results.Count == 0)
            {
                Snackbar.Add("Nenhuma questão encontrada para revisão.", Severity.Warning);
            }
            else
            {
                SelectQuestion(Results[0]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reviewing questions: " + ex.Message);
            Snackbar.Add("Erro ao revisar questões com IA.", Severity.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void SelectQuestion(QuestionReviewResult result)
    {
        SelectedResult = result;
        SelectedQuestionDto = Questions.FirstOrDefault(q => q.QuestionId == result.QuestionId);

        if (!_selections.TryGetValue(result.QuestionId, out var state))
        {
            state = QuestionReviewSelectionState.Create(result.Review);
            _selections[result.QuestionId] = state;
        }

        SelectedState = state;
    }

    protected void Close() => Dialog.Close(DialogResult.Ok(true));

    protected async Task ApproveAsync()
    {
        if (SelectedResult?.Review?.Question is null || SelectedQuestionDto is null || SelectedState is null)
            return;

        var updatedQuestion = BuildUpdatedQuestionDto(SelectedQuestionDto, SelectedResult.Review.Question, SelectedState);

        var parameters = new DialogParameters<ApproveQuestionReviewModal>
        {
            { x => x.OriginalQuestion, SelectedQuestionDto },
            { x => x.UpdatedQuestion, updatedQuestion }
        };

        var dialog = await DialogService.ShowAsync<ApproveQuestionReviewModal>("Confirmar alterações", parameters);
        var result = await dialog.Result;

        if (result is null || result.Canceled)
            return;

        try
        {
            var command = new UpdateQuestionCommand(updatedQuestion);
            var validationResult = await CommandDispatcher.DispatchAsync<UpdateQuestionCommand, FluentValidation.Results.ValidationResult>(command);

            if (!validationResult.IsValid)
            {
                Snackbar.Add("Não foi possível aprovar as alterações.", Severity.Error);
                return;
            }

            var index = Questions.FindIndex(q => q.QuestionId == updatedQuestion.QuestionId);
            if (index >= 0)
            {
                Questions[index] = updatedQuestion;
            }
            SelectedQuestionDto = updatedQuestion;

            Snackbar.Add("Questão atualizada com sucesso.", Severity.Success);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error approving question review: " + ex.Message);
            Snackbar.Add("Erro ao aprovar alterações da questão.", Severity.Error);
        }
    }

    private static QuestionDto BuildUpdatedQuestionDto(QuestionDto original, IAQuestion iaQuestion, QuestionReviewSelectionState state)
    {
        var updated = QuestionDto.Clone(original);

        if (state.MainAreaSelected)
        {
            updated.MainArea = iaQuestion.MainArea;
        }

        // Substitui apenas os itens marcados, mantendo os demais intactos (por posição).
        for (var i = 0; i < state.SubAreasSelected.Count && i < iaQuestion.SubAreas.Count; i++)
        {
            if (state.SubAreasSelected[i])
            {
                var subArea = iaQuestion.SubAreas[i];
                if (!updated.SubAreas.Contains(subArea))
                {
                    updated.SubAreas = updated.SubAreas.Append(subArea).ToArray();
                }
            }
        }

        for (var i = 0; i < state.QuestionContentsSelected.Count && i < iaQuestion.QuestionContents.Count && i < updated.QuestionContents.Count; i++)
        {
            if (state.QuestionContentsSelected[i])
            {
                var originalOrder = updated.QuestionContents[i].Order;
                updated.QuestionContents[i] = ToParagraphBlock(iaQuestion.QuestionContents[i], originalOrder);
            }
        }

        if (iaQuestion.Alternatives is { Count: > 0 } && updated.Choices is not null)
        {
            for (var i = 0; i < state.AlternativesSelected.Count && i < iaQuestion.Alternatives.Count && i < updated.Choices.Count; i++)
            {
                if (state.AlternativesSelected[i])
                {
                    updated.Choices[i] = ToChoice(iaQuestion.Alternatives[i]);
                }
            }
        }

        return updated;
    }

    private static ParagraphBlock ToParagraphBlock(SimpleContent content, int order)
        => new()
        {
            Inlines = [new TextInline { Text = content.Text }],
            Order = order,
        };

    private static Choice ToChoice(SimpleAlternative alternative)
        => new()
        {
            Option = alternative.Letter,
            IsCorrect = alternative.IsCorrect,
            Content = [new TextInline { Text = alternative.Text }],
        };

    protected static string GetSeverityIcon(string severity) => severity?.Trim().ToLowerInvariant() switch
    {
        "baixa" or "baixo" => Icons.Material.Filled.Info,
        "média" or "media" or "médio" or "medio" => Icons.Material.Filled.Warning,
        "alta" or "alto" => Icons.Material.Filled.Report,
        _ => Icons.Material.Filled.HelpOutline,
    };

    protected static Color GetSeverityColor(string severity) => severity?.Trim().ToLowerInvariant() switch
    {
        "baixa" or "baixo" => Color.Info,
        "média" or "media" or "médio" or "medio" => Color.Warning,
        "alta" or "alto" => Color.Error,
        _ => Color.Default,
    };
}

/// <summary>
/// Estado das seleções de checkbox do usuário para os itens da revisão de IA de uma questão,
/// usado para indicar quais propriedades da questão simplificada serão usadas na correção.
/// </summary>
public class QuestionReviewSelectionState
{
    public bool MainAreaSelected { get; set; }
    public List<bool> SubAreasSelected { get; set; } = [];
    public List<bool> QuestionContentsSelected { get; set; } = [];
    public List<bool> AlternativesSelected { get; set; } = [];

    public static QuestionReviewSelectionState Create(QuestionReview? review)
    {
        var state = new QuestionReviewSelectionState();
        if (review?.Question is null) return state;

        state.SubAreasSelected = review.Question.SubAreas.Select(_ => false).ToList();
        state.QuestionContentsSelected = review.Question.QuestionContents.Select(_ => false).ToList();
        state.AlternativesSelected = (review.Question.Alternatives ?? []).Select(_ => false).ToList();

        return state;
    }
}
