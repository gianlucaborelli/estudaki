using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetAreasPaginated;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class QuestionEditorModalBase : ComponentBase
{
    private const int AreaSearchPageSize = 5;

    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected ILogger<QuestionEditorModalBase> Logger { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
    [Inject]
    protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public List<QuestionSupportDto> AvailableQuestionSupports { get; set; } = [];

    [Parameter]
    public QuestionDto? Question { get; set; }

    [Parameter]
    public PublicNoticeDto? PublicNotice { get; set; }

    public QuestionDto EditedQuestion { get; set; } = new QuestionDto();

    protected AreaDto? SelectedMainArea { get; set; }
    protected IReadOnlyCollection<AreaDto> SelectedSubAreas { get; set; } = [];
    protected AreaDto? SelectedSubAreaValue { get; set; }
    protected MudAutocomplete<AreaDto> SubAreaAutocomplete { get; set; } = null!;

    // Enums
    protected enum InlineType { Text, Image, Math, Chemical }

    protected override void OnParametersSet()
    {
        if (Question != null)
        {
            EditedQuestion = QuestionDto.Clone(Question);
            Logger.LogDebug("Editor de questão inicializado com a questão: {QuestionId}", Question.QuestionId);

            SelectedMainArea = !string.IsNullOrWhiteSpace(EditedQuestion.MainArea)
                ? new AreaDto { Name = EditedQuestion.MainArea, Type = AreaType.Area }
                : null;

            SelectedSubAreas = EditedQuestion.SubAreas
                .Select(name => new AreaDto { Name = name, Type = AreaType.SubArea })
                .ToList();
        }
    }

    // ==================== MÉTODOS DE GERENCIAMENTO DE ÁREA/SUBÁREA ====================

    protected async Task<IEnumerable<AreaDto>> SearchMainAreas(string searchText, CancellationToken cancellationToken)
    {
        var query = new GetAreasPaginatedQuery(AreaType.Area, searchText, 1, AreaSearchPageSize);
        var result = await QueryDispatcher.DispatchAsync<GetAreasPaginatedQuery, PagedResult<AreaDto>>(query, cancellationToken);
        return result.Items;
    }

    protected void OnMainAreaChanged(AreaDto? area)
    {
        SelectedMainArea = area;
        EditedQuestion.MainArea = area?.Name ?? string.Empty;
    }

    protected async Task<IEnumerable<AreaDto>> SearchSubAreas(string searchText, CancellationToken cancellationToken)
    {
        var query = new GetAreasPaginatedQuery(AreaType.SubArea, searchText, 1, AreaSearchPageSize);
        var result = await QueryDispatcher.DispatchAsync<GetAreasPaginatedQuery, PagedResult<AreaDto>>(query, cancellationToken);
        return result.Items.Where(a => SelectedSubAreas.All(selected => selected.Name != a.Name));
    }

    protected async Task OnSubAreaSelected(AreaDto? area)
    {
        if (area is null)
        {
            return;
        }

        if (SelectedSubAreas.All(a => a.Name != area.Name))
        {
            OnSubAreasChanged(SelectedSubAreas.Append(area));
        }

        SelectedSubAreaValue = null;
        await SubAreaAutocomplete.ClearAsync();
    }

    protected void OnSubAreasChanged(IEnumerable<AreaDto> areas)
    {
        SelectedSubAreas = areas?.ToList() ?? [];
        EditedQuestion.SubAreas = SelectedSubAreas.Select(a => a.Name).ToArray();
    }

    protected void RemoveSubArea(AreaDto area)
    {
        OnSubAreasChanged(SelectedSubAreas.Where(a => a.Name != area.Name));
    }

    protected async Task OpenNewAreaModal(AreaType type)
    {
        var parameters = new DialogParameters<NewAreaModal>
        {
            { x => x.Type, type }
        };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var title = type == AreaType.Area ? "Nova Área Principal" : "Nova Subárea";

        var dialog = await DialogService.ShowAsync<NewAreaModal>(title, parameters, options);
        var result = await dialog.Result;

        if (result is null || result.Canceled || result.Data is not AreaDto newArea)
        {
            return;
        }

        if (type == AreaType.Area)
        {
            OnMainAreaChanged(newArea);
        }
        else
        {
            OnSubAreasChanged(SelectedSubAreas.Append(newArea));
        }

        StateHasChanged();
    }

    protected async Task Save()
    {
        if (!string.IsNullOrEmpty(EditedQuestion.QuestionId))
        {
            var command = new UpdateQuestionCommand(EditedQuestion);
            var result = await CommandDispatcher.DispatchAsync<UpdateQuestionCommand, ValidationResult>(command);

            if (result.IsValid)
            {
                Logger.LogInformation("Questão {QuestionId} atualizada com sucesso", EditedQuestion.QuestionId);
                Snackbar.Add("Questão atualizada com sucesso!", Severity.Success);
                Dialog.Close(DialogResult.Ok(EditedQuestion));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Logger.LogWarning("Erro de validação ao salvar questão {QuestionId}: {Error}", EditedQuestion.QuestionId, error.ErrorMessage);
                    Snackbar.Add(error.ErrorMessage, Severity.Error);
                }
            }
        }
        else
        {
            var command = new AddNewQuestionIntoExamCommand(EditedQuestion);
            var result = await CommandDispatcher.DispatchAsync<AddNewQuestionIntoExamCommand, ValidationResult>(command);
            if (result.IsValid)
            {
                Logger.LogInformation("Nova questão criada com sucesso: {QuestionId}", EditedQuestion.QuestionId);
                Snackbar.Add("Nova questão criada com sucesso!", Severity.Success);
                Dialog.Close(DialogResult.Ok(EditedQuestion));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Logger.LogWarning("Erro de validação ao criar nova questão: {Error}", error.ErrorMessage);
                    Snackbar.Add(error.ErrorMessage, Severity.Error);
                }
            }
        }
        
    }   
    
    protected void ToggleQuestionSupportLink(QuestionSupportDto support, bool isChecked)
    {
        if (isChecked)
        {
            EditedQuestion.QuestionSupports ??= new List<QuestionSupportDto>();
            if (!EditedQuestion.QuestionSupports.Contains(support))
            {
                EditedQuestion.QuestionSupports.Add(support);
            }
        }
        else
        {
            var supportInList = EditedQuestion.QuestionSupports.FirstOrDefault(x => x.Id == support.Id);
            if (supportInList != null)
            {
                EditedQuestion.QuestionSupports.Remove(supportInList);
            }
        }

        StateHasChanged();
    }

    // ==================== MÉTODOS DE GERENCIAMENTO DE ALTERNATIVAS ====================

    protected void AddChoice()
    {
        EditedQuestion.Choices ??= new List<Choice>();

        var nextOption = EditedQuestion.Choices.Count > 0
            ? ((char)(EditedQuestion.Choices.Last().Option?[0] ?? 'A' + 1)).ToString()
            : "A";

        EditedQuestion.Choices.Add(new Choice
        {
            Option = nextOption,
            IsCorrect = false,
            Content = new List<InlineContent> { new TextInline() }
        });

        StateHasChanged();
    }

    protected void AddChoiceText(Choice choice)
    {
        var textInline = new TextInline();
        choice.Content.Add(textInline);
    }

    protected void RemoveChoice(Choice choice)
    {
        if (EditedQuestion?.Choices == null || choice == null) return;        
        EditedQuestion.Choices.Remove(choice);
        StateHasChanged();
    }       
}
