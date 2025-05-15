using System.Linq.Expressions;
using Comitee.Exceptions;
using Comitee.Repository;
using Comitee.UnitOfWork;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Comitee.Mongo;

internal class MongoRepository<TDataModelId, TDataModel>(IUnitOfWorkOperator unitOfWorkOperator,
    IMongoCollection<TDataModel> collection) : IRepository<TDataModelId, TDataModel>
    where TDataModel : DataModel<TDataModelId>
    where TDataModelId : notnull
{
    private readonly Dictionary<TDataModelId, TDataModel> _trackedDataModels = [];
    
    public Task<TDataModel?> FindAsync(TDataModelId id, CancellationToken cancellationToken)
    {
        return FindAsync(d => Equals(d.Id, id), cancellationToken);
    }
    
    public async Task<TDataModel?> FindAsync(Expression<Func<TDataModel, bool>> expression, CancellationToken cancellationToken)
    {
        var dataModel = _trackedDataModels.Values.SingleOrDefault(expression.Compile());
        
        if (dataModel is not null)
        {
            return dataModel;
        }
        
        dataModel = (await collection.FindAsync(expression, cancellationToken: cancellationToken))
            .SingleOrDefault(cancellationToken: cancellationToken);
            
        if (dataModel is not null)
        {
            _trackedDataModels.Add(dataModel.Id, dataModel);
        }

        return dataModel;
    }
    
    public async Task<ICollection<TDataModel>> FindManyAsync(Expression<Func<TDataModel, bool>> expression, CancellationToken cancellationToken)
    {
        var dataModels = _trackedDataModels.Values.Where(expression.Compile()).ToList() ?? [];
        
        if (dataModels.Any())
        {
            return dataModels;
        }
        
        dataModels = (await collection.FindAsync(expression, cancellationToken: cancellationToken))
            .ToList(cancellationToken: cancellationToken) ?? [];
            
        if (dataModels.Any())
        {
            foreach (var dataModel in dataModels)
            {
                _trackedDataModels.Add(dataModel.Id, dataModel);
            }
        }

        return dataModels;
    }

     public async Task<PagedResult<TDataModel>> FindByPageAsync(int page, int pageSize, CancellationToken cancellationToken)
     {
         var countFacet = AggregateFacet.Create("count",
             PipelineDefinition<TDataModel, AggregateCountResult>.Create(new[]
             {
                 PipelineStageDefinitionBuilder.Count<TDataModel>()
             }));

         var dataFacet = AggregateFacet.Create("data",
             PipelineDefinition<TDataModel, TDataModel>.Create(new[]
             {
                 PipelineStageDefinitionBuilder.Sort(Builders<TDataModel>.Sort.Ascending(x => x.CreatedAt)),
                 PipelineStageDefinitionBuilder.Skip<TDataModel>((page - 1) * pageSize),
                 PipelineStageDefinitionBuilder.Limit<TDataModel>(pageSize),
             }));

         var filter = Builders<TDataModel>.Filter.Empty;
         var aggregation = await collection.Aggregate()
             .Match(filter)
             .Facet(countFacet, dataFacet)
             .ToListAsync(cancellationToken: cancellationToken);

         var dataModelCount = aggregation.First()
             .Facets.First(x => x.Name == countFacet.Name)
             .Output<AggregateCountResult>()
             ?.FirstOrDefault()
             ?.Count ?? 0;

         var totalPages = (int)Math.Ceiling((double)dataModelCount / pageSize);

         var data = aggregation.First()
             .Facets.First(x => x.Name == dataFacet.Name)
             .Output<TDataModel>();

         return new PagedResult<TDataModel>(
             TotalPages: totalPages,
             TotalDataCount: dataModelCount,
             Data: data);
     }

    public void Add(TDataModel dataModel) =>
        unitOfWorkOperator.AddOperation(() =>
            collection.InsertOne(
                unitOfWorkOperator.Context as IClientSessionHandle,
                dataModel)
        );

    public void Update(TDataModel dataModel) =>
        unitOfWorkOperator.AddOperation(() =>
        {
            var currentVersion = dataModel.Version;
            dataModel.Version += 1;
            
            var result = collection.ReplaceOne(
                unitOfWorkOperator.Context as IClientSessionHandle,
                d => Equals(d.Id, dataModel.Id) && d.Version == currentVersion, dataModel);

            if (result.IsAcknowledged && result.ModifiedCount == 0)
            {
                throw new ConcurrencyException("The document has been modified out of process");
            }
        });

    public void Remove(TDataModelId id) => 
        unitOfWorkOperator.AddOperation(() =>
            collection.DeleteOne(
                unitOfWorkOperator.Context as IClientSessionHandle,
                d => Equals(d.Id, id)));
}