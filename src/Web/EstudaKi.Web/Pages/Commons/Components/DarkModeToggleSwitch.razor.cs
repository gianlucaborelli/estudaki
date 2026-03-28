using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Pages.Commons.Components;

public partial class DarkModeToggleSwitchBase : ComponentBase
{
    [Parameter]
    public bool IsDarkMode { get; set; }

    [Parameter]
    public EventCallback<bool> IsDarkModeChanged { get; set; }

    protected async Task DarkModeToggle()
    {
        await IsDarkModeChanged.InvokeAsync(!IsDarkMode);
    }

    public string DarkLightModeButtonIcon => IsDarkMode switch
    {
        true => Icons.Material.Rounded.LightMode,
        false => Icons.Material.Outlined.DarkMode,
    };
}
