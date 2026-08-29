using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Inject]
        public IJSRuntime JS { get; set; } = default!;  

        public string? currentUrl;
        protected bool _drawerOpen = true;

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

        protected void GoHome()
        {
            NavigationManager.NavigateTo("/");
        }

        protected async Task SubmitLogout()
        {            
            await JS.InvokeVoidAsync("document.getElementById('logoutForm').submit");
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
