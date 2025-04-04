using MongoDB.Driver;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface IMongoDbService
{
    void SetDatabase(string db);
    IMongoDatabase GetDatabase();
    IMongoCollection<T> GetCollection<T>();
}
