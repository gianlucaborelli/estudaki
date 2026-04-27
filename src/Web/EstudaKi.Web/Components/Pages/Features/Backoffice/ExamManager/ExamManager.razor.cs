using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager
{
    public partial class ExamManagerBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected IQueryDispatcher QueryDispatcher { get; set; } = default!;

        protected bool IsLoading { get; set; } = false;
        protected PublicNoticeDto PublicNotice { get; set; } = default!;
        protected List<QuestionDto> QuestionList { get; set; } = [];
        protected QuestionDto? SelectedQuestion { get; set; } = null;

        protected override async Task OnInitializedAsync()
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
            }catch (Exception ex)
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

        protected string SelectedQuestionRowClassFunc(QuestionDto question, int rowNumber)
            => SelectedQuestion == question ? "selected" : string.Empty;
    }
}
