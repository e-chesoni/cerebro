using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;

using MongoDbCrudPOC.ViewModels;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MongoDbCrudPOC.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private void GetSlices_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //ViewModel.SetPrintByID(directoryInput.Text);
        //ViewModel.SetPrintByDirectory(directoryInput.Text);
        ViewModel.GenerateNewPrint(directoryInput.Text);
        
        // TODO: current slice may be null here
        if (ViewModel.currentSlice != null)
        {
            if (ViewModel.currentSlice.imagePath == null)
            {
                Debug.WriteLine("❌ImagePath is null.");
                return;
            }
            PrintNameTextBlock.Text = ViewModel.currentPrint.directoryPath.ToString();
            CurrentSliceTextBox.Text = ViewModel.currentSlice.imagePath.ToString();
        }
    }

    private void MarkSliceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // proceed to next slice
    }

    private void DeletePrintButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private async void BrowseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        folderPicker.FileTypeFilter.Add("*");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, hwnd);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder != null)
        {
            directoryInput.Text = folder.Path;
            ViewModel.GenerateNewPrint(folder.Path);
        }
    }
}
