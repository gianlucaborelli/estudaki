using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    public class PublicNoticeDocument
    {
        /// <summary>
        /// Seleciona ou atribui um ID único para o documento.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// Número do Edital.
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// Ano do Edital.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Indica qual é a fase do exame.
        /// </summary>
        public string? ExamPhase { get; set; }

        /// <summary>
        /// Nome da Banca Examinadora.
        /// </summary>
        public string? ExamBoard { get; set; }

        /// <summary>
        /// Nome do cargo ou função.
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// Link para o arquivo PDF do caderno de questões do exame.
        /// </summary>
        public string? ExamBookletURL { get; set; }

        /// <summary>
        /// Link para o arquivo PDF do gabarito do exame.
        /// </summary>
        public string? ExamAnswerKeyURL { get; set; }

        /// <summary>
        /// Data de criação do documento.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
