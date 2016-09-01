using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using ShareClass.Utilities.Helpers;

namespace ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup
{
    public class MyFolderSettingViewModel : ObservableObject
    {
        private ObservableCollection<StorageFolder> _folderCollection;
        private ObservableCollection<StorageFile> _myFolderImageRoot;
        private bool _isSubFolderEnabled;

        public ObservableCollection<StorageFolder> FolderCollection
        {
            get { return _folderCollection; }
            set
            {
                if (Equals(value, _folderCollection)) return;
                _folderCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StorageFile> MyFolderImageRoot
        {
            get { return _myFolderImageRoot; }
            set
            {
                if (Equals(value, _myFolderImageRoot)) return;
                _myFolderImageRoot = value;
                OnPropertyChanged();
            }
        }

        public bool IsSubFolderEnabled
        {
            get { return _isSubFolderEnabled; }
            set
            {
                if (value == _isSubFolderEnabled) return;
                _isSubFolderEnabled = value;
                OnPropertyChanged();
            }
        }

        public MyFolderSettingViewModel()
        {
            FolderCollection = new ObservableCollection<StorageFolder>();
        }

        public async void GetFolderEvent()
        {
            await TwoStepGetFolder();
        }

        private async Task TwoStepGetFolder()
        {
            await GetFolder();
            var v = Application.Current.Resources["Locator"] as ViewModelLocator;
            Debug.Assert(v != null, "v != null");
            await v.StartVm.UpdateListTask();
        }

        public async Task GetFolder()
        {
            StorageFolder s;
            string savedPath = SettingManager.GetSavePath();

            if (!string.IsNullOrEmpty(savedPath))
            {
                string token = SettingManager.GetSaveToken();
                s = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
            }
            else
            {
                FolderPicker f = new FolderPicker();
                f.FileTypeFilter.Add(".jpg");
                f.FileTypeFilter.Add(".jpeg");
                f.FileTypeFilter.Add(".png");
                s = await f.PickSingleFolderAsync();
                if (s != null)
                {
                    StorageApplicationPermissions.FutureAccessList.Clear();
                    var token = StorageApplicationPermissions.FutureAccessList.Add(s);
                    SettingManager.SetSaveMode(3, s.Path, token);
                }
            }

            if (s != null)
            {
                FolderCollection.Add(s);
                await GetFileTask(s);
            }
        }

        private async Task GetFileTask(StorageFolder s)
        {
            MyFolderImageRoot = new ObservableCollection<StorageFile>();
            var temp = await s.GetFilesAsync(CommonFileQuery.OrderByName);
            foreach (StorageFile file in temp)
            {
                if (file.FileType.ToLower() == ".jpg" || file.FileType.ToLower() == ".png" ||
                    file.FileType.ToLower() == ".jpeg")
                {
                    MyFolderImageRoot.Add(file);
                }
            }
        }
    }
}
