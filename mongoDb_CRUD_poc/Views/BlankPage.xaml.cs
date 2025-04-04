using Microsoft.UI.Xaml.Controls;

using MongoDbCrudPOC.ViewModels;

namespace MongoDbCrudPOC.Views;

public sealed partial class BlankPage : Page
{
    public BlankViewModel ViewModel
    {
        get;
    }

    public BlankPage()
    {
        ViewModel = App.GetService<BlankViewModel>();
        InitializeComponent();
    }
}
