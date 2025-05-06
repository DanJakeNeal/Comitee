using Comitee.Repository;
using Comitee.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Comitee.Mongo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoRepository<TDocumentId, TDocument>(
        this IServiceCollection services, 
        string connectionString,
        string databaseName,
        string collectionName,
        MongoCollectionSettings? collectionSettings = null)
        where TDocument : DataModel<TDocumentId>
        where TDocumentId : notnull
    {
        services.TryAddScoped<UnitOfWork>();
        
        services.TryAddScoped<IUnitOfWork>(x => x.GetRequiredService<UnitOfWork>());
        
        services.TryAddScoped<IUnitOfWorkOperator>(x => x.GetRequiredService<UnitOfWork>());
        
        services.AddMongoCollection<TDocument>(connectionString, databaseName, collectionName, collectionSettings);
        
        services.TryAddScoped<IRepository<TDocumentId, TDocument>>(x =>
            new MongoRepository<TDocumentId, TDocument>(
                x.GetRequiredService<IUnitOfWorkOperator>(),
                x.GetRequiredService<IMongoCollection<TDocument>>()));

        return services;
    }
    
    private static IServiceCollection AddMongoCollection<TDocument>(
        this IServiceCollection services,
        string connectionString,
        string databaseName,
        string collectionName,
        MongoCollectionSettings? collectionSettings)
    {
        var mongoClient = new MongoClient(connectionString);
        
        var mongoDatabase = mongoClient.GetDatabase(databaseName);
        
        services.TryAddSingleton(mongoDatabase);
        services.TryAddSingleton<IMongoCollection<TDocument>>(_ => mongoDatabase.GetCollection<TDocument>(collectionName, collectionSettings));
        
        return services;
    }
}