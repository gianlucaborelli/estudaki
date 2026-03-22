using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProvaOnline.Models;

namespace ProvaOnline.ModelsV2
{
    public class QuestionDocumentV2
    {
        /// <summary>
        /// Seleciona ou atribui um ID único para o documento.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Edital do Exame.
        /// </summary>
        public string? PublicNoticeId { get; set; }

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
        /// Número da questão na prova.
        /// </summary>
        public int QuestionNumber { get; set; }

        /// <summary>
        /// Tipo de questão, Ex: Verstibular, Concursos, ENEM, etc.
        /// </summary>
        public string QuestionType { get; set; } 

        /// <summary>
        /// Principal área de conhecimento da questão.
        /// </summary>
        public required string MainArea { get; set; }

        /// <summary>
        /// Subáreas de conhecimento da questão.
        /// </summary>
        public string[] SubAreas { get; set; } = [];

        /// <summary>
        /// Corpo da questão.
        /// </summary>
        public List<ContentBlock> QuestionContents { get; set; } = [];

        /// <summary>
        /// Alternativas de resposta para a questão.
        /// </summary>
        public List<ChoiceV2>? Choices { get; set; }        
    }
}
