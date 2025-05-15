namespace Comitee.UnitOfWork;

internal interface IUnitOfWorkOperator
{
    IDisposable Context { get; }
    void AddOperation(Action operation);
}