using MongoDB.Bson.Serialization.Attributes;

namespace EstudaKi.Web.Models
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(TextInline), typeof(ImageInline))]
    public abstract class InlineContent
    {
        public string Type { get; set; } // "text", "image"
    }
}
