using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
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

        [Inject]
        protected IQuestionServices _questionService { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected NavigationManager _navigationManager { get; set; } = default!;

        [CascadingParameter]
        protected MainLayoutPage? MainLayout { get; set; }

        private async Task GetFilterParameters()
        {
            var filterParameters = new FilterParameters
            {
                TypeQuestions = _questionTypeSelected?.ToArray() ?? Array.Empty<string>(),
                MainAreas = _mainAreaSelected?.ToArray() ?? Array.Empty<string>(),
                SubAreas = _subAreaSelected?.ToArray() ?? Array.Empty<string>()
            };
            filterParameters = await _questionService.FindFilterParametersAsync(filterParameters);
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
            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = "1",
                ["size"] = "10"
            };

            if (!string.IsNullOrWhiteSpace(_wordKey))
                queryParams["q"] = _wordKey;

            var types = _questionTypeSelected?.ToArray() ?? [];
            if (types.Length > 0)
                queryParams["types"] = string.Join(",", types);

            var areas = _mainAreaSelected?.ToArray() ?? [];
            if (areas.Length > 0)
                queryParams["areas"] = string.Join(",", areas);

            var subAreas = _subAreaSelected?.ToArray() ?? [];
            if (subAreas.Length > 0)
                queryParams["subareas"] = string.Join(",", subAreas);

            var url = QueryHelpers.AddQueryString("/result", queryParams);

            MainLayout?.CloseDrawer();
            _navigationManager.NavigateTo(url);
        }
    }
}
