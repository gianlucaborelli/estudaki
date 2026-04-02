namespace Estudaki.Commons.Core.Storage;

public interface IStorageService
{
    /// <summary>
    /// Obtém a URL completa construindo o caminho
    /// </summary>
    /// <returns>URL completa para acesso ao arquivo</returns>
    string GetFileUrl();
}
