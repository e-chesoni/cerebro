using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using mongoDb_CRUD_poc.Core.Attributes;

namespace MongoDbCrudPOC.Core.Models;

[MongoCollection("prints")]
public class PrintModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public string directoryPath { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
    public TimeSpan printDuration => endTime - startTime;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> sliceIds { get; set; }
    public int totalSlices => sliceIds?.Count ?? 0;
    public int slicesMarked  { get; set; }
    public bool complete { get; set; }
}
    
