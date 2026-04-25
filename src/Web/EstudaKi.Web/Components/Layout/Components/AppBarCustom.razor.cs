using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Domain.Common;
using EstudaKi.Web.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;

namespace EstudaKi.Web.Components.Layout.Components
{
    public partial class AppBarCustomBase : ComponentBase , IDisposable
    {
        [CascadingParameter(Name = "Theme")]
        protected MudTheme? Theme { get; set; }

        [CascadingParameter(Name = "IsDarkModeValue")]
        protected bool IsDarkModeValue { get; set; }

        [Parameter]
        public bool IsDarkMode { get; set; }

        [Parameter]
        public EventCallback<bool> IsDarkModeChanged { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        public string? currentUrl;

        protected override void OnInitialized()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        protected async Task OnDarkModeChanged(bool value)
        {
            IsDarkMode = value;
            await IsDarkModeChanged.InvokeAsync(value);
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            StateHasChanged();
        }

        protected string AppBarStyle
        {
            get
            {
                if (Theme == null) return string.Empty;

                var bgColor = IsDarkModeValue ? Theme.PaletteDark.AppbarBackground.Value : Theme.PaletteLight.AppbarBackground.Value;
                var textColor = IsDarkModeValue ? Theme.PaletteDark.AppbarText.Value : Theme.PaletteLight.AppbarText.Value;

                return $"background-color: {bgColor}; color: {textColor}; border-radius: 0px; position: relative; z-index: 2;";
            }
        }
               

        protected bool _showFilters = false;
        protected string _wordKey { get; set; } = string.Empty;

        protected string[] _questionType { get; set; } = [];
        protected List<(string Value, string DisplayName)> _questionTypeDisplay { get; set; } = [];
        protected IEnumerable<string> _questionTypeSelected { get; set; } = [];

        protected string[] _examCategory { get; set; } = [];
        protected List<(string Value, string DisplayName)> _examCategoryDisplay { get; set; } = [];
        protected IEnumerable<string> _examCategorySelected { get; set; } = [];

        protected string[] _mainArea { get; set; } = [];
        protected IEnumerable<string> _mainAreaSelected { get; set; } = [];

        protected string[] _subArea { get; set; } = [];
        protected IEnumerable<string> _subAreaSelected { get; set; } = [];

        [Inject]
        protected IQueryDispatcher _queryDispatcher { get; set; } = default!;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected NavigationManager _navigationManager { get; set; } = default!;

        private async Task GetFilterParameters()
        {
            var filterParameters = new FilterParameters
            {
                TypeQuestions = _questionTypeSelected?.ToArray() ?? Array.Empty<string>(),
                ExamCategories = _examCategorySelected?.ToArray() ?? Array.Empty<string>(),
                MainAreas = _mainAreaSelected?.ToArray() ?? Array.Empty<string>(),
                SubAreas = _subAreaSelected?.ToArray() ?? Array.Empty<string>()
            };

            filterParameters = await _queryDispatcher
                        .DispatchAsync<GetFilterParametersQuery, FilterParameters>(new GetFilterParametersQuery(filterParameters));

            _questionType = filterParameters.TypeQuestions.ToArray();
            _questionTypeDisplay = QuestionTypeHelper.GetDisplayList(_questionType);

            _examCategory = filterParameters.ExamCategories.ToArray();
            _examCategoryDisplay = ExamCategoryHelper.GetDisplayList(_examCategory);

            _mainArea = filterParameters.MainAreas.ToArray();
            _subArea = filterParameters.SubAreas.ToArray();

            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetFilterParameters();
        }

        protected async Task LoadingFilterParameters()
        {
            await GetFilterParameters();
        }

        protected void ToggleFilters()
        {
            _showFilters = !_showFilters;
        }

        protected void CloseFilters()
        {
            _showFilters = false;
        }

        protected void NavigateToRegister()
        {
            Console.WriteLine("Navigating to Register page...");
            _navigationManager.NavigateTo("/Account/Register", forceLoad: false);
        }

        protected void NavigateToLogin()
        {
            Console.WriteLine("Navigating to Login page...");
            _navigationManager.NavigateTo("/Account/Login", forceLoad: false);
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

            var categories = _examCategorySelected?.ToArray() ?? [];
            if (categories.Length > 0)
                queryParams["categories"] = string.Join(",", categories);

            var areas = _mainAreaSelected?.ToArray() ?? [];
            if (areas.Length > 0)
                queryParams["areas"] = string.Join(",", areas);

            var subAreas = _subAreaSelected?.ToArray() ?? [];
            if (subAreas.Length > 0)
                queryParams["subareas"] = string.Join(",", subAreas);

            var url = QueryHelpers.AddQueryString("/result", queryParams);

            _showFilters = false;
            _navigationManager.NavigateTo(url);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
