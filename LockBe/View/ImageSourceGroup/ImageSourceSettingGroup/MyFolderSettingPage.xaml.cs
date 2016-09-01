using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShareClass.Utilities.Helpers;
using ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup;

namespace LockBe.View.ImageSourceGroup.ImageSourceSettingGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyFolderSettingPage
    {

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public MyFolderSettingViewModel Vm => (MyFolderSettingViewModel)DataContext;

        public MyFolderSettingPage()
        {
            InitializeComponent();
        }

        private void RemoveFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            StorageFolder s = ((AppBarButton) sender).Tag as StorageFolder;
            Vm.FolderCollection.Remove(s);
            SettingManager.SetSaveMode(3);
        }
    }
}
