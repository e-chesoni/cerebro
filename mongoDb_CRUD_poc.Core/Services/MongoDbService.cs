using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using mongoDb_CRUD_poc.Core.Attributes;
using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Services;
public class MongoDbService : IMongoDbService
{
    private readonly IMongoClient _client;
    private IMongoDatabase _database;

    public MongoDbService(IMongoClient client)
    {
        _client = client;
        _database = _client.GetDatabase("magnetoDb");
    }

    public void SetDatabase(string db)
    {
        _database = _client.GetDatabase(db);
    }

    public IMongoDatabase GetDatabase()
    {
        return _database;
    }

    public async Task<List<string>> GetCollectionNames()
    {
        var collectionCursor = await _database.ListCollectionNamesAsync();
        return await collectionCursor.ToListAsync();
    }

    /// <summary>
    /// Use a model to collection like this:
    ///     var printCollection = mongoDbService.GetCollection<PrintModel>();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Collection</returns>
    public IMongoCollection<T> GetCollection<T>()
    {
        var name = typeof(T)
            .GetCustomAttributes(typeof(MongoCollectionAttribute), true)
            .Cast<MongoCollectionAttribute>()
            .FirstOrDefault()?.Name ?? typeof(T).Name;

        return _database.GetCollection<T>(name);
    }
}

