using System.Collections.ObjectModel;
using System.Windows;
using GToonManager.ViewModels;

namespace GToonManager.Views;

public partial class FeaturesAndTraitsWindow : Window
{
    public FeaturesAndTraitsWindow(ObservableCollection<FeatureTraitViewModel> featuresAndTraits)
    {
        InitializeComponent();
        DataContext = new FeaturesAndTraitsWindowViewModel(featuresAndTraits);
    }

    private void ContinueExport_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelExport_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

public class FeaturesAndTraitsWindowViewModel
{
    public ObservableCollection<FeatureTraitViewModel> FeaturesAndTraits { get; }

    public FeaturesAndTraitsWindowViewModel(ObservableCollection<FeatureTraitViewModel> featuresAndTraits)
    {
        FeaturesAndTraits = featuresAndTraits;
    }
} 