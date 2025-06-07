using System.Windows;
using GToonManager.ViewModels;

namespace GToonManager.Views
{
    public partial class SubclassSelectionWindow : Window
    {
        public SubclassSelectionWindow()
        {
            InitializeComponent();
        }

        public SubclassSelectionWindow(SubclassSelectionViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
} 