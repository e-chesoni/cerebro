using MongoDB.Driver;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Services;
public class SliceService : ISliceService
{
    private readonly IMongoCollection<SliceModel> _slices;
    public SliceService(IMongoDbService mongoDbService)
    {
        _slices = mongoDbService.GetCollection<SliceModel>();
    }

    public async Task<long> MarkedSlicesCount(string printId)
    {
        var filter = Builders<SliceModel>.Filter.And(
            Builders<SliceModel>.Filter.Eq(s => s.printId, printId),
            Builders<SliceModel>.Filter.Eq(s => s.marked, false)
        );
        return await _slices.CountDocumentsAsync(filter);
    }

    public async Task<bool> AllSlicesMarked(string printId)
    {
        return await MarkedSlicesCount(printId) == 0;
    }

    public async Task<SliceModel> GetSliceBySliceId(string id)
    {
        var filter = Builders<SliceModel>.Filter.Eq(s => s.id, id);
        return await _slices.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<SliceModel>> GetSlicesByPrintId(string printId)
    {
        var filter = Builders<SliceModel>.Filter.Eq(s => s.printId, printId);
        var slices = await _slices.Find(filter).SortBy(s => s.layer).ToListAsync();
        return slices;
    }
    public async Task<SliceModel?> GetFirstSlice(PrintModel print)
    {
        if (print == null || print.sliceIds == null || !print.sliceIds.Any())
            return null;

        var filter = Builders<SliceModel>.Filter.In(s => s.id, print.sliceIds);
        var slices = await _slices.Find(filter).ToListAsync();

        // Find slice with file name starting with "0000"
        var firstSlice = slices.FirstOrDefault(s =>
            Path.GetFileNameWithoutExtension(s.imagePath)?.StartsWith("0000") == true);

        return firstSlice;
    }

    public async Task<SliceModel?> GetNextSlice(PrintModel print)
    {
        if (print?.sliceIds == null || !print.sliceIds.Any())
            return null;

        var filter = Builders<SliceModel>.Filter.And(
            Builders<SliceModel>.Filter.In(s => s.id, print.sliceIds),
            Builders<SliceModel>.Filter.Eq(s => s.marked, false)
        );

        var slice = await _slices
            .Find(filter)
            .SortBy(s => s.layer)
            .FirstOrDefaultAsync();

        return slice;
    }

    public async Task<SliceModel?> GetLastSlice(PrintModel print)
    {
        if (print?.sliceIds == null || !print.sliceIds.Any())
            return null;

        var filter = Builders<SliceModel>.Filter.And(
            Builders<SliceModel>.Filter.In(s => s.id, print.sliceIds),
            Builders<SliceModel>.Filter.Eq(s => s.marked, true)
        );

        var slice = await _slices
            .Find(filter)
            .SortByDescending(s => s.layer)
            .FirstOrDefaultAsync();

        return slice;
    }


    public async Task AddSlice(SliceModel slice)
    {
        await _slices.InsertOneAsync(slice);
    }
    public async Task DeleteSlice(SliceModel slice)
    {
        var toDelete = Builders<SliceModel>.Filter.Eq(s => s.id, slice.id);
        await _slices.DeleteOneAsync(toDelete);
    }
    public async Task EditSlice(SliceModel slice)
    {
        var toEdit = Builders<SliceModel>.Filter.Eq(s => s.id, slice.id);
        await _slices.ReplaceOneAsync(toEdit, slice);
    }
    public async Task<IEnumerable<SliceModel>> GetAllSlices()
    {
        var results = await _slices.Find(_ => true).ToListAsync();
        return results.AsEnumerable();
    }
    public async Task DeleteAllSlices()
    {
        await _slices.DeleteManyAsync(_ => true);
    }

}
