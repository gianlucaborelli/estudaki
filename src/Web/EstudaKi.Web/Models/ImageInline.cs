using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    [BsonDiscriminator("ImageInline")]
    public class ImageInline : InlineContent
    {
        /// <summary>
        /// Chave única para identificar a imagem.
        /// Padrão: "questions/{year}/{publicNoticeId}/{questionId}/{guid}.ext"
        /// </summary>
        public string Key { get; set; } // ex: "questions/2023/q1/img1.png"

        public string? Alt { get; set; } = null;

        public int Width { get; set; } // ex: 50px
        public int Height { get; set; }
    }
}
