namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface IPrintSeeder
{
    Task CreatePrintFromDirectory(string directory);
}
