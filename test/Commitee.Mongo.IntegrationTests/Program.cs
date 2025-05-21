using Comitee.Hosting;
using Comitee.Mongo;
using Comitee.Mongo.Hosting;
using Commitee.Mongo.IntegrationTests.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetSection("Mongo").GetValue<string>("ConnectionString")!;

// Set up repository to test ServiceCollectionExtensions
builder.Services.AddComitee().AddMongo(new MongoSettings(mongoConnectionString, "test-database"));

builder.Services.AddMongoRepository<ObjectId, ObjectIdTestDocument>("objectIdTestDocuments");

var app = builder.Build();
app.Run();