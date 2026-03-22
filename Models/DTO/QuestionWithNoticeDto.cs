namespace ProvaOnline.Models.DTO
{
    /// <summary>
    /// DTO que combina uma Question com seu PublicNotice para facilitar a renderização no front-end
    /// </summary>
    public class QuestionWithNoticeDto
    {
        public QuestionDocument Question { get; set; } = null!;
        public PublicNoticeDocument? PublicNotice { get; set; }
    }
}
