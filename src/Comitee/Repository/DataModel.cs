namespace Comitee.Repository;

public abstract class DataModel<TDataModelId>(TDataModelId id)
{
    public TDataModelId Id { get; private set; } = id;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; protected internal set; } = DateTimeOffset.Now;
    public long Version { get; protected internal set; }
}