using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShareClass.Model;
using ShareClass.Utilities.Helpers;
using ShareClass.ViewModel;
using ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup;
using ShareClass.ViewModel.StartGroup;

namespace LockBe.View.ImageSourceGroup.ImageSourceSettingGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BingSettingPage
    {
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public BingSettingViewModel Vm => (BingSettingViewModel)DataContext;

        public StartViewModel StartVm => ((ViewModelLocator) Application.Current.Resources["Locator"]).StartVm;

        public BingSettingPage()
        {
            InitializeComponent();
            Loaded += BingSettingPage_Loaded;
        }

        private void BingSettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            string code = SettingManager.BingGetLanguage();
            Debug.Assert(LanguageComboBox.Items != null, "LanguageComboBox.Items != null");
            foreach (BingLanguage bingLanguage in LanguageComboBox.Items)
            {
                if (bingLanguage.Code == code)
                {
                    LanguageComboBox.SelectedItem = bingLanguage;
                }
            }
        }

        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                BingLanguage b = e.AddedItems[0] as BingLanguage;
                if (b != null)
                {
                    if (b.Code == Vm.LanguageCode) return;
                    SettingManager.BingSetLanguage(b.Code);
                    Vm.LanguageCode = b.Code;
                }
                await Vm.GetImageRoot();

                await StartVm.UpdateListTask();
            }
        }
            
        private void ShowImageInfoToggle_OnToggled(object sender, RoutedEventArgs e)
        {
            if (ShowImageInfoToggle.IsOn != Vm.IsShowImageInfo)
            {
                Vm.IsShowImageInfo = ShowImageInfoToggle.IsOn;
                Vm.UpdatePreviewImage();
            }       
        }
    }
}