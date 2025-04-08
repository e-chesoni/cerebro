using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface IPrintService
{
    #region Counters
    Task<long> TotalPrintsCount();
    public Task<long> TotalSlicesCount(string printId);
    public Task<long> MarkedOrUnmarkedCount(string printId);
    #endregion
    
    #region Getters
    public Task<PrintModel> GetFirstPrintAsync();
    Task<PrintModel> GetPrintByDirectory(string DirectoryPath);
    Task<PrintModel> GetPrintById(string id);
    Task<IEnumerable<SliceModel>> GetSlicesByPrintId(string id);
    Task<IEnumerable<PrintModel>> GetAllPrints();
    #endregion

    #region Checkers
    public Task<bool> IsPrintComplete(string printId);
    #endregion

    #region CRUD
    Task AddPrint(PrintModel print);
    Task EditPrint(PrintModel print);
    Task DeletePrint(PrintModel print);
    Task DeleteAllPrints();
    #endregion
}
