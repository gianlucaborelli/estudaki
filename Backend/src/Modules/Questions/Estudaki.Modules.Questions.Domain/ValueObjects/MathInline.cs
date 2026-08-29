namespace Estudaki.Modules.Questions.Domain.ValueObjects
{
    /// <summary>
    /// Representa uma fórmula matemática inline compatível com MathQuill/LaTeX
    /// </summary>
    public class MathInline : InlineContent
    {
        /// <summary>
        /// Fórmula matemática em formato LaTeX
        /// Exemplos: "x^2 + 2x + 1", "\frac{a}{b}", "\sqrt{x}"
        /// </summary>
        public string Latex { get; set; } = string.Empty;

        public MathInline()
        {
            Type = "math";
        }
    }
}
