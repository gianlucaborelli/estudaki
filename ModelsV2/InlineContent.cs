using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.ModelsV2
{
    [BsonDiscriminator("inline")]
    [BsonKnownTypes(typeof(TextInline), typeof(ImageInline))]
    public abstract class InlineContent
    {
        public string Type { get; set; } // "text", "image"
    }
}
