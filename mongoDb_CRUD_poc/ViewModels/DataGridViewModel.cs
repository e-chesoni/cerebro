using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using mongoDb_CRUD_poc.ViewModels.DisplayModels;
using MongoDbCrudPOC.Contracts.ViewModels;

namespace MongoDbCrudPOC.ViewModels;

public class DataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly IPrintService _printService;
    public ObservableCollection<PrintDisplayModel> printCollection { get; } = new ObservableCollection<PrintDisplayModel>();

    public DataGridViewModel(IPrintService printService)
    {
        _printService = printService;
    }
    public async void OnNavigatedTo(object parameter)
    {
        printCollection.Clear();

        try
        {
            var prints = await _printService.GetAllPrints();

            var displayModels = prints.Select(p => new PrintDisplayModel
            {
                id = p.id,
                directoryPath = p.directoryPath,
                duration = p.duration?.ToString(@"hh\:mm\:ss") ?? "",
                startTimeLocal = p.startTime.ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss"),
                endTimeLocal = p.endTime?.ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss") ?? "",
                totalSlices = p.totalSlices,
                complete = p.complete,
            }).ToList();

            foreach (var item in displayModels)
            {
                printCollection.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
