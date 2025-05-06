using Comitee.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Commitee.Mongo.IntegrationTests.Models;

public class ObjectIdTestDocument(ObjectId id) : Document<ObjectId>(id, DateTimeOffset.UtcNow)
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid TestProperty { get; private set; } = new();
}