using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models.DTOs;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public partial class UploadExamFilesModalBase : ComponentBase
    {
        [CascadingParameter]
        protected IMudDialogInstance Dialog { get; set; } = default!;
        [Inject]
        protected ILogger<UploadExamFilesModalBase> Logger { get; set; } = default!;
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        protected ICommandDispatcher CommandDispatcher { get; set; } = default!;

        [Parameter]
        public PublicNoticeDto? Notice { get; set; }

        [Parameter]
        public string? ExamId { get; set; }

        protected IBrowserFile? examFile;
        protected IBrowserFile? answerKeyFile;
        protected bool IsUploading = false;        

        protected bool CanUpload()
        {
            return Notice != null && !string.IsNullOrEmpty(ExamId) && examFile != null && answerKeyFile != null;
        }

        protected async Task UploadFiles()
        {
            if (!CanUpload() || Notice == null || string.IsNullOrEmpty(ExamId) || examFile == null || answerKeyFile == null)
                return;

            IsUploading = true;
            StateHasChanged();

            try
            {                
                var examFileToUpload = await UploadFileDto.CreateAsync(examFile);

                var answerKeyFileToUpload = await UploadFileDto.CreateAsync(answerKeyFile);

                var uploadFileCommand = new UploadExamFilesCommand(Notice.Id, ExamId, examFileToUpload, answerKeyFileToUpload);

                var result = await CommandDispatcher.DispatchAsync<UploadExamFilesCommand, ValidationResult>(uploadFileCommand);

                if(result.IsValid)
                {
                    Snackbar.Add("Arquivos enviados com sucesso.", Severity.Success);
                    Dialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                    foreach (var error in result.Errors)
                    {
                        Snackbar.Add(error.ErrorMessage, Severity.Error);
                    }
                    Logger.LogError("Validation failed for uploading files for public notice {NoticeId}: {Errors}", Notice.Id, errors);
                    Dialog.Close(DialogResult.Ok(false));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error uploading files for public notice {NoticeId}", Notice.Id);
            }
            finally
            {
                IsUploading = false;
                StateHasChanged();
            }
        }
    }
}
