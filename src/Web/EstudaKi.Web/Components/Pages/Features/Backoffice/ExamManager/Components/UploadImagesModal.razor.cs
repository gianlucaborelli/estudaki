using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
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
        public IStorageService StorageService { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public PublicNoticeDto? Notice { get; set; }

        protected IReadOnlyList<IBrowserFile> SelectedFiles { get; set; } = [];
        protected Dictionary<string, UploadResult> uploadResults = new();
        protected bool IsLoading = false;
        protected bool IsUploading = false;
        protected int uploadedCount = 0;
        protected string? errorMessage;
        protected string? successMessage;
        protected bool pasteAreaFocused = false;
        protected IJSObjectReference? pasteModule;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (pasteModule == null)
            {
                try
                {
                    pasteModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/paste-handler.js");
                    await pasteModule.InvokeVoidAsync("initialize", "pasteArea", "fileInput");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Paste module not loaded: {ex.Message}");
                }
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
            var file = SelectedFiles.FirstOrDefault(f => f.Name == fileName);
            if (file != null)
            {
                //selectedFiles.Remove(file);
            }
        }

        protected bool CanUpload()
        {
            return Notice != null && SelectedFiles.Any() && !IsLoading;
        }

        

        protected string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F2} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }

        protected async Task UploadFiles()
        {
            if (!CanUpload() || Notice == null)
                return;

            IsLoading = true;
            uploadedCount = 0;
            uploadResults.Clear();
            errorMessage = null;
            successMessage = null;
            StateHasChanged();

            var s3ImagesFolder = "";//Notice.GetImagesFolder();
            var successCount = 0;
            var failCount = 0;

            try
            {
                foreach (var file in SelectedFiles)
                {
                    var result = new UploadResult { FileName = file.Name };

                    try
                    {
                        // Validar tamanho
                        if (file.Size > 10 * 1024 * 1024) // 10 MB
                        {
                            result.Success = false;
                            result.ErrorMessage = "Arquivo muito grande (máximo 10 MB)";
                            uploadResults[file.Name] = result;
                            failCount++;
                            continue;
                        }

                        // Validar extensão
                        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                        if (extension is not (".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" or ".bmp" or ".svg"))
                        {
                            result.Success = false;
                            result.ErrorMessage = "Formato de arquivo não suportado";
                            uploadResults[file.Name] = result;
                            failCount++;
                            continue;
                        }

                        // Upload para S3
                        using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

                        // Gerar GUID para o arquivo
                        var guid = Guid.NewGuid().ToString();
                        var newFileName = $"{guid}{extension}";
                        var s3Key = $"{s3ImagesFolder}/{newFileName}";

                        var contentType = extension switch
                        {
                            ".png" => "image/png",
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".gif" => "image/gif",
                            ".webp" => "image/webp",
                            ".svg" => "image/svg+xml",
                            _ => "image/*"
                        };

                        // await StorageService.UploadFileAsync(stream, s3Key, contentType);

                        result.Success = true;
                        result.NewKey = guid;
                        result.S3Url = $"{StorageService.GetFileUrl()}/{s3Key}";
                        uploadResults[file.Name] = result;
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.ErrorMessage = ex.Message;
                        uploadResults[file.Name] = result;
                        failCount++;
                    }

                    uploadedCount++;
                    StateHasChanged();
                }

                successMessage = $"Upload concluído: {successCount} arquivo(s) enviado(s), {failCount} falha(s)";
                                
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro geral no upload: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
               

        public async ValueTask DisposeAsync()
        {
            if (pasteModule != null)
            {
                try
                {
                    await pasteModule.InvokeVoidAsync("dispose");
                    await pasteModule.DisposeAsync();
                }
                catch
                {
                    // Ignorar erros ao fazer dispose
                }
            }
        }

        protected class UploadResult
        {
            public string FileName { get; set; } = string.Empty;
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public string? NewKey { get; set; }
            public string? S3Url { get; set; }
        }
    }
}
