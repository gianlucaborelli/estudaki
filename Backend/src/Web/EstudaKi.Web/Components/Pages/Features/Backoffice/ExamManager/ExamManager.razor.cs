using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByExamId;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;
using Estudaki.Modules.Questions.Domain.Entities;
using EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;
using EstudaKi.Web.Components.Shared;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager
{
    public partial class ExamManagerBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        [SupplyParameterFromQuery(Name = "examId")]
        public string? ExamId { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected IDialogService Dialog { get; set; } = default!;
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
        [Inject]
        protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

        protected bool IsLoading { get; set; } = false;
        protected PublicNoticeDto PublicNotice { get; set; } = default!;
        protected List<QuestionDto> QuestionList { get; set; } = [];
        protected List<QuestionSupportDto> QuestionSupports { get; set; } = [];
        protected QuestionDto? SelectedQuestion { get; set; } = null;
        protected Exam? SelectedExam { get; set; } = null;
        protected QuestionSupportDto? SelectedQuestionSupport { get; set; }
        protected bool IsReviewingQuestions { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadContent();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(ExamId) && SelectedExam?.Id != ExamId)
            {
                await LoadContent();
            }
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

                if (!string.IsNullOrEmpty(ExamId))
                {
                    SelectedExam = PublicNotice.Exams.FirstOrDefault(e => e.Id == ExamId);
                }

                SelectedExam ??= PublicNotice.Exams.FirstOrDefault();

                if (SelectedExam != null)
                {
                    var questionsQuery = new GetQuestionsByExamIdQuery(SelectedExam.Id);
                    QuestionList = await QueryDispatcher
                                                .DispatchAsync<GetQuestionsByExamIdQuery, List<QuestionDto>>(questionsQuery);
                }

                var questionSupportsQuery = new GetQuestionSupportsByPublicNoticeIdQuery(PublicNotice.Id);
                QuestionSupports = await QueryDispatcher
                                            .DispatchAsync<GetQuestionSupportsByPublicNoticeIdQuery, List<QuestionSupportDto>>(questionSupportsQuery);
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

        protected async Task OnSelectedExamChanged(Exam? newExam)
        {
            if (newExam == null || newExam.Id == SelectedExam?.Id) return;

            SelectedExam = newExam;
            SelectedQuestion = null; // Limpar seleção de questão ao trocar de exame

            // Atualizar a URL com o examId na query string
            var uri = NavigationManager.GetUriWithQueryParameter("examId", newExam.Id);
            NavigationManager.NavigateTo(uri, false);

            // Recarregar as questões do novo exame
            IsLoading = true;
            try
            {
                var questionsQuery = new GetQuestionsByExamIdQuery(newExam.Id);
                QuestionList = await QueryDispatcher
                                            .DispatchAsync<GetQuestionsByExamIdQuery, List<QuestionDto>>(questionsQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading questions: " + ex.Message);
                Snackbar.Add("Erro ao carregar questões do exame.", Severity.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async Task OpenUploadFileDialogAsync()
        {
            var examId = SelectedExam?.Id ?? PublicNotice?.Exams?.FirstOrDefault()?.Id;

            if (string.IsNullOrEmpty(examId))
            {
                Snackbar.Add("Nenhum exame disponível para upload de arquivos.", Severity.Warning);
                return;
            }

            var parameters = new DialogParameters<UploadExamFilesModal> {
                { x => x.Notice, PublicNotice },
                { x => x.ExamId, examId }
            };

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
            var parameters = new DialogParameters<EditPublicNoticeModal> {
                { x => x.OriginalNoticePublic, PublicNotice }
            };
            
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<EditPublicNoticeModal>("Editar Edital", parameters, options);           
            var result = await dialog.Result;

            if (result is not null && !result.Canceled) await LoadContent();

            return ;
        }

        protected async Task OpenEditExamDialogAsync()
        {
            var parameters = new DialogParameters<EditExamModal> {
                { x => x.OriginalExam, SelectedExam }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<EditExamModal>("Editar Exame", parameters, options);
            var result = await dialog.Result;

            if (result is not null && !result.Canceled) await LoadContent();

            return;
        }

        protected async Task OpenUploadImagesDialogAsync()
        {           
            var parameters = new DialogParameters<UploadImagesModal> {
                { x => x.Notice, PublicNotice }
            };
            
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<UploadImagesModal>("Adicionar Imagens", parameters, options);           
            var result = await dialog.Result;

            if (result is not null && !result.Canceled) await LoadContent();

            return ;
        }

        protected async Task ReviewQuestionsWithAIAsync()
        {
            if (PublicNotice is null) return;

            var parameters = new DialogParameters<QuestionReviewResultsModal> {
                { x => x.PublicNoticeId, PublicNotice.Id }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.False, FullWidth = true };
            await Dialog.ShowAsync<QuestionReviewResultsModal>("Revisão de Questões (IA)", parameters, options);
        }

        protected async Task AddNewQuestionEditorModalAsync()
        {
            if (SelectedExam is null) 
            {
                return;
            }

            var newQuestion = QuestionDto.Create(PublicNotice, SelectedExam);

            var parameters = new DialogParameters<QuestionEditorModal>{
                { x => x.Question, newQuestion },
                { x => x.AvailableQuestionSupports, QuestionSupports },
                { x => x.PublicNotice, PublicNotice }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<QuestionEditorModal>("Editar Questão", parameters, options);           
            var result = await dialog.Result;
            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return ;
        }

        protected async Task OpenQuestionSupportEditorModalAsync(bool isNew = false)
        {
            if (isNew) SelectedQuestionSupport = new QuestionSupportDto();

            if (SelectedQuestionSupport is null) return;

            var parameters = new DialogParameters<QuestionSupportEditorModal>{
                { x => x.QuestionSupport, SelectedQuestionSupport },
                { x => x.Notice, PublicNotice },
                { x => x.IsNew, isNew }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<QuestionSupportEditorModal>("Editar Suporte de Questão", parameters, options);           
            var result = await dialog.Result;
            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return ;
        }

        protected async Task DeleteQuestionAsync()
        {
            if (SelectedQuestion == null) return;
            var parameters = new DialogParameters<CustomDialog>
            {
                { x => x.ContentText, "Deseja realmente excluir esta questão? Este processo não pode ser desfeito." },
                { x => x.ButtonText, "Excluir" },
                { x => x.Color, Color.Error }
            };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = await Dialog.ShowAsync<CustomDialog>("Confirmar Delete", parameters, options);
            var result = await dialog.Result;

            if (result is not null && !result.Canceled)
            {
                var deleteQuery = new DeleteQuestionCommand(SelectedQuestion.QuestionId, SelectedExam!.Id);
                var deleteResult = await CommandDispatcher.DispatchAsync<DeleteQuestionCommand, ValidationResult>(deleteQuery);
                if (deleteResult.IsValid)
                {
                    Snackbar.Add("Questão excluída com sucesso!", Severity.Success);
                    SelectedQuestion = null;
                    await LoadContent();
                }
                else
                    Snackbar.Add("Falha ao excluir a questão. Verifique se há dependências.", Severity.Error);
            }
            return;
        }

        protected async Task DeleteQuestionSupportAsync()
        {
            if (SelectedQuestionSupport == null) return;
            var parameters = new DialogParameters<CustomDialog>
            {
                { x => x.ContentText, "Deseja realmente excluir este suporte de questão? Este processo não pode ser desfeito." },
                { x => x.ButtonText, "Excluir" },
                { x => x.Color, Color.Error }
            };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = await Dialog.ShowAsync<CustomDialog>("Confirmar Delete", parameters, options);
            var result = await dialog.Result;

            if (result is not null && !result.Canceled)
            {
                var deleteQuery = new DeleteQuestionSupportCommand(SelectedQuestionSupport.Id);
                var deleteResult = await CommandDispatcher.DispatchAsync<DeleteQuestionSupportCommand, ValidationResult>(deleteQuery);
                if (deleteResult.IsValid)
                {
                    Snackbar.Add("Suporte de questão excluído com sucesso!", Severity.Success);
                    await LoadContent();
                }
                else
                    Snackbar.Add("Falha ao excluir o suporte de questão. Verifique se há dependências.", Severity.Error);
                await LoadContent();
            }

            return;
        }

        protected async Task OpenQuestionUnifyDialogAsync()
        {
            var parameters = new DialogParameters<QuestionUnifyModal>
            {
                { x => x.PublicNoticeId, PublicNotice.Id }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
            var dialog = await Dialog.ShowAsync<QuestionUnifyModal>("Editar Suporte de Questão", parameters, options);
            var result = await dialog.Result;
            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return;
        }

        protected async Task OpenAddExistingQuestionIntoExamModal()
        {
            if (SelectedExam == null)
            {
                Snackbar.Add("Selecione um exame para unificar as questões.", Severity.Warning);
                return;
            }
            var parameters = new DialogParameters<AddExistingQuestionIntoExamModal>
            {
                { x => x.ExamId, SelectedExam?.Id },
                { x => x.PublicNoticeId, PublicNotice.Id }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
            var dialog = await Dialog.ShowAsync<AddExistingQuestionIntoExamModal>("Adicionar Questão Existente ao Exame", parameters, options);
            var result = await dialog.Result;
            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return;
        }

        protected async Task OpenQuestionEditorModalAsync()
        {
            if (SelectedQuestion is null)
            {
                return;
            }
            var parameters = new DialogParameters<QuestionEditorModal>{
                { x => x.Question, SelectedQuestion },
                { x => x.AvailableQuestionSupports, QuestionSupports },
                { x => x.PublicNotice, PublicNotice }
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await Dialog.ShowAsync<QuestionEditorModal>("Editar Questão", parameters, options);
            var result = await dialog.Result;
            if (result is not null && !result.Canceled)
            {
                await LoadContent();
            }
            return;
        }

        protected void QuestionsRowClickEvent(TableRowClickEventArgs<QuestionDto> tableRowClickEventArgs)
        {
            SelectedQuestion = tableRowClickEventArgs.Item;
        }

        protected void QuestionSupportRowClickEvent(TableRowClickEventArgs<QuestionSupportDto> tableRowClickEventArgs)
        {
            SelectedQuestionSupport = tableRowClickEventArgs.Item;
        }

        protected string SelectedQuestionRowClassFunc(QuestionDto question, int rowNumber)
            => SelectedQuestion == question ? "selected" : string.Empty;

        protected string SelectedQuestionSupportRowClassFunc(QuestionSupportDto questionSupport, int rowNumber)
            => SelectedQuestionSupport == questionSupport ? "selected" : string.Empty;
    }
}
