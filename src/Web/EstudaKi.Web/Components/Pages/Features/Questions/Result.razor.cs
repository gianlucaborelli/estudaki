using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Domain.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using EstudaKi.Web.Helpers;

namespace EstudaKi.Web.Components.Pages.Features.Questions;

public partial class ResultBase : ComponentBase
{
    [Inject]
    protected IQueryDispatcher _queryDispatcher { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "page")]
    public int CurrentPage { get; set; } = 1;

    [SupplyParameterFromQuery(Name = "size")]
    public int PageSize { get; set; } = 10;

    [SupplyParameterFromQuery(Name = "q")]
    public string WordKey { get; set; } = string.Empty;

    [SupplyParameterFromQuery(Name = "types")]
    public string? TypeQuestionsParam { get; set; }

    [SupplyParameterFromQuery(Name = "categories")]
    public string? ExamCategoriesParam { get; set; }

    [SupplyParameterFromQuery(Name = "areas")]
    public string? MainAreasParam { get; set; }

    [SupplyParameterFromQuery(Name = "subareas")]
    public string? SubAreasParam { get; set; }

    protected string[] TypeQuestions => TypeQuestionsParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
    protected string[] ExamCategories => ExamCategoriesParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
    protected string[] MainAreas => MainAreasParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
    protected string[] SubAreas => SubAreasParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

    public int TotalPages { get; set; } = 0;
    protected int BoundaryCount { get; set; } = 1;
    protected QuestionDto[] Questions { get; set; } = [];

    protected string SearchQuery { get; set; } = string.Empty;
    protected IReadOnlyCollection<string> SelectedTypeQuestion { get; set; } = [];
    protected IReadOnlyCollection<string> SelectedExamCategory { get; set; } = [];
    protected IEnumerable<string> SelectedMainArea { get; set; } = [];
    protected IEnumerable<string> SelectedSubArea { get; set; } = [];

    protected List<(string Value, string DisplayName)> AvailableTypeQuestionsDisplay { get; set; } = [];
    protected string[] AvailableTypeQuestions { get; set; } = [];
    protected List<(string Value, string DisplayName)> AvailableExamCategoriesDisplay { get; set; } = [];
    protected string[] AvailableExamCategories { get; set; } = [];
    protected string[] AvailableMainAreas { get; set; } = [];
    protected string[] AvailableSubAreas { get; set; } = [];

    private string _previousParametersHash = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        if (CurrentPage < 1) CurrentPage = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100; // Limite máximo para evitar sobrecarga

        var currentHash = $"{CurrentPage}|{PageSize}|{WordKey}|{TypeQuestionsParam}|{ExamCategoriesParam}|{MainAreasParam}|{SubAreasParam}";

        if (_previousParametersHash != currentHash)
        {
            _previousParametersHash = currentHash;

            SearchQuery = WordKey ?? string.Empty;           

            SelectedTypeQuestion = TypeQuestions;
            SelectedExamCategory = ExamCategories;
            SelectedMainArea = MainAreas;
            SelectedSubArea = SubAreas;
            StateHasChanged();

            await LoadAvailableFiltersFromParams(SelectedTypeQuestion.ToArray(), SelectedExamCategory.ToArray(), SelectedMainArea.ToArray(), SelectedSubArea.ToArray());
            
            StateHasChanged();

            await RefreshDataAsync();
        }
    }

    private async Task RefreshDataAsync()
    {
        try
        {
            var searchParameters = new SearchParameters
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                WordKey = WordKey,
                TypeQuestions = TypeQuestions,
                ExamCategories = ExamCategories,
                MainAreas = MainAreas,
                SubAreas = SubAreas
            };

            var searchResult = await _queryDispatcher
                .DispatchAsync<SearchQuestionsPaginatedQuery, PageResult<QuestionDto>>(new SearchQuestionsPaginatedQuery(searchParameters));

            Questions = [.. searchResult.Items];
            TotalPages = searchResult.TotalPages;
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar questões: {ex.Message}", Severity.Error);
            Questions = [];
            TotalPages = 0;
        }
    }

    protected void OnPageChanged(int newPage)
    {
        if (CurrentPage != newPage)
        {
            CurrentPage = newPage;
            UpdateUrl();
        }
    }

    private void UpdateUrl()
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = CurrentPage.ToString(),
            ["size"] = PageSize.ToString()
        };

        if (!string.IsNullOrWhiteSpace(WordKey))
            queryParams["q"] = WordKey;

        if (TypeQuestions.Length > 0)
            queryParams["types"] = string.Join(",", TypeQuestions);

        if (ExamCategories.Length > 0)
            queryParams["categories"] = string.Join(",", ExamCategories);

        if (MainAreas.Length > 0)
            queryParams["areas"] = string.Join(",", MainAreas);

        if (SubAreas.Length > 0)
            queryParams["subareas"] = string.Join(",", SubAreas);

        var url = QueryHelpers.AddQueryString("/result", queryParams);
        Navigation.NavigateTo(url);
    }

    protected async Task LoadingFilterParameters()
    {
        await LoadAvailableFilters();
    }

    protected async Task LoadAvailableFilters()
    {
        await LoadAvailableFiltersFromParams(
            SelectedTypeQuestion?.ToArray() ?? Array.Empty<string>(),
            SelectedExamCategory?.ToArray() ?? Array.Empty<string>(),
            SelectedMainArea?.ToArray() ?? Array.Empty<string>(),
            SelectedSubArea?.ToArray() ?? Array.Empty<string>()
        );
    }

    protected async Task LoadAvailableFiltersFromParams(string[] types, string[] categories, string[] areas, string[] subareas)
    {
        try
        {
            var filterParams = new FilterParameters
            {
                TypeQuestions = types,
                ExamCategories = categories,
                MainAreas = areas,
                SubAreas = subareas
            };

            var result = await _queryDispatcher
                .DispatchAsync<GetFilterParametersQuery, FilterParameters>(
                    new GetFilterParametersQuery(filterParams));

            AvailableTypeQuestions = result.TypeQuestions.ToArray();
            AvailableTypeQuestionsDisplay = QuestionTypeHelper.GetDisplayList(AvailableTypeQuestions);
            AvailableExamCategories = result.ExamCategories.ToArray();    
            AvailableExamCategoriesDisplay = ExamCategoryHelper.GetDisplayList(AvailableExamCategories);
            AvailableMainAreas = result.MainAreas;
            AvailableSubAreas = result.SubAreas;
            StateHasChanged();
        }
        catch
        {
            AvailableTypeQuestions = [];
            AvailableExamCategories = [];
            AvailableMainAreas = [];
            AvailableSubAreas = [];
        }
    }

    protected void OnSearchKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            ApplyFilters();
        }
    }

    protected void ApplyFilters()
    {
        CurrentPage = 1;

        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = "1",
            ["size"] = PageSize.ToString()
        };

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            queryParams["q"] = SearchQuery;
        }

        if (SelectedTypeQuestion != null && SelectedTypeQuestion.Any())
        {
            queryParams["types"] = string.Join(",", SelectedTypeQuestion);
        }

        if (SelectedExamCategory != null && SelectedExamCategory.Any())
        {
            queryParams["categories"] = string.Join(",", SelectedExamCategory);
        }

        if (SelectedMainArea != null && SelectedMainArea.Any())
        {
            queryParams["areas"] = string.Join(",", SelectedMainArea);
        }

        if (SelectedSubArea != null && SelectedSubArea.Any())
        {
            queryParams["subareas"] = string.Join(",", SelectedSubArea);
        }

        var url = QueryHelpers.AddQueryString("/result", queryParams);
        Navigation.NavigateTo(url);
    }

    protected async Task ClearFilters()
    {
        SearchQuery = string.Empty;
        SelectedTypeQuestion = [];
        SelectedExamCategory = [];
        SelectedMainArea = [];
        SelectedSubArea = [];
        await LoadAvailableFilters();

        // Navega para página de resultados sem filtros
        Navigation.NavigateTo("/result?page=1&size=10");
    }
}
