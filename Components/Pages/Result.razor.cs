using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;
using ProvaOnline.Services;

namespace ProvaOnline.Components.Pages;

public partial class ResultBase : ComponentBase
{
    [Inject]
    protected IQuestionServices QuestionService { get; set; } = default!;

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

    [SupplyParameterFromQuery(Name = "areas")]
    public string? MainAreasParam { get; set; }

    [SupplyParameterFromQuery(Name = "subareas")]
    public string? SubAreasParam { get; set; }

    protected string[] TypeQuestions => TypeQuestionsParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
    protected string[] MainAreas => MainAreasParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
    protected string[] SubAreas => SubAreasParam?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

    public int TotalPages { get; set; } = 0;
    protected int BoundaryCount { get; set; } = 1;
    protected QuestionDocument[] Questions { get; set; } = [];

    private string _previousParametersHash = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        var currentHash = $"{CurrentPage}|{PageSize}|{WordKey}|{TypeQuestionsParam}|{MainAreasParam}|{SubAreasParam}";

        if (_previousParametersHash != currentHash)
        {
            _previousParametersHash = currentHash;
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
                MainAreas = MainAreas,
                SubAreas = SubAreas
            };

            var searchResult = await QuestionService.SearchQuestionsPaginatedAsync(searchParameters);

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

        if (MainAreas.Length > 0)
            queryParams["areas"] = string.Join(",", MainAreas);

        if (SubAreas.Length > 0)
            queryParams["subareas"] = string.Join(",", SubAreas);

        var url = QueryHelpers.AddQueryString("/result", queryParams);
        Navigation.NavigateTo(url);
    }
}
