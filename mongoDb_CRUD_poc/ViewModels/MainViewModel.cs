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
    private readonly IPrintService _printService;
    private readonly ISliceService _sliceService;
    private readonly IPrintSeeder _seeder;
    public ObservableCollection<SliceModel> sliceCollection { get; } = new ObservableCollection<SliceModel>();
    public PrintModel? currentPrint = new();
    public SliceModel? currentSlice = new();
    private string _defaultPath = @"C:\Scanner Application\Scanner Software\jobfiles\test";

    public MainViewModel(IPrintSeeder seeder, IPrintService printService, ISliceService sliceService)
    {
        _printService = printService;
        _sliceService = sliceService;
        _seeder = seeder;
    }
    public async void GenerateNewPrint(string fullPath)
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
        SetPrint(fullPath); // calls update slices

        return;
    }
    public async void SetPrint(string directoryPath)
    {
        currentPrint = await _printService.GetPrintByDirectory(directoryPath);
        // update slices
        UpdateSlicesHelper();
    }

    private void UpdateSlicesHelper()
    {
        sliceCollection.Clear();
        GetSlices();
        SetCurrentSlice();
    }

    public async void SetPrintByID(string id)
    {
        Debug.WriteLine("✅Updating current print based on id.");
        currentPrint = await _printService.GetPrintById(id);
        // update slices
        UpdateSlicesHelper();
    }

    public async void SetPrintByDirectory(string fullPath)
    {
        Debug.WriteLine("✅Updating current print based on directory.");
        currentPrint = await _printService.GetPrintByDirectory(fullPath);
        // update slices
        UpdateSlicesHelper();
    }

    public async void GetSlices()
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
    private async void SetCurrentSlice()
    {
        var nextUnmarked = await _sliceService.GetFirstUnmarkedSlice(currentPrint);
        if (nextUnmarked == null)
        {
            // make current slice the last marked slice
            Debug.WriteLine("All slices have been marked. Setting current slice to last marked slice.");
            currentSlice = await _sliceService.GetLastMarkedSlice(currentPrint);
        }
        else
        {
            Debug.WriteLine("✅Setting current slice to next unmarked slice.");
            currentSlice = nextUnmarked;
        }
    }

    public async void OnNavigatedTo(object parameter)
    {
        sliceCollection.Clear();
        currentPrint = await _printService.GetFirstPrintAsync();
        UpdateSlicesHelper();
    }

    public void OnNavigatedFrom()
    {
    }
}
