using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace ProvaOnline.Components.Layout
{
    public partial class MainLayoutPage : LayoutComponentBase
    {
        protected bool _drawerOpen = true;
        protected bool _isDarkMode = true;
        protected MudTheme? _theme = null;

        protected Breakpoint _currentBreakpoint;
        protected string _drawerWidth = "280px";

        [Inject] private IJSRuntime JS { get; set; } = default!;
        protected void OnBreakpointChanged(Breakpoint breakpoint)
        {
            _currentBreakpoint = breakpoint;

            _drawerWidth = breakpoint switch
            {
                Breakpoint.Xs => "100%",
                Breakpoint.Sm => "100%",
                Breakpoint.Md => "340px",
                Breakpoint.Lg => "380px",
                _ => "400px"
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                bool prefersDark = await JS.InvokeAsync<bool>("getPreferredColorScheme");
                _isDarkMode = prefersDark;
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _theme = new()
            {
                PaletteLight = _lightPalette,
                PaletteDark = _darkPalette,
                LayoutProperties = new LayoutProperties()
            };
        }

        protected void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        public void CloseDrawer()
        {
            if (_currentBreakpoint == Breakpoint.Xs || _currentBreakpoint == Breakpoint.Sm)
            {
                _drawerOpen = false;
                StateHasChanged();
            }
        }

        protected void DarkModeToggle()
        {
            _isDarkMode = !_isDarkMode;
        }

        private readonly PaletteLight _lightPalette = new()
        {
            Primary = "#7aa33e",
            Background = "#f9f9f6",
            Surface = "#ffffff",
            AppbarBackground = "#5e6f45",
            AppbarText = "#ffffff",
            DrawerBackground = "#f9f9f6",
            DrawerText = "#4c4c4c",
            DrawerIcon = "#5d5d5d",
            TextPrimary = "#2e2e2e",
            TextSecondary = "#5e5e5e",
            TextDisabled = "#9a9a9a",
            ActionDefault = "#5c5c5c",
            ActionDisabled = "#c0c0c080",
            ActionDisabledBackground = "#e0e0e066",
            Divider = "#dedede",
            LinesDefault = "#dddddd",
            TableLines = "#e5e5e5",
            Success = "#5faa47",  
            Warning = "#d7a93b",
            Error = "#cc4c4c",
            Info = "#4a86ff",
            GrayLight = "#e9e9e9",
            GrayLighter = "#f5f5f5",
            OverlayLight = "#ffffff99" 
        };

        private readonly PaletteDark _darkPalette = new()
        {
            Primary = "#7aa33e", 
            Surface = "#101714",
            Background = "#111614",
            BackgroundGray = "#151521",
            AppbarText = "#b0b6a8",
            AppbarBackground = "#0d1210",
            DrawerBackground = "#111614",
            ActionDefault = "#939b89", 
            ActionDisabled = "#6f6f6f80",
            ActionDisabledBackground = "#4e4e4e33",            
            TextPrimary = "#b6bfae",  
            TextSecondary = "#8f9685", 
            TextDisabled = "#b6bfae55", 
            DrawerIcon = "#a7b29a",
            DrawerText = "#a7b29a",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#5faa47", 
            Warning = "#e5b657",
            Error = "#e15b5b",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80"
        };

        public string DarkLightModeButtonIcon => _isDarkMode switch
        {
            true => Icons.Material.Rounded.LightMode,
            false => Icons.Material.Outlined.DarkMode,
        };
    }
}
