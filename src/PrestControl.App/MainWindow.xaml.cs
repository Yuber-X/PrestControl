using System.Windows;
using PrestControl.Services;
using PrestControl.ViewModels;

namespace PrestControl.App;

public partial class MainWindow : Window
{
    private readonly AuthService _auth;

    public MainWindow(MainViewModel vm, AuthService auth)
    {
        InitializeComponent();
        DataContext = vm;
        _auth = auth;
    }

    private async void BotonSalir_Click(object sender, RoutedEventArgs e)
    {
        await _auth.LogoutAsync();
        Application.Current.Shutdown();
    }
}
