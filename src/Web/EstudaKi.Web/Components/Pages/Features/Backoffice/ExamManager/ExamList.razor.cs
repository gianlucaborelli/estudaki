using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.CQRS.Dispatchers;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using EstudaKi.Web.Components.Shared;
using FluentValidation.Results;
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
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
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

    protected async Task UnifyPublicNoticeAsync()
    {
        if (SelectedItems.Count < 2) return;
        var parameters = new DialogParameters<CustomDialog>
            {
                { x => x.ContentText, "Deseja realmente unificar essas provas? Este processo não pode ser desfeito." },
                { x => x.ButtonText, "Unificar" },
                { x => x.Color, Color.Error }
            };
        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
        var dialog = await DialogService.ShowAsync<CustomDialog>("Confirmar Unificação", parameters, options);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            var publicNoticeIds = SelectedItems.Select(x => x.Id).ToList();


            var unifyCommand = new UnifyPublicNoticeCommand(publicNoticeIds);
            var deleteResult = await CommandDispatcher.DispatchAsync<UnifyPublicNoticeCommand, ValidationResult>(unifyCommand);
            if (deleteResult.IsValid)
            {
                Snackbar.Add("Provas unificadas com sucesso!", Severity.Success);
                await LoadPublicNotices();
            }
            else
                Snackbar.Add("Falha ao unificar as provas. Verifique se há dependências.", Severity.Error);
            await LoadPublicNotices();
        }

        return;
    }
}
