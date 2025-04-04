using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDbCrudPOC.Core.Models;

namespace mongoDb_CRUD_poc.Core.Contracts.Services;
public interface ISamplePrintDataService
{
    Task<IEnumerable<PrintModel>> GetGridDataAsync();
}
