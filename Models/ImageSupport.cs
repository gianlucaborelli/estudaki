using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.Models
{
    public class ImageSupport : QuestionSupport
    {
        /// <summary>
        /// Tipo de arquivo da imagem.
        /// </summary>
        public required string ContentType { get; set; }

        /// <summary>
        /// String Base64 da imagem.
        /// </summary>
        public required string Base64 { get; set; }

        /// <summary>
        /// Título ou legenda descritiva da imagem.
        /// </summary>
        [BsonIgnoreIfNull]  // Ignora se for nulo
        public string? Title { get; set; }

        /// <summary>
        /// Fonte de onde a imagem foi obtida, como autor, instituição ou URL.
        /// </summary>
        [BsonIgnoreIfNull]  // Ignora se for nulo
        public string? Source { get; set; }

        /// <summary>
        /// Descrição adicional da imagem que complementa seu contexto ou conteúdo.
        /// </summary>
        [BsonIgnoreIfNull]  // Ignora se for nulo
        public string? Description { get; set; }
    }
}
