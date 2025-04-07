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

    private void UpdatePageText()
    {
        if (ViewModel.currentPrint == null)
        {
            Debug.WriteLine("❌ Current print is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ViewModel.currentPrint.directoryPath))
        {
            Debug.WriteLine("❌ Directory path is null or empty.");
            return;
        }

        if (ViewModel.currentSlice == null)
        {
            Debug.WriteLine("❌ Current slice is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ViewModel.currentSlice.imagePath))
        {
            Debug.WriteLine("❌ Slice image path is null or empty."); // image path is null or empty
            return;
        }

        // ✅ All values are valid — update the UI
        PrintNameTextBlock.Text = ViewModel.currentPrint.name;
        CurrentSliceTextBox.Text = ViewModel.currentSlice.fileName;
    }


    private void GetSlices_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.GenerateNewPrint(directoryInput.Text);
        if (ViewModel.currentSlice != null)
        {
            if (ViewModel.currentSlice.imagePath == null)
            {
                Debug.WriteLine("❌ImagePath is null.");
                return;
            }
            UpdatePageText();
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
            await ViewModel.GenerateNewPrint(folder.Path);
        }
        UpdatePageText();
    }
}
