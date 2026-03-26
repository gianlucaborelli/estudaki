using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    [BsonDiscriminator("ParagraphBlock")]
    public class ParagraphBlock : ContentBlock
    {
        public List<InlineContent> Inlines { get; set; } = [];
    }
}
