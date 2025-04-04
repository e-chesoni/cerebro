using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface ISliceService
{
    Task<IEnumerable<SliceModel>> GetAllSlices();

    Task<SliceModel> GetSliceBySliceId(string sliceId);

    Task<IEnumerable<SliceModel>> GetSlicesByPrintId(string printId);

    public Task<SliceModel?> GetFirstSliceForPrint(PrintModel print);

    public Task<SliceModel?> GetFirstUnmarkedSlice(PrintModel print);

    public Task<SliceModel?> GetLastMarkedSlice(PrintModel print);

    Task AddSlice(SliceModel slice);

    Task EditSlice(SliceModel slice);

    Task DeleteSlice(SliceModel slice);

    Task DeleteAllSlices();
}
