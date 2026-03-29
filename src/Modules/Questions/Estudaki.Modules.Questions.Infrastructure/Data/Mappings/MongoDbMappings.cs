using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Mappings;

public static class MongoDbMappings
{
    private static bool _isRegistered = false;

    public static void RegisterMappings()
    {
        if (_isRegistered) return;

        BsonClassMap.RegisterClassMap<Question>(cm =>
        {
            cm.AutoMap();            
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<PublicNotice>(cm =>
        {
            cm.AutoMap();            
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<ContentBlock>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.AddKnownType(typeof(ParagraphBlock));
            cm.AddKnownType(typeof(ImageBlock));
        });

        BsonClassMap.RegisterClassMap<ParagraphBlock>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator("ParagraphBlock");
        });

        BsonClassMap.RegisterClassMap<ImageBlock>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator("ImageBlock");
        });

        BsonClassMap.RegisterClassMap<InlineContent>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
            cm.AddKnownType(typeof(TextInline));
            cm.AddKnownType(typeof(ImageInline));
        });

        BsonClassMap.RegisterClassMap<TextInline>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator("TextInline");
        });

        BsonClassMap.RegisterClassMap<ImageInline>(cm =>
        {
            cm.AutoMap();
            cm.SetDiscriminator("ImageInline");
        });

        BsonClassMap.RegisterClassMap<Choice>(cm =>
        {
            cm.AutoMap();
        });

        _isRegistered = true;
    }
}
