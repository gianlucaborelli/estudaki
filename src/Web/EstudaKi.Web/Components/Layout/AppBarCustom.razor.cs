using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Domain.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;

namespace EstudaKi.Web.Components.Layout
{
    public partial class AppBarCustomBase : ComponentBase
    {
        [CascadingParameter(Name = "Theme")]
        protected MudTheme? Theme { get; set; }

        [CascadingParameter(Name = "IsDarkModeValue")]
        protected bool IsDarkModeValue { get; set; }

        [Parameter]
        public bool IsDarkMode { get; set; }

        [Parameter]
        public EventCallback<bool> IsDarkModeChanged { get; set; }

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
        protected IEnumerable<string> _questionTypeSelected { get; set; } = [];

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
                MainAreas = _mainAreaSelected?.ToArray() ?? Array.Empty<string>(),
                SubAreas = _subAreaSelected?.ToArray() ?? Array.Empty<string>()
            };

            filterParameters = await _queryDispatcher
                        .DispatchAsync<GetFilterParametersQuery, FilterParameters>(new GetFilterParametersQuery(filterParameters));

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

        protected void ToggleFilters()
        {
            _showFilters = !_showFilters;
        }

        protected void CloseFilters()
        {
            _showFilters = false;
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

            _showFilters = false;
            _navigationManager.NavigateTo(url);
        }
    }
}
