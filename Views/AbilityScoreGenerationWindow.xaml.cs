using System.Windows;
using System.Windows.Controls;
using GToonManager.ViewModels;
using GToonManager.Controls;

namespace GToonManager.Views
{
    public partial class AbilityScoreGenerationWindow : Window
    {
        private object? _viewModel;
        
        public AbilityScoreGenerationWindow()
        {
            InitializeComponent();
        }

        public void SetContent(string title, UserControl control, object viewModel)
        {
            TitleText.Text = title;
            ContentArea.Content = control;
            control.DataContext = viewModel;
            _viewModel = viewModel;
            
            // Show apply button for certain view models
            if (viewModel is StandardArrayViewModel || viewModel is PointBuyViewModel || 
                viewModel is FreeEntryViewModel || viewModel is RollingViewModel)
            {
                ApplyButton.Visibility = Visibility.Visible;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Apply the changes based on the view model type
                switch (_viewModel)
                {
                    case StandardArrayViewModel standardArray:
                        if (standardArray.CanApplyStandardArray())
                        {
                            standardArray.ApplyStandardArray();
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Please complete all ability score assignments before applying.", 
                                          "Incomplete Assignment", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                        
                    case PointBuyViewModel pointBuy:
                        if (pointBuy.CanApplyPointBuy)
                        {
                            pointBuy.ApplyPointBuy();
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Please use all available points before applying.", 
                                          "Points Remaining", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                        
                    case FreeEntryViewModel freeEntry:
                        // Free entry can always be applied
                        DialogResult = true;
                        Close();
                        break;
                        
                    case RollingViewModel rolling:
                        if (rolling.AssignmentViewModel?.CanApplyAssignments == true)
                        {
                            rolling.AssignmentViewModel.ApplyAssignments();
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Please generate scores and complete assignments before applying.", 
                                          "Incomplete Assignment", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        break;
                        
                    default:
                        DialogResult = true;
                        Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying ability scores: {ex.Message}", 
                              "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 