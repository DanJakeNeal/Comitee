using Comitee.UnitOfWork;
using MongoDB.Driver;

namespace Comitee.Mongo;

internal class UnitOfWork(IMongoDatabase mongoDatabase) : IUnitOfWork, IUnitOfWorkOperator
{
    private readonly IClientSessionHandle _mongoSession = mongoDatabase.Client.StartSession();
    
    public IDisposable Context => _mongoSession;
    private readonly List<Action> _operations = [];

    public void AddOperation(Action operation)
    {
        _operations.Add(operation);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        _mongoSession.StartTransaction();

        foreach (var operation in _operations)
        {
            operation.Invoke();
        }

        await _mongoSession.CommitTransactionAsync(cancellationToken);
        
        _operations.Clear();
    }
}