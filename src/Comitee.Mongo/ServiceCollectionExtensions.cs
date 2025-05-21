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
        string collectionName,
        MongoCollectionSettings? collectionSettings = null)
        where TDocument : DataModel<TDocumentId>
        where TDocumentId : notnull
    {
        services.TryAddScoped<UnitOfWork>();
        
        services.TryAddScoped<IUnitOfWork>(x => x.GetRequiredService<UnitOfWork>());
        
        services.TryAddScoped<IUnitOfWorkOperator>(x => x.GetRequiredService<UnitOfWork>());
        
        services.AddMongoCollection<TDocument>(collectionName, collectionSettings);
        
        services.TryAddScoped<IRepository<TDocumentId, TDocument>>(x =>
            new MongoRepository<TDocumentId, TDocument>(
                x.GetRequiredService<IUnitOfWorkOperator>(),
                x.GetRequiredService<IMongoCollection<TDocument>>()));

        return services;
    }
    
    private static IServiceCollection AddMongoCollection<TDocument>(
        this IServiceCollection services,
        string collectionName,
        MongoCollectionSettings? collectionSettings)
    {
        services.TryAddSingleton<IMongoCollection<TDocument>>(s =>
        {
            var mongoSettings = s.GetService<MongoSettings>();

            if (mongoSettings is null)
            {
                throw new ArgumentException("No mongodb settings have been specified. Ensure AddMongo has been called");
            }
            
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
        
            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);
            
            return mongoDatabase.GetCollection<TDocument>(collectionName, collectionSettings);
        });
        
        return services;
    }
}