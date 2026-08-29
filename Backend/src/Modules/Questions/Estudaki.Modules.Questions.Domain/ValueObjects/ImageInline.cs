namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ImageInline : InlineContent
{
    /// <summary>
    /// A chave de armazenamento do arquivo, será usada como parte da URL para acessar a imagem, e deve ser única para cada imagem junto com a extensão do arquivo.
    /// 
    /// Estrutura de armazenamento de arquivos a ser adotado
    /// /files/exams/{public-notice-year}/{examBoard}/{public-notice-id}/images/{Key}.png (ou outra extensão)
    /// </summary>
    public string Key { get; set; } = string.Empty;
    public string? Alt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public ImageInline()
    {
        Type = "image";
    }
}
