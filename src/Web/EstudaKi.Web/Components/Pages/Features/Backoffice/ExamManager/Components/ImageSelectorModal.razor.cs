using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models.DTOs;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetImageListByPublicNoticeId;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public partial class ImageSelectorModalBase : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance DialogInstance { get; set; } = default!;
    [Inject] private IStorageService StorageService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private ILogger<ImageSelectorModalBase> Logger { get; set; } = default!;
    [Inject] private IQueryDispatcher QueryDispatcher { get; set; } = default!;
    [Inject] private ICommandDispatcher CommandDispatcher { get; set; } = default!;

    [Parameter]
    public PublicNoticeDto? PublicNotice { get; set; }

    protected class ImageInfo
    {
        public string Key { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsLoaded { get; set; }
        public bool HasError { get; set; }
    }

    protected List<ImageInfo> images = new();
    protected List<ImageInfo> filteredImages = new();
    protected string searchTerm = string.Empty;
    protected string? selectedImageKey;
    protected bool isLoading = false;
    protected bool previousVisibility = false;
    protected bool pasteAreaFocused = false;
    protected bool isUploading = false;
    protected string? uploadMessage;
    protected string? uploadError;
    protected InputFile? pasteInputFileRef;

    protected override async Task OnParametersSetAsync()
    {
        if (PublicNotice != null)
        {
            await LoadImagesAsync();
        }
    }

    protected async Task LoadImagesAsync()
    {
        try
        {
            isLoading = true;

            var imageListPathQuery = new GetImageListByPublicNoticeIdQuery(PublicNotice!.Id);
            var imageListPath = await QueryDispatcher.DispatchAsync<GetImageListByPublicNoticeIdQuery, List<string>>(imageListPathQuery);
            
            images = imageListPath
                .Where(f => IsImageFile(f))
                .Select(f =>
                {               
                    var fullPath = f;
                    var key = Path.GetFileName(fullPath);

                    return new ImageInfo
                    {
                        Key = key,
                        Url = fullPath,
                        IsLoaded = false
                    };
                })
                .OrderBy(i => i.Key)
                .ToList();

            filteredImages = images.ToList();

            _ = LoadImagesProgressivelyAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao carregar imagens do S3");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task LoadImagesProgressivelyAsync()
    {
        foreach (var image in images)
        {
            image.IsLoaded = true;
            StateHasChanged();
            await Task.Delay(50);
        }
    }

    protected bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" or ".bmp" or ".svg";
    }

    protected void FilterImages()
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            filteredImages = images.ToList();
        }
        else
        {
            filteredImages = images
                .Where(i => i.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    protected void SelectImage(string key)
    {
        selectedImageKey = key;
        Logger.LogInformation("Imagem selecionada: {Key}", key);
    }

    protected void OnImageError(ImageInfo image)
    {
        image.HasError = true;
        Logger.LogWarning("Erro ao carregar imagem: {Key} - URL: {Url}", image.Key, image.Url);
        StateHasChanged();
    }

    protected async Task Confirm()
    {
        if (!string.IsNullOrEmpty(selectedImageKey))
        {
            Logger.LogInformation("Confirmando seleção da imagem: {Key}", selectedImageKey);
            DialogInstance.Close(DialogResult.Ok(selectedImageKey));
        }
    }

    protected async Task Close()
    {
        selectedImageKey = null;
        searchTerm = string.Empty;
        pasteAreaFocused = false;
        uploadMessage = null;
        uploadError = null;
        DialogInstance.Cancel();
    }

    protected async Task FocusPasteArea()
    {
        pasteAreaFocused = true;
        try
        {
            await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('imageSelectorPasteArea')?.focus()");
        }
        catch
        {
            // Ignorar erro
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            // Inicializar paste handler usando abordagem similar ao paste-handler.js
            await JSRuntime.InvokeVoidAsync("eval", @"
                if (!window.imageSelectorPasteHandler) {
                    console.log('[ImageSelectorModal] Criando paste handler');

                    window.imageSelectorPasteHandler = (e) => {
                        const pasteArea = document.getElementById('imageSelectorPasteArea');
                        const fileInput = document.getElementById('imageSelectorPasteInput');

                        if (!pasteArea || !fileInput) {
                            console.log('[ImageSelectorModal] Elementos não encontrados');
                            return;
                        }

                        // Só processar se for na área de paste
                        if (e.target !== pasteArea && !pasteArea.contains(e.target)) {
                            console.log('[ImageSelectorModal] Paste não está na área - ignorando');
                            return;
                        }

                        const items = e.clipboardData?.items;
                        if (!items) {
                            console.log('[ImageSelectorModal] Clipboard vazio');
                            return;
                        }

                        // Verificar se tem imagem
                        let hasImage = false;
                        for (let i = 0; i < items.length; i++) {
                            if (items[i].type.indexOf('image') !== -1) {
                                hasImage = true;
                                break;
                            }
                        }

                        if (!hasImage) {
                            console.log('[ImageSelectorModal] Sem imagem no clipboard');
                            return;
                        }

                        console.log('[ImageSelectorModal] Imagem detectada - processando');
                        e.preventDefault();

                        // Criar DataTransfer e adicionar arquivos
                        const dataTransfer = new DataTransfer();
                        let fileCount = 0;

                        for (let i = 0; i < items.length; i++) {
                            if (items[i].type.indexOf('image') !== -1) {
                                const blob = items[i].getAsFile();
                                if (blob) {
                                    const timestamp = Date.now();
                                    const ext = blob.type.split('/')[1] || 'png';
                                    const fileName = `pasted-image-${timestamp}.${ext}`;
                                    const file = new File([blob], fileName, { type: blob.type });
                                    dataTransfer.items.add(file);
                                    fileCount++;
                                    console.log('[ImageSelectorModal] Arquivo adicionado:', fileName, 'Tamanho:', blob.size);
                                }
                            }
                        }

                        if (fileCount > 0) {
                            fileInput.files = dataTransfer.files;
                            const event = new Event('change', { bubbles: true });
                            fileInput.dispatchEvent(event);
                            console.log('[ImageSelectorModal] Evento change disparado -', fileCount, 'arquivo(s)');
                        }
                    };

                    document.addEventListener('paste', window.imageSelectorPasteHandler);
                    console.log('[ImageSelectorModal] Handler registrado');
                }
            ");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao inicializar paste handler");
        }

    }

    protected async Task OnPastedImageSelected(InputFileChangeEventArgs e)
    {
        Logger.LogInformation("OnPastedImageSelected - Arquivos recebidos: {Count}", e.FileCount);

        if (PublicNotice == null)
        {
            Logger.LogError("PublicNotice é null - não é possível fazer upload");
            uploadError = "Erro: PublicNotice não está carregado";
            StateHasChanged();
            return;
        }

        if (isUploading)
        {
            Logger.LogWarning("Upload já em andamento - ignorando");
            return;
        }

        try
        {
            isUploading = true;
            uploadError = null;
            uploadMessage = "Enviando imagem...";
            pasteAreaFocused = false;
            StateHasChanged();

            var file = e.File;
            Logger.LogInformation("Processando arquivo: {FileName}, Tipo: {ContentType}, Tamanho: {Size}",
                file.Name, file.ContentType, file.Size);

            var imageList = new List<UploadFileDto> { await UploadFileDto.CreateAsync(file) };
            var uploadImageCommand = new UploadQuestionImagesCommand(imageList, PublicNotice.Id);

            var result = await CommandDispatcher.DispatchAsync<UploadQuestionImagesCommand, ValidationResult>(uploadImageCommand);

            if(!result.IsValid)
            {
                var errors = string.Join("; ", result.Errors.Select(err => err.ErrorMessage));
                Logger.LogError("Erro ao enviar imagem: {Errors}", errors);
                uploadError = $"Erro ao enviar imagem: {errors}";
                return;
            }
            await LoadImagesAsync();

            StateHasChanged();

            await Task.Delay(3000);
            uploadMessage = null;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao fazer upload de imagem colada");
            uploadError = $"Erro ao enviar imagem: {ex.Message}";
        }
        finally
        {
            isUploading = false;
            StateHasChanged();
        }
    }       
}
