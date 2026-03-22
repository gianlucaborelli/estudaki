using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.Models;

[Obsolete("This class is deprecated.")]
[BsonDiscriminator("base")]
[BsonKnownTypes(typeof(TextSupport), typeof(ImageSupport))]
public abstract class QuestionSupport
{
}
