using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DiscoverCostaRica.Functions.Models;

public class LogModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("message")]
    public required string Message { get; set; }

    [BsonElement("category")]
    public required string Category { get; set; }

    [BsonElement("exception")]
    public required string Exception { get; set; }

    [BsonElement("stacktrace")]
    public required string Stacktrace { get; set; }
}
