using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using MongoDbCrudPOC.Contracts.ViewModels;
using MongoDbCrudPOC.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Models;

namespace MongoDbCrudPOC.ViewModels;

public class DataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly IPrintService _printService;
    public ObservableCollection<PrintModel> printCollection { get; } = new ObservableCollection<PrintModel>();

    public DataGridViewModel(IPrintService printService)
    {
        _printService = printService;
    }
    public async void OnNavigatedTo(object parameter)
    {
        printCollection.Clear();

        try
        {
            var data = await _printService.GetAllPrints();
            foreach (var item in data)
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
