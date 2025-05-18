using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using ProvaOnline.Extensions;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;
using ProvaOnline.Services;

namespace ProvaOnline.Components.Pages;

public partial class ResultBase : ComponentBase, IDisposable
{
    [Inject]
    protected IQuestionServices QuestionService { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    
    private int _currentPage = 1;
    [Parameter]
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                _ = OnParameterChangedAsync();
            }
        }
    }

    private int _pageSize = 10;
    [Parameter]
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (_pageSize != value)
            {
                _pageSize = value;
                _ = OnParameterChangedAsync();
            }
        }
    }

    private string[] _typeQuestions = [];
    [Parameter]
    public string[] TypeQuestions
    {
        get => _typeQuestions;
        set
        {
            if (!_typeQuestions.AreArraysEqual(value))
            {
                _typeQuestions = value;
                _ = OnParameterChangedAsync();
            }
        }
    }

    private string[] _mainAreas = [];
    [Parameter]
    public string[] MainAreas
    {
        get => _mainAreas;
        set
        {
            if (!_mainAreas.AreArraysEqual(value))
            {
                _mainAreas = value;
                _ = OnParameterChangedAsync();
            }
        }
    }

    private string[] _subAreas = [];
    [Parameter]
    public string[] SubAreas
    {
        get => _subAreas;
        set
        {
            if (!_subAreas.AreArraysEqual(value))
            {
                _subAreas = value;
                _ = OnParameterChangedAsync();
            }
        }
    }

    private string _wordKey = string.Empty;
    [Parameter]
    public string WordKey
    {
        get => _wordKey;
        set
        {
            if (_wordKey != value)
            {
                _wordKey = value;
                _ = OnParameterChangedAsync();
            }
        }
    }

    public int TotalPages { get; set; } = 0;
    protected int BoundaryCount { get; set; } = 1;

    protected QuestionDocument[] Questions { get; set; } = [];
    private SearchParameters _searchParameters = new();

    protected override async Task OnInitializedAsync()
    {
        Navigation.LocationChanged += OnLocationChanged;
        await ParseUrlParametersAndRefresh();
    }    

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        await ParseUrlParametersAndRefresh();
        StateHasChanged();
    }

    private async Task ParseUrlParametersAndRefresh()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        _currentPage = int.TryParse(query["CurrentPage"], out var cp) ? cp : 1;
        _pageSize = int.TryParse(query["PageSize"], out var ps) ? ps : 10;
        _wordKey = query["WordKey"] ?? string.Empty;
        _typeQuestions = query["TypeQuestions"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
        _mainAreas = query["MainAreas"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
        _subAreas = query["SubAreas"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];

        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        _searchParameters = new SearchParameters
        {
            CurrentPage = CurrentPage,
            PageSize = PageSize,
            WordKey = WordKey,
            TypeQuestions = TypeQuestions,
            MainAreas = MainAreas,
            SubAreas = SubAreas
        };

        var searchResult = await QuestionService.SearchQuestionsPaginatedAsync(_searchParameters);

        Questions = [.. searchResult.Items];
        TotalPages = searchResult.TotalPages;
        _currentPage = searchResult.PageNumber;
    }

    private async Task OnParameterChangedAsync()
    {
        await RefreshDataAsync();
        Navigation.NavigateTo($"/result?{_searchParameters}");
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= OnLocationChanged;
    }
}
