using MongoDB.Bson.Serialization.Attributes;

namespace ProvaOnline.Models;

[BsonDiscriminator("base")]
[BsonKnownTypes(typeof(TextSupport), typeof(ImageSupport))]
public abstract class QuestionSupport
{
}
