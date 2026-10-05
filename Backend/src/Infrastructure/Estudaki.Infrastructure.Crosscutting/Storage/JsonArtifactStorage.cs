using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Estudaki.Commons.Core.Storage;

namespace Estudaki.Infrastructure.Crosscutting.Storage;

/// <summary>
/// Implementação de <see cref="IJsonArtifactStorage"/> baseada em <see cref="IStorageService"/>,
/// responsável por serializar objetos e enviá-los ao bucket sob uma estrutura de pastas
/// previsível: "{container}/{requestId}/{itemId}.json".
/// </summary>
public class JsonArtifactStorage : IJsonArtifactStorage
{
    private readonly IStorageService _storageService;

    public JsonArtifactStorage(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public string BuildKey(string container, string requestId, string itemId)
        => $"{container.Trim('/')}/{requestId}/{itemId}.json";

    public async Task<string> SaveAsync<TValue>(
        string container,
        string requestId,
        string itemId,
        TValue value,
        CancellationToken cancellationToken = default)
    {
        var key = BuildKey(container, requestId, itemId);
        var json = JsonSerializer.Serialize(value);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await _storageService.UploadFileAsync(stream, key, MediaTypeNames.Application.Json);

        return key;
    }
}
