namespace ProvaOnline.Models
{
    public class PublicNotice
    {
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
        public string ExamBoard { get; set; }

        /// <summary>
        /// Nome do cargo ou função.
        /// </summary>
        public string Position { get; set; }
    }
}
