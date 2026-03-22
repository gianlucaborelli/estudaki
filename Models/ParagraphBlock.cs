using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.Models
{
    [BsonDiscriminator("ParagraphBlock")]
    public class ParagraphBlock : ContentBlock
    {
        public List<InlineContent> Inlines { get; set; } = [];
    }
}
