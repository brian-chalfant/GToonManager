using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GToonManager.Views
{
    public partial class LoadingWindow : Window, INotifyPropertyChanged
    {
        private bool _isComplete = false;
        private string _currentMessage = "🧙‍♂️ Preparing the magical realm...";
        private double _currentProgress = 0;

        public LoadingWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadBackgroundImage();
            StartGlowAnimation();
        }

        private void LoadBackgroundImage()
        {
            try
            {
                // Debug information about the image loading
                System.Diagnostics.Debug.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                System.Diagnostics.Debug.WriteLine($"Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
                
                // Check if the file exists in expected locations
                var possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures", "20250604_1917_Horizontally Oriented Wizard.png"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "20250604_1917_Horizontally Oriented Wizard.png")
                };

                foreach (var path in possiblePaths)
                {
                    System.Diagnostics.Debug.WriteLine($"Checking path: {path} - Exists: {File.Exists(path)}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadBackgroundImage: {ex.Message}");
            }
        }

        public bool IsComplete
        {
            get => _isComplete;
            set
            {
                _isComplete = value;
                OnPropertyChanged(nameof(IsComplete));
                if (value)
                {
                    ShowCloseButton();
                }
            }
        }

        public string CurrentMessage
        {
            get => _currentMessage;
            set
            {
                _currentMessage = value;
                OnPropertyChanged(nameof(CurrentMessage));
                Dispatcher.Invoke(() => LoadingMessage.Text = value);
            }
        }

        public double CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = Math.Max(0, Math.Min(100, value));
                OnPropertyChanged(nameof(CurrentProgress));
                Dispatcher.Invoke(() => 
                {
                    MainProgressBar.Value = _currentProgress;
                    UpdateProgressText();
                });
            }
        }

        private void StartGlowAnimation()
        {
            var storyboard = (Storyboard)Resources["PulseAnimation"];
            storyboard.Begin();
        }

        private void UpdateProgressText()
        {
            // Find the TextBlock in the ProgressBar template
            if (MainProgressBar.Template?.FindName("ProgressText", MainProgressBar) is System.Windows.Controls.TextBlock progressText)
            {
                progressText.Text = $"{_currentProgress:F0}%";
            }
        }

        private void ShowCloseButton()
        {
            Dispatcher.Invoke(() => CloseButton.Visibility = Visibility.Visible);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        // Predefined loading messages for different stages
        public void SetSplashScreenMessages()
        {
            var messages = new[]
            {
                "🧙‍♂️ Awakening the ancient magic...",
                "📚 Loading spell components...",
                "⚔️ Sharpening digital swords...",
                "🏰 Building the character realm...",
                "🎲 Rolling for initiative...",
                "✨ Magic is ready!"
            };

            Task.Run(async () =>
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    CurrentMessage = messages[i];
                    CurrentProgress = (i + 1) * (100.0 / messages.Length);
                    await Task.Delay(500); // Half second per stage
                }
                
                await Task.Delay(500); // Brief pause at completion
                IsComplete = true;
            });
        }

        public void SetPdfExportMessages()
        {
            var messages = new[]
            {
                "📄 Preparing character sheet...",
                "🖋️ Scribing character details...", 
                "🎨 Applying medieval formatting...",
                "📊 Calculating ability scores...",
                "⚔️ Adding equipment and spells...",
                "📋 Finalizing character sheet...",
                "💾 Saving PDF document...",
                "✅ Export complete!"
            };

            Task.Run(async () =>
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    CurrentMessage = messages[i];
                    CurrentProgress = (i + 1) * (100.0 / messages.Length);
                    
                    // Simulate variable work times for different stages
                    int delay = i switch
                    {
                        0 => 300,  // Quick prep
                        1 => 800,  // Scribing takes time
                        2 => 600,  // Formatting
                        3 => 400,  // Calculations
                        4 => 900,  // Equipment/spells (lots of data)
                        5 => 700,  // Finalizing
                        6 => 1000, // Saving (file I/O)
                        7 => 500,  // Completion
                        _ => 500
                    };
                    
                    await Task.Delay(delay);
                }
                
                await Task.Delay(800); // Brief pause to show completion
                IsComplete = true;
            });
        }

        // Method to update progress from external processes
        public void UpdateProgress(double progress, string? message = null)
        {
            CurrentProgress = progress;
            if (!string.IsNullOrEmpty(message))
            {
                CurrentMessage = message;
            }
        }

        // Static method to show splash screen
        public static async Task<bool?> ShowSplashScreen()
        {
            var loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.SetSplashScreenMessages();
            
            // Auto-close after completion or wait for user
            await Task.Delay(3500); // Total time for splash
            
            if (loadingWindow.IsVisible)
            {
                loadingWindow.Close();
            }
            
            return true;
        }

        // Static method to show PDF export progress
        public static LoadingWindow ShowPdfExportProgress()
        {
            var loadingWindow = new LoadingWindow();
            loadingWindow.CurrentMessage = "📄 Preparing PDF export...";
            loadingWindow.Show();
            return loadingWindow;
        }

        // Manual progress control for custom operations
        public void StartManualProgress(string initialMessage = "🔄 Processing...")
        {
            CurrentMessage = initialMessage;
            CurrentProgress = 0;
        }

        public void CompleteProgress(string completionMessage = "✅ Complete!")
        {
            CurrentMessage = completionMessage;
            CurrentProgress = 100;
            IsComplete = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Stop animations when closing
            if (Resources["PulseAnimation"] is Storyboard storyboard)
            {
                storyboard.Stop();
            }
            base.OnClosing(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 