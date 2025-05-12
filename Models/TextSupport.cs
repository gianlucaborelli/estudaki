namespace ProvaOnline.Models
{
    public class TextSupport : QuestionSupport
    {
        /// <summary>
        /// Texto da questão, pode ser nulo quando a resposta for baseada em analise de imagem.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Título do texto, pode ser nulo.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Fonte do texto, pode ser nula.
        /// </summary>
        public string? Source { get; set; }
    }
}
