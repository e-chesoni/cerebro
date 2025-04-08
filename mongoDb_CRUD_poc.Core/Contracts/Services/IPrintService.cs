using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface IPrintService
{
    public Task<PrintModel> GetFirstPrintAsync();
    
    Task<long> TotalPrints();

    public Task<bool> IsPrintComplete(string printId);

    Task<IEnumerable<PrintModel>> GetAllPrints();

    Task<PrintModel> GetPrintById(string id);

    Task<PrintModel> GetPrintByDirectory(string DirectoryPath);

    Task<IEnumerable<SliceModel>> GetSlicesByPrintId(string id);

    Task AddPrint(PrintModel print);

    Task EditPrint(PrintModel print);

    Task DeletePrint(PrintModel print);

    Task DeleteAllPrints();
}
