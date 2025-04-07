using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Models;
using MongoDbCrudPOC.Core.Services;

namespace mongoDb_CRUD_poc.Core.Seeders;
public class PrintSeeder : IPrintSeeder
{
    private readonly IFileService _fileService;
    private readonly IPrintService _printService;
    private readonly ISliceService _sliceService;
    public PrintSeeder(IFileService fileService, IPrintService printService, ISliceService sliceService)
    {
        _fileService = fileService;
        _printService = printService;
        _sliceService = sliceService;
    }
    public async Task CreatePrintFromDirectory(string directoryPath)
    {
        var files = _fileService.GetSliceFilesFromDirectoryPath(directoryPath).ToList();
        if (!files.Any()) return;

        var printId = ObjectId.GenerateNewId().ToString(); // works because declared Bson on print model
        var sliceIds = new List<string>();

        for (var i = 0; i < files.Count(); i++)
        {
            // gen slice id
            var sliceId = ObjectId.GenerateNewId().ToString();

            // add sliceId to sliceId list
            sliceIds.Add(sliceId);

            // get full path to job file
            var fullPath = files[i];

            // extract file name
            var fileName = Path.GetFileName(fullPath);

            // create a slice model
            var slice = new SliceModel
            {
                id = sliceId,
                printId = printId,
                layer = i,
                imagePath = fullPath,
                fileName = fileName,
                marked = false,
            };
            // add slices to slice collection using slice service
            await _sliceService.AddSlice(slice);
        }
        
        // get print directory
        var printName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar));

        // create a print model and add slice ids to it
        var print = new PrintModel
        {
            id = printId,
            name = printName,
            directoryPath = directoryPath, // TODO: You need to get the full path
            startTime = DateTime.UtcNow,
            sliceIds = sliceIds,
            slicesMarked = 0,
        };
        await _printService.AddPrint(print);
    }
}
