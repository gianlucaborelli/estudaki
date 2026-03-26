using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(ParagraphBlock), typeof(ImageBlock))]
    public abstract class ContentBlock
    {
        public int Order { get; set; }
    }
}
