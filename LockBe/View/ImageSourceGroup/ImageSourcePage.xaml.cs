using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LockBe.View.ImageSourceGroup.ImageSourceSettingGroup;
using ShareClass.Model;
using ShareClass.Utilities.Helpers;
using ShareClass.ViewModel;
using ShareClass.ViewModel.ImageSourceGroup;
using ShareClass.ViewModel.StartGroup;

namespace LockBe.View.ImageSourceGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageSourcePage
    {
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public ImageSourceViewModel Vm => (ImageSourceViewModel) DataContext;

        public StartViewModel StartVm => ((ViewModelLocator)Application.Current.Resources["Locator"]).StartVm;

        public ImageSourcePage()
        {
            InitializeComponent();
            Loaded += ImageSourcePage_Loaded;
        }

        private void ImageSourcePage_Loaded(object sender, RoutedEventArgs e)
        {
            int index = SettingManager.GetImageService();
            Debug.Assert(SourceComboBox.Items != null, "SourceComboBox.Items != null");
            foreach (ImageSourceItem item in SourceComboBox.Items)
            {
                if (item.Number == index)
                {
                    SourceComboBox.SelectedItem = item;
                }
            }
        }

        private async void SourceComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceComboBox.SelectedIndex == -1)
            {
                return;
            }

            int oldValue = SettingManager.GetImageService();

            if (oldValue != SourceComboBox.SelectedIndex)
            {
                //Image service changed
                StaticData.IsImageServiceChanged = true;
            }

            //Save to setting
            SettingManager.SetImageService(SourceComboBox.SelectedIndex);

            NavigateToFunction(ImageSourceSettingsFrame, (ImageSourceItem) SourceComboBox.SelectedItem);

            Vm.SelectedSource = (ImageSourceItem) SourceComboBox.SelectedItem;

            var vm = ((Grid)Frame.Parent).DataContext as StartViewModel;
            if (vm != null)
            {
                var imageSourceItem = (ImageSourceItem) SourceComboBox.SelectedItem;
                //if (imageSourceItem != null && imageSourceItem.Number != 0)
                //{
                //    //Is not Bing, because bing have it own UpdateListTask call
                //    await vm.UpdateListTask();
                //}
                if (imageSourceItem != null)
                {
                    await vm.UpdateListTask();
                }
            }
            var selectedItem = (ImageSourceItem) SourceComboBox.SelectedItem;
            if (selectedItem != null && selectedItem.Number == 2)
            {
                //Local folder
                LocalImageGridView.Visibility = Visibility.Visible;
                OnlineImageGridView.Visibility = Visibility.Collapsed;
            }
            else
            {
                LocalImageGridView.Visibility = Visibility.Collapsed;
                OnlineImageGridView.Visibility = Visibility.Visible;
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            bool success = await StartVm.ChangeCurrentBackgroundTask();
            if (success)
            {
                MessageDialog msg = new MessageDialog("Lockscreen changed");
                await msg.ShowAsync();
            }
        }

        public void NavigateToFunction(Frame frame, ImageSourceItem i)
        {
            switch (i.Number)
            {
                case 0:
                {
                    frame.Navigate(typeof (BingSettingPage));
                    break;
                }
                case 1:
                {
                    frame.Navigate(typeof (FlickrSettingPage));
                    break;
                }
                case 2:
                {
                    frame.Navigate(typeof (MyFolderSettingPage));
                    break;
                }
                default:
                {
                    frame.Navigate(typeof (BingSettingPage));
                    break;
                }
            }
        }
    }
}