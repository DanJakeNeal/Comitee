using System.Linq.Expressions;

namespace Comitee.Repository;

public interface IRepository<in TDataModelId, TDataModel>
    where TDataModel : DataModel<TDataModelId>
    where TDataModelId : notnull
{
    Task<TDataModel?> FindAsync(TDataModelId id, CancellationToken cancellationToken = default);
    Task<TDataModel?> FindAsync(Expression<Func<TDataModel, bool>> expression, CancellationToken cancellationToken = default);
    Task<ICollection<TDataModel>> FindManyAsync(Expression<Func<TDataModel, bool>> expression, CancellationToken cancellationToken = default);
    Task<PagedResult<TDataModel>> FindByPageAsync(int page, int pageSize, CancellationToken cancellationToken);
    
    void Add(TDataModel dataModel);
    void Update(TDataModel dataModel);
    void Remove(TDataModelId id);
}