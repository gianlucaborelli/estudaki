using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    [BsonDiscriminator("ImageBlock")]
    public class ImageBlock : ContentBlock
    {
        /// <summary>
        /// Chave única para identificar a imagem.
        /// Padrão: "questions/{year}/{publicNoticeId}/{questionId}/{guid}.ext"
        /// </summary>
        public string Key { get; set; } // ex: "questions/2023/q1/img1.png"

        /// <summary>
        /// Título da imagem.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Fonte da imagem.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Descrição da imagem.
        /// </summary>
        public string? Description { get; set; }
    }
}
