using Comitee.Repository;
using MongoDB.Bson;

namespace Comitee.Mongo;

public class Document<TDataModelId>(TDataModelId id, DateTimeOffset createdAt) : DataModel<TDataModelId>(id, createdAt);
