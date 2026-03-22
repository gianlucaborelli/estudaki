using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.Models
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(ParagraphBlock), typeof(ImageBlock))]
    public abstract class ContentBlock
    {
        public int Order { get; set; }
    }
}
