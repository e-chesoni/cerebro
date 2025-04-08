using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using MongoDbCrudPOC.Core.Models;
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

    private async void PopulatePageText()
    {
        var print = ViewModel.currentPrint;
        var slice = ViewModel.currentSlice;
        if (print != null)
        {
            if (!string.IsNullOrWhiteSpace(print.directoryPath))
            {
                if (slice != null)
                {
                    if (!string.IsNullOrWhiteSpace(slice.imagePath))
                    {
                        // ✅ All values are valid — update the UI
                        PrintNameTextBlock.Text = print.name;
                        CurrentSliceTextBox.Text = slice.fileName;
                        StatusTextBlock.Text = print?.complete == true ? "Complete" : "Incomplete";
                        SlicesMarkedTextBlock.Text = (await ViewModel.GetSlicesMarked()).ToString();
                        TotalSlicesTextBlock.Text = (await ViewModel.GetTotalSlices()).ToString();
                        // convert UTC to local time
                        var duration = print.duration;
                        var localStart = print.startTime.ToLocalTime();
                        var localEnd = print.endTime?.ToLocalTime();
                        Debug.WriteLine($"📅 start: {print.startTime}, end: {print.endTime}, duration: {print.duration}");
                        DurationTextBlock.Text = duration?.ToString(@"hh\:mm\:ss") ?? "—";
                    }
                    else
                    {
                        Debug.WriteLine("❌ Slice image path is null or empty.");
                        return;
                    }
                }
                else
                {
                    Debug.WriteLine("❌ Current slice is null.");
                    return;
                }
            }
            else
            {
                Debug.WriteLine("❌ Directory path is null or empty.");
                return;
            }
        }
        else 
        {
            Debug.WriteLine("❌ Current print is null.");
            return;
        }
    }

    private void ClearPageText()
    {
        directoryInput.Text = "";
        PrintNameTextBlock.Text = "";
        CurrentSliceTextBox.Text = "";
        StatusTextBlock.Text = "";
        DurationTextBlock.Text = "";
        SlicesMarkedTextBlock.Text = "";
        TotalSlicesTextBlock.Text = "";
        ViewModel.ClearData();
    }

    private async void GetSlices_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.AddPrintToDatabase(directoryInput.Text);
        if (ViewModel.currentSlice != null)
        {
            if (ViewModel.currentSlice.imagePath == null)
            {
                Debug.WriteLine("❌ImagePath is null.");
                return;
            }
            PopulatePageText();
        }
    }

    private async void MarkSliceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO: change current slice marked? to true
        await ViewModel.MarkSlice();
        // TODO: update current slice in display
        PopulatePageText();
    }

    private async void DeletePrintButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.currentPrint == null)
        {
            Debug.WriteLine("❌Current print is null");
            return;
        }
        else
        {
            Debug.WriteLine("✅Deleting print.");
            await ViewModel.DeleteCurrentPrint();
            Debug.WriteLine("✅Removing data from display.");
            ClearPageText();
        }
    }

    private async void BrowseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        folderPicker.FileTypeFilter.Add("*");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, hwnd);

        var folder = await folderPicker.PickSingleFolderAsync();
        // folder must contain .sjf files. if it does not contain any, error and return
        if (folder != null)
        {
            // Check for .sjf files in the selected folder
            var files = Directory.EnumerateFiles(folder.Path, "*.sjf");
            if (!files.Any())
            {
                Debug.WriteLine("❌ No .sjf files found in the selected folder.");
                ContentDialog dialog = new ContentDialog
                {
                    Title = "No Job Files in Folder",
                    Content = "The selected folder does not contain any .sjf files.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }
            directoryInput.Text = folder.Path;
            await ViewModel.AddPrintToDatabase(folder.Path);
        }
        PopulatePageText();
    }
}
