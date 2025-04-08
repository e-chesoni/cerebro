using System.Collections.ObjectModel;
using System.Diagnostics;
using Amazon.Auth.AccessControlPolicy;
using CommunityToolkit.Mvvm.ComponentModel;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using mongoDb_CRUD_poc.Core.Services;
using MongoDbCrudPOC.Contracts.ViewModels;
using MongoDbCrudPOC.Core.Models;

namespace MongoDbCrudPOC.ViewModels;

public class MainViewModel : ObservableRecipient, INavigationAware
{
    #region Variables
    private readonly IPrintService _printService;
    private readonly ISliceService _sliceService;
    private readonly IPrintSeeder _seeder;
    public ObservableCollection<SliceModel> sliceCollection { get; } = new ObservableCollection<SliceModel>();
    public PrintModel? currentPrint = new();
    public SliceModel? currentSlice = new();
    private string _defaultPath = @"C:\Scanner Application\Scanner Software\jobfiles\test";
    #endregion

    #region Constructor
    public MainViewModel(IPrintSeeder seeder, IPrintService printService, ISliceService sliceService)
    {
        _printService = printService;
        _sliceService = sliceService;
        _seeder = seeder;
    }
    #endregion

    #region Setters
    public async Task SetPrint(string directoryPath)
    {
        currentPrint = await _printService.GetPrintByDirectory(directoryPath);
        await UpdateSlicesHelper(); // ✅ await this now
    }
    public async Task SetPrintByID(string id)
    {
        Debug.WriteLine("✅Updating current print based on id.");
        currentPrint = await _printService.GetPrintById(id);
        await UpdateSlicesHelper(); // ✅ now awaits properly
    }

    public async void SetPrintByDirectory(string fullPath)
    {
        Debug.WriteLine("✅Updating current print based on directory.");
        currentPrint = await _printService.GetPrintByDirectory(fullPath);
        await UpdateSlicesHelper();
    }
    #endregion

    private async Task CompleteCurrentPrintAsync()
    {
        var print = currentPrint;
        if (print == null)
        {
            Debug.WriteLine("❌Cannot update print; print is null.");
            return;
        }
        else
        {
            // update end time to now
            print.endTime = DateTime.UtcNow;
            // update print status to complete
            print.complete = true;
            // set current print to updated print
            currentPrint = print;
            // update print in db
            await _printService.EditPrint(print);
        }
    }

    public Task<TimeSpan?> GetPrintDuration()
    {
        if (currentPrint == null)
        {
            Debug.WriteLine("❌Cannot update print; print is null.");
            return Task.FromResult<TimeSpan?>(null);
        }
        else
        {
            Debug.WriteLine("✅Current print is set.");
            if (currentPrint.duration == null)
            {
                Debug.WriteLine("❌Duration is null.");
                return Task.FromResult<TimeSpan?>(null);
            }
            else
            {
                Debug.WriteLine("✅Returning current print duration.");
                return Task.FromResult(currentPrint.duration);
            }
        }
    }
    #region Getters

    private async Task<SliceModel?> GetNextSlice()
    {
        if (_sliceService == null)
        {
            Debug.WriteLine("❌Slice service is null.");
            return null;
        }

        if (currentPrint == null)
        {
            Debug.WriteLine("❌Current print is null.");
            return null;
        }

        var printComplete = await _printService.IsPrintComplete(currentPrint.id);

        if (printComplete)
        {
            Debug.WriteLine("✅Print is complete. Returning last marked slice.");
            // update print in db
            await CompleteCurrentPrintAsync();
            return await _sliceService.GetLastSlice(currentPrint);
        }
        else
        {
            Debug.WriteLine("➡️Print is not complete. Returning next unmarked slice.");
            return await _sliceService.GetNextSlice(currentPrint);
        }
    }
    public async Task<long> GetSlicesMarked()
    {
        return await _printService.MarkedOrUnmarkedCount(currentPrint.id);
    }
    public async Task<long> GetTotalSlices()
    {
        return await _printService.TotalSlicesCount(currentPrint.id);
    }

    public async Task<IEnumerable<SliceModel>> GetSlices()
    {
        if (currentPrint == null)
        {
            Debug.WriteLine("❌Current print is null.");
            return Enumerable.Empty<SliceModel>();
        }
        return await _printService.GetSlicesByPrintId(currentPrint.id);
    }
    #endregion

    #region Helpers
    private async Task UpdateSlicesHelper()
    {
        sliceCollection.Clear();
        await LoadSliceData();
        currentSlice = await GetNextSlice();
    }
    #endregion

    #region Data Management
    public async Task LoadSliceData()
    {
        sliceCollection.Clear();
        try
        {
            // check for null values
            if (currentPrint == null)
            {
                Debug.WriteLine("❌ No print found in DB.");
                return;
            }

            if (string.IsNullOrEmpty(currentPrint.id))
            {
                Debug.WriteLine("❌ Print ID is null or empty.");
                return;
            }

            Debug.WriteLine("✅Getting slices.");
            // use print service to get slices
            var slices = await _printService.GetSlicesByPrintId(currentPrint.id);
            foreach (var s in slices)
            {
                Debug.WriteLine($"Adding slice: {s.imagePath}");
                sliceCollection.Add(s);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
    }
    public async Task AddPrintToDatabase(string fullPath)
    {
        // check if print already exists in db
        var existingPrint = await _printService.GetPrintByDirectory(fullPath);

        if (existingPrint != null)
        {
            Debug.WriteLine($"❌Print with this file path {fullPath} already exists in the database. Canceling new print.");
        }
        else
        {
            // seed prints
            await _seeder.CreatePrintFromDirectory(fullPath);
        }

        // set print on view model
        await SetPrint(fullPath); // calls update slices

        return;
    }
    public void ClearData()
    {
        sliceCollection.Clear();
        currentPrint = null;
        currentSlice = null;
    }
    #endregion

    #region Access CRUD Methods
    public async Task DeleteCurrentPrint()
    {
        await _printService.DeletePrint(currentPrint); // deletes slices associated with print
    }
    #endregion

    #region Print Methods
    public async Task MarkSlice()
    {
        if (currentSlice == null)
        {
            Debug.WriteLine("❌Current slice is null.");
            return;
        }

        if (currentSlice.marked)
        {
            Debug.WriteLine("❌Slice already marked. Canceling operation");
            return;
        }

        Debug.WriteLine($"✅ Marking slice {currentSlice.fileName}.");

        currentSlice.marked = true;
        await _sliceService.EditSlice(currentSlice);

        await UpdateSlicesHelper(); // this should update currentSlice
    }

    #endregion

    #region Navigation
    public async void OnNavigatedTo(object parameter)
    {
        sliceCollection.Clear();
        //currentPrint = await _printService.GetFirstPrintAsync();
        //UpdateSlicesHelper();
    }

    public void OnNavigatedFrom()
    {
    }
    #endregion
}
