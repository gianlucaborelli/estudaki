using System.Text.Json.Serialization;

namespace Estudaki.Modules.Questions.Domain.ValueObjects;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ParagraphBlock), "paragraph")]
[JsonDerivedType(typeof(ImageBlock), "image")]
public abstract class ContentBlock
{
    public int Order { get; set; }
}
