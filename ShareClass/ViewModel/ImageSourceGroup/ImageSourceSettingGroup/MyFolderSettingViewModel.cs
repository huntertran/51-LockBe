using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
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

        public async Task GetFolder(bool isEventCall = true)
        {
            StorageFile s;
            bool fileExist = false;
            string savedPath = SettingManager.GetSavePath();

            // If savedPath string is Null & method is NOT called from GUI's event
            if (!string.IsNullOrEmpty(savedPath) && !isEventCall)
            {
                string token = SettingManager.GetSaveToken();
                await Task.Run(() =>
                {
                    Task.Yield();
                    fileExist = File.Exists(savedPath);
                    
                });

                //If file isn't exist then open FilePicker for User in the next time App is opened
                if (!fileExist) goto NewPath;
                s = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                goto GetFile;
            }

        NewPath:
            {
                FileOpenPicker f = new FileOpenPicker();
                f.FileTypeFilter.Add(".jpg");
                f.FileTypeFilter.Add(".jpeg");
                f.FileTypeFilter.Add(".png");
                s = await f.PickSingleFileAsync();
                if (s != null)
                {
                    var destinationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Offline Background",
                   CreationCollisionOption.OpenIfExists);

                    //Delete all old files
                    var oldFile = await destinationFolder.GetFilesAsync();
                    foreach (var storageFile in oldFile)
                    {
                        if (StorageHelper.IsFileExisted(destinationFolder, storageFile.Name))
                        {
                            await storageFile.DeleteAsync(StorageDeleteOption.Default);
                        }
                    }

                    //Copy User's image to Offline image folder
                    var offlineImage = await s.CopyAsync(destinationFolder);

                    //Save Offline image's path & token 
                    //Use Offline image to draw, NOT use user's image to draw like before
                    //Improve App UX when User delete/move/rename their images for other purposes
                    StorageApplicationPermissions.FutureAccessList.Clear();
                    var token = StorageApplicationPermissions.FutureAccessList.Add(offlineImage);
                    SettingManager.SetSaveMode(3, offlineImage.Path, token);
                }
            }
           
        GetFile:
            {
                if (s != null)
                {
                    //FolderCollection.Add(s);
                    GetSingleFileTask(s);
                }

            }
            

            //ToDo: User Offline Image Folder in future
            //StorageFolder s;
            //string savedPath = SettingManager.GetSavePath();

            //if (!string.IsNullOrEmpty(savedPath))
            //{
            //    string token = SettingManager.GetSaveToken();
            //    s = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
            //}
            //else
            //{
            //    FolderPicker f = new FolderPicker();
            //    f.FileTypeFilter.Add(".jpg");
            //    f.FileTypeFilter.Add(".jpeg");
            //    f.FileTypeFilter.Add(".png");
            //    s = await f.PickSingleFolderAsync();
            //    if (s != null)
            //    {
            //        StorageApplicationPermissions.FutureAccessList.Clear();
            //        var token = StorageApplicationPermissions.FutureAccessList.Add(s);
            //        SettingManager.SetSaveMode(3, s.Path, token);
            //    }
            //}

            //if (s != null)
            //{
            //    FolderCollection.Add(s);
            //    await GetFileTask(s);
            //}
        }

        //private async Task GetFileTask(StorageFolder s)
        //{
        //    MyFolderImageRoot = new ObservableCollection<StorageFile>();
        //    var temp = await s.GetFilesAsync(CommonFileQuery.OrderByName);
        //    foreach (StorageFile file in temp)
        //    {
        //        if (file.FileType.ToLower() == ".jpg" || file.FileType.ToLower() == ".png" ||
        //            file.FileType.ToLower() == ".jpeg")
        //        {
        //            MyFolderImageRoot.Add(file);
        //        }
        //    }
        //}

        private void GetSingleFileTask(StorageFile s)
        {
            MyFolderImageRoot = new ObservableCollection<StorageFile> {s};
        }
    }
}
