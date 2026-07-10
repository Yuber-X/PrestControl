using System.Windows;
using System.Windows.Controls;
using PrestControl.Models;
using PrestControl.ViewModels;

namespace PrestControl.Views;

public partial class CobrosView : UserControl
{
    private CobrosViewModel? _vm;

    public CobrosView() => InitializeComponent();

    // Lógica de UI: al registrarse un pago, abrir la ventana del recibo
    private void CobrosView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_vm is not null)
            _vm.PagoRegistrado -= MostrarRecibo;

        _vm = e.NewValue as CobrosViewModel;
        if (_vm is not null)
            _vm.PagoRegistrado += MostrarRecibo;
    }

    private void MostrarRecibo(ReciboPago recibo)
    {
        var ventana = new ReciboWindow(recibo) { Owner = Window.GetWindow(this) };
        ventana.ShowDialog();
    }
}
