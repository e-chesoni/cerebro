using System.Diagnostics;
using MongoDB.Driver;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Services;
public class PrintService : IPrintService
{
    private readonly IMongoCollection<PrintModel> _prints;

    private readonly ISliceService _sliceService;

    public PrintService(IMongoDbService mongoDbService, ISliceService sliceService)
    {
        _prints = mongoDbService.GetCollection<PrintModel>();
        _sliceService = sliceService;
    }
    public async Task<PrintModel> GetFirstPrintAsync()
    {
        return await _prints
            .Find(_ => true)
            .SortByDescending(p => p.startTime)
            .FirstOrDefaultAsync();
    }
    public async Task<PrintModel> GetPrintById(string id)
    {
        var filter = Builders<PrintModel>.Filter.Eq(p => p.id, id);
        return await _prints.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<PrintModel?> GetPrintByDirectory(string directoryPath)
    {
        var normalizedPath = Path.GetFullPath(directoryPath.Trim()).TrimEnd(Path.DirectorySeparatorChar);

        var filter = Builders<PrintModel>.Filter.Eq(p => p.directoryPath, normalizedPath);
        var print = await _prints.Find(filter).FirstOrDefaultAsync();

        Debug.WriteLine(print != null
            ? $"✅ Found print. Path: {print.directoryPath}"
            : $"❌ No print found for path: {normalizedPath}");

        return print;
    }

    public async Task<IEnumerable<SliceModel>> GetSlicesByPrintId(string printId)
    {
        if (string.IsNullOrEmpty(printId))
            return Enumerable.Empty<SliceModel>();

        return await _sliceService.GetSlicesByPrintId(printId);
    }

    /// <summary>
    /// Gets the total number of prints in the print collection
    /// </summary>
    /// <returns>long total prints in database</returns>
    public async Task<long> TotalPrints()
    {
        return await _prints.CountDocumentsAsync(_ => true);
    }

    public async Task<bool> IsPrintComplete(string printId)
    {
        return await _sliceService.AllSlicesMarked(printId);
    }

    public async Task<long> TotalSlices(string printId)
    {
        return await _sliceService.TotalSlices(printId);
    }
    public async Task<long> SlicesMarked(string printId)
    {
        return await _sliceService.CountMarkedOrUnmarked(printId, true);
    }

    public async Task AddPrint(PrintModel print)
    {
        await _prints.InsertOneAsync(print);
    }
    public async Task DeletePrint(PrintModel print)
    {
        var toDelete = Builders<PrintModel>.Filter.Eq(p => p.id, print.id);
        var slices = await GetSlicesByPrintId(print.id);
        // get print slices
        foreach (var s in slices)
        {
            // delete print slices
            await _sliceService.DeleteSlice(s);
        }
        // delete print
        await _prints.DeleteOneAsync(toDelete);
    }
    public async Task EditPrint(PrintModel print)
    {
        var toEdit = Builders<PrintModel>.Filter.Eq(p => p.id, print.id);
        await _prints.ReplaceOneAsync(toEdit, print);
    }
    public async Task<IEnumerable<PrintModel>> GetAllPrints()
    {
        var results = await _prints.Find(_ => true).ToListAsync();
        return results.AsEnumerable();
    }
    public async Task DeleteAllPrints()
    {
        await _prints.DeleteManyAsync(_ => true);
    }

}
