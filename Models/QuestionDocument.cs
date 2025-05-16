using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProvaOnline.Models
{
    /// <summary>
    /// Represents a question document.
    /// </summary>
    public class QuestionDocument
    {
        /// <summary>
        /// Seleciona ou atribui um ID único para o documento.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        /// <summary>
        /// Data de criação do documento.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica se a questão já foi publicada.
        /// </summary>
        public bool IsPublished { get; set; } = false;

        /// <summary>
        /// Indica se a questão foi anulada.
        /// </summary>
        public bool? IsNullified { get; set; } = false;

        /// <summary>
        /// Edital do Exame.
        /// </summary>
        public PublicNotice? PublicNotice { get; set; }

        /// <summary>
        /// Número da questão na prova.
        /// </summary>
        public int QuestionNumber { get; set; }

        public string QuestionType { get; set; }

        /// <summary>
        /// Texto da questão.
        /// </summary>
        public string QuestionBody { get; set; }

        /// <summary>
        /// Texto ou Imagem de apoio a questão.
        /// </summary>
        public List<QuestionSupport> Supports { get; set; } = new List<QuestionSupport>();

        /// <summary>
        /// Alternativas de resposta para a questão.
        /// </summary>
        public List<Choice>? Choices { get; set; }

        /// <summary>
        /// Principal área de conhecimento da questão.
        /// </summary>
        public required string MainArea { get; set; }

        /// <summary>
        /// Subáreas de conhecimento da questão.
        /// </summary>
        public string[] SubAreas { get; set; } = [];
    }
}
