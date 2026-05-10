namespace Estudaki.Commons.Core.Storage;

public interface IStorageService
{
    /// <summary>
    /// Obtém a URL completa construindo o caminho
    /// </summary>
    /// <returns>URL completa para acesso ao arquivo</returns>
    string GetFileUrl();

    /// <summary>
    /// Faz upload de um arquivo para o S3
    /// </summary>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Verifica se um arquivo existe no S3
    /// </summary>
    Task<bool> FileExistsAsync(string fileName);

    /// <summary>
    /// Deleta um arquivo do S3
    /// </summary>
    Task DeleteFileAsync(string fileName);

    /// <summary>
    /// Lista todos os arquivos em uma pasta do S3
    /// </summary>
    Task<List<string>> ListFilesAsync(string folderPath);

    /// <summary>
    /// Upload de imagem local para S3 com novo nome (GUID)
    /// </summary>
    Task<string> UploadImageFromLocalAsync(string localFilePath, string s3FolderPath);

    /// <summary>
    /// Copia um arquivo de uma localização para outra dentro do S3
    /// </summary>
    Task<string> CopyFileAsync(string sourceKey, string destinationKey);

    /// <summary>
    /// Move um arquivo de uma localização para outra dentro do S3 (copia e deleta o original)
    /// </summary>
    Task<string> MoveFileAsync(string sourceKey, string destinationKey);

    /// <summary>
    /// Copia todos os arquivos de uma pasta para outra dentro do S3
    /// </summary>
    Task<List<string>> CopyFolderAsync(string sourceFolderPath, string destinationFolderPath);
}
