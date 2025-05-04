using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProvaOnline.Components.Layout
{
    public partial class MainLayoutPage : LayoutComponentBase
    {
        protected bool _drawerOpen = true;
        protected bool _isDarkMode = true;
        protected MudTheme? _theme = null;

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

        protected void DarkModeToggle()
        {
            _isDarkMode = !_isDarkMode;
        }

        private readonly PaletteLight _lightPalette = new()
        {
            Primary = "#5c6600",
            Black = "#ecf1ea",
            AppbarText = "#424242",
            AppbarBackground = "#b9c9af",
            DrawerBackground = "#f5f5f5",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",            
        }; 

        private readonly PaletteDark _darkPalette = new()
        {
            Primary = "#5c6600",
            Surface = "#1e1e2d",
            Background = "#0d1210",
            BackgroundGray = "#151521",
            AppbarText = "#92929f",
            AppbarBackground = "#111614",
            DrawerBackground = "#101714",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextPrimary = "#b2b0bf",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#3dcb6c",
            Warning = "#ffb545",
            Error = "#ff3f5f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80",
        };

        public string DarkLightModeButtonIcon => _isDarkMode switch
        {
            true => Icons.Material.Rounded.AutoMode,
            false => Icons.Material.Outlined.DarkMode,
        };
    }
}
