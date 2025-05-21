using Comitee.Repository;
using MongoDB.Bson;

namespace Comitee.Mongo;

public class Document<TDataModelId>(TDataModelId id) : DataModel<TDataModelId>(id);
