namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ImageBlock : ContentBlock
{
    //Estrutura de armazenamento de arquivos a ser adotado
    // /files/exams/{public-notice-year}/{examBoard}/{public-notice-id}/questions/{Key}.png (ou outra extensão)
    public string Key { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
}
