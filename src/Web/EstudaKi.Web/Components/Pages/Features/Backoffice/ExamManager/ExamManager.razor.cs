using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager
{
    public partial class ExamManagerBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected IDialogService Dialog { get; set; } = default!;
        [Inject]
        protected IQueryDispatcher QueryDispatcher { get; set; } = default!;

        protected bool IsLoading { get; set; } = false;
        protected PublicNoticeDto PublicNotice { get; set; } = default!;
        protected List<QuestionDto> QuestionList { get; set; } = [];
        protected QuestionDto? SelectedQuestion { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadContent();
        }

        private async Task LoadContent()
        {
            IsLoading = true;
            try
            {
                var publicNoticeQuery = new GetPublicNoticeByIdQuery(Id);
                var publicNotice = await QueryDispatcher
                                            .DispatchAsync<GetPublicNoticeByIdQuery, PublicNoticeDto?>(publicNoticeQuery);
                if (publicNotice == null)
                {
                    NavigationManager.NavigateTo("/Error/404");
                    return;
                }
                PublicNotice = publicNotice;

                var questionsQuery = new GetQuestionsByPublicNoticeIdQuery(Id);
                var questions = await QueryDispatcher
                                            .DispatchAsync<GetQuestionsByPublicNoticeIdQuery, List<QuestionDto>>(questionsQuery);
                QuestionList = questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
                NavigationManager.NavigateTo("/Error/500");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void QuestionsRowClickEvent(TableRowClickEventArgs<QuestionDto> tableRowClickEventArgs)
        {
            SelectedQuestion = tableRowClickEventArgs.Item;
        }

        protected async Task OpenUploadFileDialogAsync()
        {
            var parameters = new DialogParameters<UploadExamFilesModal>();
            parameters.Add(nameof(UploadExamFilesModal.Notice), PublicNotice);
            
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

            var dialog = await Dialog.ShowAsync<UploadExamFilesModal>("Upload de Arquivos do Edital", parameters, options);           

            var result = await dialog.Result;

            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return ;
        }

        protected async Task OpenEditPublicNoticeDialogAsync()
        {
            var parameters = new DialogParameters<EditPublicNoticeModal>();
            parameters.Add(nameof(EditPublicNoticeModal.OriginalNoticePublic), PublicNotice);
            
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<EditPublicNoticeModal>("Editar Edital", parameters, options);           
            var result = await dialog.Result;
            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return ;
        }

        protected string SelectedQuestionRowClassFunc(QuestionDto question, int rowNumber)
            => SelectedQuestion == question ? "selected" : string.Empty;
    }
}
