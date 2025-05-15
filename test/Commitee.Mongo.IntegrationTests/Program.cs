using Comitee.Mongo;
using Commitee.Mongo.IntegrationTests.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetSection("Mongo").GetValue<string>("ConnectionString")!;

// Set up repository to test ServiceCollectionExtensions
builder.Services.AddMongoRepository<ObjectId, ObjectIdTestDocument>(mongoConnectionString, "test-database", "objectIdTestDocuments");

var app = builder.Build();
app.Run();