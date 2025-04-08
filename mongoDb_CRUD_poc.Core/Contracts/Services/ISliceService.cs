using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface ISliceService
{
    Task<IEnumerable<SliceModel>> GetAllSlices();

    public Task<long> CountMarkedOrUnmarked(string printId, bool marked);

    public Task<long> TotalSlices(string printId);

    public Task<bool> AllSlicesMarked(string printId);

    Task<SliceModel> GetSliceBySliceId(string sliceId);

    Task<IEnumerable<SliceModel>> GetSlicesByPrintId(string printId);

    public Task<SliceModel?> GetFirstSlice(PrintModel print);

    /// <summary>
    /// Return the first unmarked slice associated with a given print
    /// </summary>
    /// <param name="print"></param>
    /// <returns></returns>
    public Task<SliceModel?> GetNextSlice(PrintModel print);

    public Task<SliceModel?> GetLastSlice(PrintModel print);

    Task AddSlice(SliceModel slice);

    Task EditSlice(SliceModel slice);

    Task DeleteSlice(SliceModel slice);

    Task DeleteAllSlices();
}
