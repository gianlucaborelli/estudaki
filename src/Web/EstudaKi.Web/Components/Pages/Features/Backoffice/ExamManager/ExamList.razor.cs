using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.CQRS.Dispatchers;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList;
using Estudaki.Modules.Questions.Domain.Common;
using EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;
using EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Modals;
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
    protected bool IsLoading { get; set; } = false;
    protected List<PublicNoticeDto> SelectedItems { get; set; } = [];

    protected MudTable<PublicNoticeDto>? Table;
    protected string SearchString { get; set; } = string.Empty;
    protected string FilterByExamCategory { get; set; } = string.Empty;

    //protected override async Task OnInitializedAsync()
    //{
    //    await Table.ReloadServerData();
    //}    

    protected async Task<TableData<PublicNoticeDto>> LoadServerData(
    TableState state,
    CancellationToken cancellationToken)
    {
        
        var page = state.Page;
        var pageSize = state.PageSize;

        var sortLabel = state.SortLabel;
        var sortDirection = state.SortDirection;

        var search = SearchString;
        var category = FilterByExamCategory;

        var query = new GetPublicNoticeListQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Category = category,
            SortLabel = sortLabel,
            SortDirection = sortDirection.ToString(),
        };

        var result = await QueryDispatcher.DispatchAsync<GetPublicNoticeListQuery, PagedResult<PublicNoticeDto>>(query);
        
        return new TableData<PublicNoticeDto>
        {
            Items = result.Items,
            TotalItems = Convert.ToInt32(result.TotalItems)
        };
    }

    protected async Task OnSearchChanged(string value)
    {
        await Table!.ReloadServerData();
    }

    protected async Task OnCategoryChanged(string? value)
    {
        FilterByExamCategory = value;
        await Table!.ReloadServerData();
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

    protected async Task OpenCreateNewPublicNoticeDialogAsync()
    {
        var parameters = new DialogParameters<CreateNewPublicNoticeModal>();

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<CreateNewPublicNoticeModal>("Criar Novo Edital", parameters, options);
        var result = await dialog.Result;

        if (result is not null) await Table.ReloadServerData();

        return;
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
                await Table.ReloadServerData();
            }
            else
                Snackbar.Add("Falha ao unificar as provas. Verifique se há dependências.", Severity.Error);
            await Table.ReloadServerData();
        }

        return;
    }
}
