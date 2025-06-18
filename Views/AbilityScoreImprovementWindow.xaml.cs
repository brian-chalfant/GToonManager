using System.Windows;
using GToonManager.ViewModels;

namespace GToonManager.Views;

public partial class AbilityScoreImprovementWindow : Window
{
    public AbilityScoreImprovementWindow(AbilityScoreImprovementViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // Subscribe to close request
        viewModel.CloseRequested += (s, e) => Close();
    }
} 