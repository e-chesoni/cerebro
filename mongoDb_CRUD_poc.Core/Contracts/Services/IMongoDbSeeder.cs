using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface IMongoDbSeeder
{
    Task SeedDatabaseAsync();
    Task ClearDatabaseAsync(bool confirm);
}
