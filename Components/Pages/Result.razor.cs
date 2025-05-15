using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;
using ProvaOnline.Services;

namespace ProvaOnline.Components.Pages;

public class ResultBase : ComponentBase
{
    [Inject]
    protected SearchService SearchService { get; set; }

    [Inject]
    protected IQuestionServices QuestionService { get; set; }

    [Inject]
    protected ISnackbar Snackbar { get; set; }

    [Inject]
    protected NavigationManager Navigation { get; set; }    

    protected int _totalPages { get; set; } = 0;
    protected int _currentPage { get; set; } = 0;
    protected QuestionDocument[] _questions { get; set; } = [];

    [Parameter]
    public string SearchId { get; set; } = string.Empty;

    protected override async Task OnParametersSetAsync()
    {      
        var searchResult = await QuestionService.SearchQuestionsPaginatedAsync(SearchService);

        _questions = [.. searchResult.Items];
        _totalPages = searchResult.TotalPages;
        _currentPage = searchResult.PageNumber;
    }
}
