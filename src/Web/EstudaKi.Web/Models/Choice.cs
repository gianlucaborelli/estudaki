using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    public class Choice
    {
        /// <summary>
        /// Ordem de opção de resposta, de acordo com a prova.
        /// </summary>
        public string? Option { get; set; }

        /// <summary>
        /// Conteúdo da opção de resposta.Pode ser texto, imagem ou ambos, dependendo do formato da questão. P
        /// </summary>
        public List<InlineContent> Content { get; set; } = [];

        /// <summary>
        /// Define se a resposta é correta ou não, de acordo com o gabarito ofícial.
        /// </summary>
        public bool IsCorrect { get; set; }
    }
}
