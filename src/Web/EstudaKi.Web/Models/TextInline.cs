using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    [BsonDiscriminator("TextInline")]
    public class TextInline : InlineContent
    {
        public string Text { get; set; }

        public bool Bold { get; set; }
        public bool Italic { get; set; }
    }
}
