using Comitee.Hosting;
using Comitee.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace Comitee.Mongo.Hosting;

public static class ComiteeBuilderExtensions
{
    public static void AddMongo(this IComiteeBuilder builder, MongoSettings settings)
    {
        var mongoClient = new MongoClient(settings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
        
        builder.Services.TryAddKeyedSingleton(settings.DatabaseName, mongoDatabase);
        
        builder.Services.TryAddScoped<UnitOfWork>();
        
        builder.Services.TryAddScoped<IUnitOfWork>(x => x.GetRequiredService<UnitOfWork>());
        
        builder.Services.TryAddScoped<IUnitOfWorkOperator>(x => x.GetRequiredService<UnitOfWork>());
        
        builder.Services.TryAddSingleton(settings);
    }
}
