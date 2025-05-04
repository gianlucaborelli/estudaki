namespace ProvaOnline.Helpers
{
    public class SearchFilter
    {
        public bool IsPublished { get; set; } = true;
        public string? MainArea { get; set; }
        public string? SubArea { get; set; }
        public string? Keyword { get; set; }
        public string? ExamBoardName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
