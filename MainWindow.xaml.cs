using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GToonManager.ViewModels;

namespace GToonManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ApplyCalculatedArmorClass_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.CurrentCharacter.ArmorClass = viewModel.CurrentCharacter.CalculatedArmorClass;
            viewModel.StatusMessage = $"Applied calculated Armor Class: {viewModel.CurrentCharacter.CalculatedArmorClass}";
        }
    }

    private void ApplyCalculatedHitPoints_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            var calculatedHp = viewModel.CurrentCharacter.CalculatedMaxHitPoints;
            viewModel.CurrentCharacter.MaxHitPoints = calculatedHp;
            viewModel.CurrentCharacter.HitPoints = calculatedHp; // Set current HP to max
            viewModel.StatusMessage = $"Applied calculated Hit Points: {calculatedHp} (both current and max)";
        }
    }
}