namespace Comitee.Repository;

public abstract class DataModel<TDataModelId>(TDataModelId id, DateTimeOffset createdAt)
{
    public TDataModelId Id { get; private set; } = id;
    public DateTimeOffset CreatedAt { get; private set; } = createdAt;
    public long Version { get; protected internal set; }
}