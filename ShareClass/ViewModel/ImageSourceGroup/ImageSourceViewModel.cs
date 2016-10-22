using System.Collections.ObjectModel;
using Microsoft.Graphics.Canvas;
using ShareClass.Model;
using ShareClass.Utilities.Helpers;
using ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup;

namespace ShareClass.ViewModel.ImageSourceGroup
{
    public class ImageSourceViewModel : ObservableObject
    {
        private ObservableCollection<ImageSourceItem> _imageSourceItemsCollection;

        public ObservableCollection<ImageSourceItem> ImageSourceItemsCollection
        {
            get { return _imageSourceItemsCollection; }
            set
            {
                if (Equals(value, _imageSourceItemsCollection)) return;
                _imageSourceItemsCollection = value;
                OnPropertyChanged();
            }
        }

        private ImageSourceItem _selectedSource;

        public ImageSourceItem SelectedSource
        {
            get
            {
                return _selectedSource;
            }
            set
            {
                if (Equals(value, _selectedSource)) return;
                _selectedSource = value;
                OnPropertyChanged();
            }
        }

        #region ViewModels

        private BingSettingViewModel _bingSettingVm;
        private FlickrSettingViewModel _flickrSettingVm;
        private MyFolderSettingViewModel _myFolderSettingVm;
        

        public BingSettingViewModel BingSettingVm
        {
            get { return _bingSettingVm; }
            set
            {
                if (Equals(value, _bingSettingVm)) return;
                _bingSettingVm = value;
                OnPropertyChanged();
            }
        }

        public FlickrSettingViewModel FlickrSettingVm
        {
            get { return _flickrSettingVm; }
            set
            {
                if (Equals(value, _flickrSettingVm)) return;
                _flickrSettingVm = value;
                OnPropertyChanged();
            }
        }

        public MyFolderSettingViewModel MyFolderSettingVm
        {
            get { return _myFolderSettingVm; }
            set
            {
                if (Equals(value, _myFolderSettingVm)) return;
                _myFolderSettingVm = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ImageSourceViewModel()
        {
            //Init child View Models
            BingSettingVm = new BingSettingViewModel();
            FlickrSettingVm = new FlickrSettingViewModel();
            MyFolderSettingVm = new MyFolderSettingViewModel();

            Initialize();
        }

        private void Initialize()
        {
            ImageSourceItemsCollection = new ObservableCollection<ImageSourceItem>();

            ImageSourceItem i = new ImageSourceItem
            {
                Name = "Bing Everyday Image",
                Number = 0
            };
            ImageSourceItemsCollection.Add(i);

            i = new ImageSourceItem
            {
                Name = "Flickr",
                Number = 1
            };
            ImageSourceItemsCollection.Add(i);

            //i = new ImageSourceItem
            //{
            //    Name = "500px",
            //    Number = 2
            //};
            //ImageSourceItemsCollection.Add(i);

            //i = new ImageSourceItem
            //{
            //    Name = "My folder",
            //    Number = 2
            //};
            //ImageSourceItemsCollection.Add(i);

            if (SelectedSource == null)
            {
                SelectedSource = ImageSourceItemsCollection[0];
            }
        }

        public void DrawInfo(CanvasDrawingSession ds, CanvasDevice device)
        {
            int imageService = SettingManager.GetImageService();
            //Currently only Bing have draw info
            if (SettingManager.BingGetShowInfo() && imageService == 0)
            {
                BingSettingVm.DrawInfo(ds, device);
            }
        }
    }
}