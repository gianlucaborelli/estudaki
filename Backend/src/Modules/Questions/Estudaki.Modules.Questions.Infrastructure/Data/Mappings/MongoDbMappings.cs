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

        BsonClassMap.RegisterClassMap<Choice>(cm =>
        {
            cm.AutoMap();
        });

        _isRegistered = true;
    }
}
