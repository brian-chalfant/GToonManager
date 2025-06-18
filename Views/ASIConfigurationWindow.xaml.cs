using System.Windows;
using GToonManager.ViewModels;

namespace GToonManager.Views;

public partial class ASIConfigurationWindow : Window
{
    public ASIConfigurationWindow(ASIConfigurationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // Subscribe to close requests
        viewModel.CloseRequested += (s, e) => Close();
        viewModel.ApplyRequested += (s, e) => DialogResult = true;
    }
} 