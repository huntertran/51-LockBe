using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShareClass.Model;
using ShareClass.Model.Rss;
using ShareClass.Utilities.Helpers;
using ShareClass.ViewModel;
using ShareClass.ViewModel.RssGroup;
using ShareClass.ViewModel.StartGroup;

namespace LockBe.View.RssGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RssPage
    {
        public RssViewModel Vm => (RssViewModel)DataContext;
        public StartViewModel StartVm => ((ViewModelLocator)Application.Current.Resources["Locator"]).StartVm;

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public RssPage()
        {
            InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_Tick;

            Loaded += RssPage_Loaded;
        }

        private void RssPage_Loaded(object sender, RoutedEventArgs e)
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

        private async void _timer_Tick(object sender, object e)
        {
            if (!string.IsNullOrEmpty(RssSourceTextBox.Text))
            {
                await Vm.GetRssTask(RssSourceTextBox.Text);
                _timer.Stop();
            }
            _timer.Stop();
        }

        private async void RssSourceTextBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await Vm.GetRssTask(RssSourceTextBox.Text);
        }

        private void RssSourceTextBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            _timer.Start();
        }

        private async void RssItemTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int num;
            var validate = int.TryParse(RssItemTextBox.Text, out num);
            if (validate)
            {
                if (num == Vm.RssItemNumber) return;
                Vm.RssItemNumber = num;
                await StartVm.UpdateListTask();
            }
            else
            {
                RssItemTextBox.Text = "0";
            }           
        }

        private void ReadArticleButton_OnClick(object sender, RoutedEventArgs e)
        {
            var rssItem = ((Button) sender).DataContext as RssItem;
            
            if (rssItem != null) StartVm.NavigateWebView(rssItem.Link);
        }

        private async void RssToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            if (IsDisplayRssToggleSwitch.IsOn == Vm.IsEnabled) return;
            SettingsHelper.SetSetting(SettingKey.IsDisplayRss.ToString(), IsDisplayRssToggleSwitch.IsOn);
            
            await StartVm.UpdateListTask();
        }

        private async void RssDescriptionToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            if (IsDisplayRssDescriptionToggleSwitch.IsOn == Vm.IsDisplayRssDescription) return;
            SettingsHelper.SetSetting(SettingKey.IsDisplayRssDescription.ToString(), IsDisplayRssDescriptionToggleSwitch.IsOn);

            await StartVm.UpdateListTask();
        }
    }
}