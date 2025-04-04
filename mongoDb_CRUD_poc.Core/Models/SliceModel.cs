using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using mongoDb_CRUD_poc.Core.Attributes;

namespace MongoDbCrudPOC.Core.Models;

[MongoCollection("slices")]
public class SliceModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string printId { get; set; }  // for reverse lookup

    public string imagePath { get; set; }
    public int layer { get; set; } // order of slice
    public double layerThickness { get; set; }
    public double power { get; set; }
    public double scanSpeed { get; set; }
    public double hatchSpacing { get; set; }
    public double beamArea { get; set; }
    public double energyDensity { get; set; }
    public bool marked { get; set; }
    public DateTime? markedAt { get; set; }
}
