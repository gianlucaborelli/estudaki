using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models.DTOs;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public partial class UploadImagesModalBase: ComponentBase
    {
        [CascadingParameter]
        protected IMudDialogInstance Dialog { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public ICommandDispatcher CommandDispatcher { get; set; } = default!;
        [Inject]
        public ILogger<UploadImagesModalBase> Logger { get; set; } = default!;

        [Parameter]
        public PublicNoticeDto? Notice { get; set; }

        protected IReadOnlyList<IBrowserFile> SelectedFiles { get; set; } = [];
        protected bool IsLoading = false;
        protected bool IsUploading = false;
        protected bool pasteAreaFocused = false;
        protected IJSObjectReference? pasteModule;
        protected InputFile? fileInputComponent;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && pasteModule == null)
            {
                try
                {
                    pasteModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/paste-handler.js");
                    await pasteModule.InvokeVoidAsync("initialize", "pasteArea", "hiddenFileInput");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Paste module not loaded: {ex.Message}");
                }
            }
        }

        protected async Task OnFilesChanged(InputFileChangeEventArgs e)
        {
            var newFiles = e.GetMultipleFiles(100).ToList();

            // Combinar arquivos existentes com os novos
            var existingFiles = SelectedFiles?.ToList() ?? [];
            existingFiles.AddRange(newFiles);

            SelectedFiles = existingFiles;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task OpenFilePicker()
        {
            if (fileInputComponent?.Element != null)
            {
                await JSRuntime.InvokeVoidAsync("eval", $"document.getElementById('hiddenFileInput').click()");
            }
        }

        protected async Task FocusPasteArea()
        {
            pasteAreaFocused = true;
            try
            {
                await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('pasteArea')?.focus()");
            }
            catch
            {
                // Ignorar erro
            }
        }       

        protected void RemoveFile(string fileName)
        {
            var file = SelectedFiles?.FirstOrDefault(f => f.Name == fileName);
            if (file != null)
            {
                var fileList = SelectedFiles?.ToList() ?? [];
                fileList.Remove(file);
                SelectedFiles = fileList;
            }
        }

        protected bool CanUpload()
        {
            return Notice != null && SelectedFiles?.Any() == true && !IsLoading;
        }        

        protected string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F2} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }

        protected async Task UploadFiles()
        {
            if (!CanUpload() || Notice == null || SelectedFiles == null)
                return;
            
            try
            {
                List<UploadFileDto> images = (await Task.WhenAll(SelectedFiles
                        .Select(async file => await UploadFileDto.CreateAsync(file))))
                        .ToList();

                var command = new UploadQuestionImagesCommand(images, Notice.Id);

                var result = await CommandDispatcher
                    .DispatchAsync<UploadQuestionImagesCommand, ValidationResult>(command);
                
                Snackbar.Add("Upload concluído com sucesso!", Severity.Success);
                Dialog.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro geral no upload de imagens.");
                Snackbar.Add($"Erro geral no upload: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }                      
    }
}
