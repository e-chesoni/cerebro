using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Models;

namespace MongoDbCrudPOC.Core.Services;
public class SamplePrintDataService : ISamplePrintDataService
{
    private List<PrintModel> _allPrints;
    public SamplePrintDataService()
    {
    }

    public static IEnumerable<PrintModel> GenerateSamplePrints()
    {
        var random = new Random();

        return Enumerable.Range(1, 20).Select(i =>
        {
            var startTime = DateTime.UtcNow.AddDays(-random.Next(1, 30));
            var endTime = startTime.AddHours(2); // Ensure it's 2 hours long

            return new PrintModel
            {
                id = ObjectId.GenerateNewId().ToString(),
                directoryPath = $"C:\\prints\\print{i}",
                startTime = startTime,
                endTime = endTime,
                sliceIds = Enumerable.Range(1, random.Next(100, 200))
                                      .Select(_ => ObjectId.GenerateNewId().ToString())
                                      .ToList()
            };
        });
    }

    public async Task<IEnumerable<PrintModel>> GetGridDataAsync()
    {
        if (_allPrints == null)
        {
            _allPrints = new List<PrintModel>(GenerateSamplePrints());
        }

        await Task.CompletedTask;
        return _allPrints.AsEnumerable();
    }

}
