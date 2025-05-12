using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.Models
{
    public class Choice
    {
        /// <summary>
        /// Ordem de opção de resposta, de acordo com a prova.
        /// </summary>
        public string? Option { get; set; }

        /// <summary>
        /// Texto da opção de resposta, pode ser nula quando a resposta for baseada em analise de imagem.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Imagem de apoio a opção de resposta, pode ser nula quando a resposta for baseada em texto.
        /// </summary>
        [BsonIgnoreIfNull]
        public ImageSupport? SupportImage { get; set; }

        /// <summary>
        /// Define se a resposta é correta ou não, de acordo com o gabarito ofícial.
        /// </summary>
        public bool IsCorrect { get; set; }        
    }
}
