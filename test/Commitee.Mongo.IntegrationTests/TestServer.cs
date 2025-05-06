using Comitee.Mongo;
using Comitee.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using Testcontainers.MongoDb;

namespace Commitee.Mongo.IntegrationTests;

[SetUpFixture]
public class TestServer
{
    private const string EnvironmentName = "IntegrationTests";
    private static WebApplicationFactory<Program> _application = null!;
    public static MongoDbContainer MongoDbContainer = null!;
    
    public static IMongoCollection<TDocument> MongoCollection<TDocument>() =>
        _application.Services.GetRequiredService<IMongoCollection<TDocument>>();

    public static IRepository<TDocumentId, TDocument> MongoRepository<TDocumentId, TDocument>()
        where TDocument : DataModel<TDocumentId>
        where TDocumentId : notnull
        => _application.Services.GetRequiredService<IRepository<TDocumentId, TDocument>>();
    
    [OneTimeSetUp]
    public async Task Setup()
    {
        MongoDbContainer = new MongoDbBuilder()
            .WithReplicaSet()
            .Build();
        
        await MongoDbContainer.StartAsync();
        Environment.SetEnvironmentVariable("Mongo__ConnectionString", MongoDbContainer.GetConnectionString());
        
        _application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(EnvironmentName);
                builder.UseContentRoot(Directory.GetCurrentDirectory());
            });

        _application.Server.PreserveExecutionContext = true;
    }
    
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await MongoDbContainer.DisposeAsync();
        await _application.DisposeAsync();
    }
}