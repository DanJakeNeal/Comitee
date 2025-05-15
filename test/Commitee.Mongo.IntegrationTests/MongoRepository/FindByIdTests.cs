using Commitee.Mongo.IntegrationTests.Models;
using FluentAssertions;
using MongoDB.Bson;
using NUnit.Framework;

namespace Commitee.Mongo.IntegrationTests.MongoRepository;

[TestFixture]
public class FindByIdTests
{
    [Test]
    [Description("Given a document exists in Mongo" +
                 "And the documents ID is an ObjectId" +
                 "When FindAsync is called by ID" +
                 "Then the document is returned")]
    public async Task ObjectIdDocumentFound()
    {
        var existingDocument = new ObjectIdTestDocument(id: ObjectId.GenerateNewId());

        await TestServer.MongoCollection<ObjectIdTestDocument>().InsertOneAsync(existingDocument);

        var sut = TestServer.MongoRepository<ObjectId, ObjectIdTestDocument>();

        var result = await sut.FindAsync(existingDocument.Id);

        result.Should().BeEquivalentTo(existingDocument);
    }
}