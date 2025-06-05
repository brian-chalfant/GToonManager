using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GToonManager.ViewModels;

namespace GToonManager.Converters
{
    public class AbilityScoreBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = value as MainViewModel;
            var ability = parameter as string;
            
            if (string.IsNullOrEmpty(ability)) return Brushes.Transparent;
            
            var bonus = vm?.GetAbilityScoreBonus(ability);
            if (bonus == 2) return new SolidColorBrush(Color.FromRgb(46, 139, 87)); // green
            if (bonus == 1) return new SolidColorBrush(Color.FromRgb(70, 130, 180)); // blue
            return Brushes.Transparent;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = value as MainViewModel;
            var ability = parameter as string;
            
            if (string.IsNullOrEmpty(ability)) return new Thickness(0);
            
            var bonus = vm?.GetAbilityScoreBonus(ability);
            if (bonus == 2) return new Thickness(4);
            if (bonus == 1) return new Thickness(3);
            return new Thickness(0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBonusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = value as MainViewModel;
            var ability = parameter as string;
            
            if (string.IsNullOrEmpty(ability)) return string.Empty;
            
            var bonus = vm?.GetAbilityScoreBonus(ability);
            if (bonus == 2) return "+2 BONUS";
            if (bonus == 1) return "+1 BONUS";
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBonusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = value as MainViewModel;
            var ability = parameter as string;
            
            if (string.IsNullOrEmpty(ability)) return Visibility.Collapsed;
            
            return (vm?.IsAbilityScoreSelected(ability) ?? false) ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBorderMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = values[0] as MainViewModel;
            var ability = values[1] as string;
            
            System.Diagnostics.Debug.WriteLine($"BorderConverter: VM={vm != null}, Ability='{ability}'");
            
            if (string.IsNullOrEmpty(ability)) return Brushes.Transparent;
            
            var bonus = vm?.GetAbilityScoreBonus(ability);
            System.Diagnostics.Debug.WriteLine($"BorderConverter: Bonus={bonus}");
            
            if (bonus == 2) return new SolidColorBrush(Color.FromRgb(46, 139, 87)); // green
            if (bonus == 1) return new SolidColorBrush(Color.FromRgb(70, 130, 180)); // blue
            return Brushes.Transparent;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBorderThicknessMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = values[0] as MainViewModel;
            var ability = values[1] as string;
            
            System.Diagnostics.Debug.WriteLine($"ThicknessConverter: VM={vm != null}, Ability='{ability}'");
            
            if (string.IsNullOrEmpty(ability)) return new Thickness(0);
            
            var bonus = vm?.GetAbilityScoreBonus(ability);
            System.Diagnostics.Debug.WriteLine($"ThicknessConverter: Bonus={bonus}");
            
            if (bonus == 2) return new Thickness(4);
            if (bonus == 1) return new Thickness(3);
            return new Thickness(0);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBonusTextMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = values[0] as MainViewModel;
            var ability = values[1] as string;
            
            System.Diagnostics.Debug.WriteLine($"BonusTextConverter: VM={vm != null}, Ability='{ability}'");
            
            if (string.IsNullOrEmpty(ability)) return string.Empty;
            
            var bonus = vm?.GetAbilityScoreBonus(ability);
            System.Diagnostics.Debug.WriteLine($"BonusTextConverter: Bonus={bonus}");
            
            if (bonus == 2) return "+2 BONUS";
            if (bonus == 1) return "+1 BONUS";
            return string.Empty;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class AbilityScoreBonusVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = values[0] as MainViewModel;
            var ability = values[1] as string;
            
            System.Diagnostics.Debug.WriteLine($"VisibilityConverter: VM={vm != null}, Ability='{ability}'");
            
            if (string.IsNullOrEmpty(ability)) return Visibility.Collapsed;
            
            var isSelected = vm?.IsAbilityScoreSelected(ability) ?? false;
            System.Diagnostics.Debug.WriteLine($"VisibilityConverter: IsSelected={isSelected}");
            
            return isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
} 