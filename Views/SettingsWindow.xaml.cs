using System.Windows;
using GToonManager.ViewModels;
using GToonManager.Models;

namespace GToonManager.Views;

public partial class SettingsWindow : Window
{
    private SettingsViewModel _viewModel;

    public SettingsWindow(Settings settings)
    {
        InitializeComponent();
        
        _viewModel = new SettingsViewModel(settings);
        DataContext = _viewModel;
        
        // Subscribe to ViewModel events to handle dialog closing
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingsViewModel.DialogResult))
        {
            if (_viewModel.DialogResult.HasValue)
            {
                DialogResult = _viewModel.DialogResult;
                Close();
            }
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.Dispose();
        }
        base.OnClosed(e);
    }
} 