namespace ProvaOnline.ModelsV2
{
    public class ParagraphBlock : ContentBlock
    {
        public List<InlineContent> Inlines { get; set; } = [];
    }
}
