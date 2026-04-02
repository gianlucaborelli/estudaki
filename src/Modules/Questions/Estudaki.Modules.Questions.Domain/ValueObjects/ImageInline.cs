namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ImageInline : InlineContent
{
    //Estrutura de armazenamento de arquivos a ser adotado
    // /files/exams/{public-notice-year}/{examBoard}/{public-notice-id}/questions/{Key}.png (ou outra extensão)

    /// <summary>
    /// A chave de armazenamento do arquivo, será usada como parte da URL para acessar a imagem, e deve ser única para cada imagem, provavelmente um guid.
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
