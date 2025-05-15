using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProvaOnline.Helpers;
using ProvaOnline.Models.DTO;
using ProvaOnline.Services;

namespace ProvaOnline.Components.Layout
{
    public partial class NavMenuBase : ComponentBase
    {
        // Filter Parameters
        protected string _wordKey { get; set; } = string.Empty;

        protected string[] _questionType { get; set; } = [];
        protected IEnumerable<string> _questionTypeSelected { get; set; } = [];

        protected string[] _mainArea { get; set; } = [];
        protected IEnumerable<string> _mainAreaSelected { get; set; } = [];

        protected string[] _subArea { get; set; } = [];
        protected IEnumerable<string> _subAreaSelected { get; set; } = [];

        // End Filter Parameters


        [Inject]
        protected IQuestionServices _questionService { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected NavigationManager _navigationManager { get; set; } = default!;

        [Inject]
        protected SearchService _searchParameters { get; set; } = default!;

        private async Task GetFilterParameters()
        {
            var filterParameters = new FilterParameters
            {
                TypeQuestions = _questionTypeSelected?.ToArray() ?? Array.Empty<string>(),
                MainAreas = _mainAreaSelected?.ToArray() ?? Array.Empty<string>(),
                SubAreas = _subAreaSelected?.ToArray() ?? Array.Empty<string>()
            };
            filterParameters = await _questionService.LoadFilterParameters(filterParameters);
            _questionType = filterParameters.TypeQuestions.ToArray();
            _mainArea = filterParameters.MainAreas.ToArray();
            _subArea = filterParameters.SubAreas.ToArray();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetFilterParameters();
        }

        protected async Task LoadingFilterParameters()
        {
            await GetFilterParameters();
        }

        protected void SearchQuestions()
        {
            var searchParameters = new SearchParameters
            {
                WordKey = _wordKey,
                TypeQuestions = _questionTypeSelected?.ToArray() ?? Array.Empty<string>(),
                MainAreas = _mainAreaSelected?.ToArray() ?? Array.Empty<string>(),
                SubAreas = _subAreaSelected?.ToArray() ?? Array.Empty<string>()
            };

            var searchId = Guid.NewGuid();
            _searchParameters.SearchParameters = searchParameters;
            _navigationManager.NavigateTo($"/result?searchId={searchId}");
        }
    }
}
