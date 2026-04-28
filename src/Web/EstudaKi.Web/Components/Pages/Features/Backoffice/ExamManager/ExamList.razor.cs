using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager;

public class ExamListBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] 
    protected IDialogService DialogService { get; set; } = default!;
    [Inject]
    protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
    protected List<PublicNoticeDto> PublicNoticeList { get; set; } = [];
    protected bool IsLoading { get; set; } = true;
    protected List<PublicNoticeDto> SelectedItems { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await LoadPublicNotices();
    }

    private async Task LoadPublicNotices()
    {
        IsLoading = true;
        try
        {
            PublicNoticeList = await QueryDispatcher.DispatchAsync<GetPublicNoticeListQuery, List<PublicNoticeDto>>(new GetPublicNoticeListQuery());
        }
        catch (Exception ex)
        {
            // Handle the exception (e.g., log it, show a message to the user, etc.)
            Console.WriteLine($"Error loading public notices: {ex.Message}");
            PublicNoticeList = new List<PublicNoticeDto>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void OnSelectedItemsChanged(HashSet<PublicNoticeDto> selectedItems)
    {
        SelectedItems = selectedItems.ToList();
    }

    protected void Edit(PublicNoticeDto item)
    {
        NavigationManager.NavigateTo($"/Backoffice/ExamManager/{item.Id}");
        Console.WriteLine($"Edit item with ID: {item.Id}");
        
    }

    protected async void Delete(PublicNoticeDto item)
    {
        bool? result = await DialogService.ShowMessageBoxAsync(
        "ATENÇÃO",
        "Deseja realmente deletar este item?",
        yesText: "Continuar", cancelText: "Cancelar");
        //TO-DO: Implement the deletion logic here
        if (result != null) { 
            Snackbar.Add($"Item {item.Id} deletado", Severity.Success);
        }
        StateHasChanged();
    }
}
