using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShareClass.Model;
using ShareClass.Model.GoogleMap;
using ShareClass.ViewModel.WeatherGroup;

namespace LockBe.View.WeatherGroup
{
    public sealed partial class WeatherPage
    {
        public WeatherViewModel Vm => (WeatherViewModel)DataContext;
        
        public WeatherPage()
        {
            InitializeComponent();

            Loaded += WeatherPage_Loaded;
        }

        private void WeatherPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PositionComboBox.Items != null)
                foreach (ImageSourceItem item in PositionComboBox.Items)
                {
                    if (item.Number == Vm.SelectedPosition.Number)
                    {
                        PositionComboBox.SelectedItem = item;
                    }
                }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Result r = args.SelectedItem as Result;
            if (r != null) sender.Text = r.FormattedAddress;
        }
    }
}
