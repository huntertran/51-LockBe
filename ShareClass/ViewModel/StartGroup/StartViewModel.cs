using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using ShareClass.Model;
using ShareClass.Model.Flickr;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using ShareClass.Utilities;
using ShareClass.Utilities.Helpers;
using ShareClass.Utilities.Helpers.SourceDataHelper;
using ShareClass.ViewModel.ImageSourceGroup;
using ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup;
using ShareClass.ViewModel.NoteGroup;
using ShareClass.ViewModel.QuoteGroup;
using ShareClass.ViewModel.RssGroup;
using ShareClass.ViewModel.WeatherGroup;

namespace ShareClass.ViewModel.StartGroup
{
    public class StartViewModel : ObservableObject
    {
        #region Define

        private ObservableCollection<MenuListItem> _functionItemList;
        private ObservableCollection<MenuListItem> _bottomFunctionItemList;
        private ObservableCollection<string> _imageList;
        private ObservableCollection<LocalImage> _localImageList;
        private StorageFolder _backgroundFolder;
        private StorageFile _backgroundFile;

        private byte _currentImageListService;
        private bool _isImageSaved;
        private bool _isIconSaved;
        private bool _isDrawing;
        private bool _isDesktopBackground;

        private CanvasBitmap _bitmap;
        private CanvasRenderTarget _renderTarget;
        private BitmapSource _previewImage;

        //private readonly DispatcherTimer _updateImageTimer;

        public CanvasBitmap IconBitmap;
        public bool StartControlPageLoaded;

        public bool IsDesktopBackground
        {
            get { return _isDesktopBackground; }
            set
            {
                if (Equals(value, _isDesktopBackground)) return;
                _isDesktopBackground = value;
                SettingsHelper.SetSetting(SettingKey.IsDesktopBackground.ToString(), _isDesktopBackground);
                OnPropertyChanged();
            }
        }
        public BitmapSource PreviewImage
        {
            get { return _previewImage; }
            set
            {
                if (Equals(value, _previewImage)) return;
                _previewImage = value;
                OnPropertyChanged();
            }
        }

        public bool IsIconSaved
        {
            get { return _isIconSaved; }
            set
            {
                if (Equals(value, _isIconSaved)) return;
                _isIconSaved = value;
                OnPropertyChanged();
            }
        }

        public bool IsDrawing
        {
            get { return _isDrawing; }
            set
            {
                if (Equals(value, _isDrawing)) return;
                _isDrawing = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MenuListItem> FunctionItemList
        {
            get { return _functionItemList; }
            set
            {
                if (Equals(value, _functionItemList)) return;
                _functionItemList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MenuListItem> BottomFunctionItemList
        {
            get { return _bottomFunctionItemList; }
            set
            {
                if (Equals(value, _bottomFunctionItemList)) return;
                _bottomFunctionItemList = value;
                OnPropertyChanged();
            }
        }

        public bool IsSplitViewPaneOpened
        {
            get { return _isSplitViewPaneOpened; }
            set
            {
                if (value == _isSplitViewPaneOpened) return;
                _isSplitViewPaneOpened = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ImageList
        {
            get { return _imageList; }
            set
            {
                if (Equals(value, _imageList)) return;
                _imageList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocalImage> LocalImageList
        {
            get { return _localImageList; }
            set
            {
                if (Equals(value, _localImageList)) return;
                _localImageList = value;
                OnPropertyChanged();
            }
        }

        public bool IsImageSaved
        {
            get { return _isImageSaved; }
            set
            {
                if (Equals(value, _isImageSaved)) return;
                _isImageSaved = value;
                OnPropertyChanged();
            }
        }

        public StorageFolder BackgroundFolder
        {
            get { return _backgroundFolder; }
            set
            {
                if (Equals(value, _backgroundFolder)) return;
                _backgroundFolder = value;
                OnPropertyChanged();
            }
        }

        public StorageFile BackgroundFile
        {
            get { return _backgroundFile; }

            set
            {
                if (Equals(value, _backgroundFile)) return;
                _backgroundFile = value;
                OnPropertyChanged();
            }
        }

        public CanvasRenderTarget RenderTarget
        {
            get { return _renderTarget; }
            set
            {
                if (Equals(value, _renderTarget)) return;
                _renderTarget = value;
                OnPropertyChanged();
            }
        }

        private bool _isShowWebView;

        public bool IsShowWebView
        {
            get { return _isShowWebView; }
            set
            {
                if (value == _isShowWebView) return;
                _isShowWebView = value;
                OnPropertyChanged();
            }
        }

        private bool _isImageUpdating;

        public bool IsImageUpdating
        {
            get { return _isImageUpdating; }
            set
            {
                if (value == _isImageUpdating) return;
                _isImageUpdating = value;
                OnPropertyChanged();
            }
        }

        public IStartPage StartPage { get; set; }

        #endregion

        #region Referenced ViewModel

        public ImageSourceViewModel ImageSourceVm;
        public WeatherViewModel WeatherVm;
        public QuoteViewModel QuoteVm;
        public RssViewModel RssVm;
        public NoteViewModel NoteVm;
        private bool _isSplitViewPaneOpened;

        #endregion

        public StartViewModel()
        {
            InitializeData();
            RegisterViewModels();
            RegisterBackgroundTask();

            //if (Window.Current != null && Window.Current.Dispatcher.HasThreadAccess)
            //{
            //    _updateImageTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1000)};
            //    _updateImageTimer.Tick += _updateImageTimer_Tick;
            //    //UpdateListAsync();
            //}
        }

        //private void _updateImageTimer_Tick(object sender, object e)
        //{
        //    //_updateImageTimer.Stop();
        //    //UpdateListAsync();
        //}

        private void InitializeData()
        {
            FunctionItemList = new ObservableCollection<MenuListItem>();

            MenuListItem m = new MenuListItem
            {
                Name = "Control Panel",
                IsEnabled = true,
                MenuF = MenuFunc.Start,
                Icon =
                    "M18,23L18,25 22,25 22,23z M26,15L26,17 30,17 30,15z M2,15L2,17 6,17 6,15z M19,10L21,10 21,21 24,21 24,27 21,27 21,32 19,32 19,27 16,27 16,21 19,21z M10,8L10,10 14,10 14,8z M3,8L5,8 5,13 8,13 8,19 5,19 5,32 3,32 3,19 0,19 0,13 3,13z M27,6L29,6 29,13 32,13 32,19 29,19 29,32 27,32 27,19 24,19 24,13 27,13z M11,0L13,0 13,6 16,6 16,12 13,12 13,32 11,32 11,12 8,12 8,6 11,6z"
            };

            _functionItemList.Add(m);

            m = new MenuListItem
            {
                Name = "Image Source",
                IsEnabled = false,
                MenuF = MenuFunc.ImageSource,
                Icon =
                    "M10.468046,9.9069975L1.9399997,20.377402 1.9399997,25.201005 30.059954,25.201005 30.059954,23.076125 22.532053,14.257 18.33605,19.681002z M22.494053,3.9449948C23.594055,3.944995 24.485056,4.8739932 24.485056,6.0209912 24.485056,7.1679886 23.594055,8.0969866 22.494053,8.0969866 21.394051,8.0969866 20.50305,7.1679886 20.50305,6.0209912 20.50305,4.8739932 21.394051,3.944995 22.494053,3.9449948z M1.9399997,1.9379893L1.9399997,17.305949 10.477046,6.8249954 18.306049,16.552001 22.458052,11.181998 30.059954,20.088858 30.059954,1.9379893z M0,0L31.997999,0 31.997999,22.359593 32.000057,22.362004 31.997999,22.363761 31.997999,27.14 0,27.14z"
            };

            _functionItemList.Add(m);

            m = new MenuListItem
            {
                Name = "Weather Forecast",
                IsEnabled = false,
                MenuF = MenuFunc.WeatherForecast,
                Icon =
                    "M 21.3333,29.864C 20.7453,29.864 20.2653,30.3386 20.2653,30.9426L 20.2653,33.0573C 20.2653,33.6506 20.74,34.136 21.3333,34.136C 21.9213,34.136 22.4013,33.6613 22.4013,33.0573L 22.4013,30.9426C 22.4013,30.3493 21.9267,29.864 21.3333,29.864 Z M 12.8027,29.864C 12.208,29.864 11.7347,30.3386 11.7347,30.9426L 11.7347,33.0573C 11.7347,33.6506 12.208,34.136 12.8027,34.136C 13.3907,34.136 13.864,33.6613 13.864,33.0573L 13.864,30.9426C 13.864,30.3493 13.3907,29.864 12.8027,29.864 Z M 25.5987,27.7347C 25.0107,27.7347 24.5307,28.208 24.5307,28.812L 24.5307,30.9213C 24.5307,31.516 25.0053,32 25.5987,32C 26.188,32 26.6667,31.5267 26.6667,30.9213L 26.6667,28.812C 26.6667,28.2133 26.1933,27.7347 25.5987,27.7347 Z M 29.4267,14.0933C 29.24,13.604 29.0053,13.1453 28.724,12.7186C 28.776,12.396 28.8027,12.068 28.8027,11.7347C 28.8027,8.19733 25.932,5.33331 22.4013,5.33331C 20.7653,5.33331 19.2707,5.948 18.14,6.95868C 16.6147,5.93201 14.776,5.33331 12.8027,5.33331C 7.568,5.33331 3.312,9.5213 3.20267,14.724C 1.28667,15.8333 0,17.9013 0,20.2653C 0,23.7973 2.86933,26.6666 6.40133,26.6666L 27.7347,26.6666C 31.2653,26.6666 34.136,23.792 34.136,20.2653C 34.136,17.3173 32.14,14.8333 29.4267,14.0933 Z M 22.4013,7.4693C 24.36,7.4693 26.016,8.79199 26.5107,10.5933C 25.4693,9.96399 24.2453,9.59863 22.932,9.59863C 22.2507,9.59863 21.588,9.69733 20.9693,9.88531C 20.6253,9.33331 20.2347,8.82263 19.792,8.35468C 20.5107,7.7973 21.416,7.4693 22.4013,7.4693 Z M 27.7347,24.5306L 6.396,24.5306C 4.04667,24.5306 2.136,22.6253 2.136,20.2653C 2.136,18.2493 3.536,16.552 5.42667,16.1093C 5.364,15.7293 5.33333,15.3333 5.33333,14.932C 5.33333,10.8066 8.67733,7.4693 12.8027,7.4693C 16.172,7.4693 19.0213,9.70264 19.948,12.7706C 20.7707,12.12 21.8027,11.7347 22.932,11.7347C 25.4067,11.7347 27.4373,13.5986 27.7027,16L 27.7347,16C 30.088,16 32,17.912 32,20.2653C 32,22.62 30.0933,24.5306 27.7347,24.5306 Z M 17.068,27.7347C 16.4787,27.7347 16,28.208 16,28.812L 16,30.9213C 16,31.516 16.4733,32 17.068,32C 17.656,32 18.136,31.5267 18.136,30.9213L 18.136,28.812C 18.136,28.2133 17.6613,27.7347 17.068,27.7347 Z M 29.1973,6.448L 30.6933,4.95331C 31.1147,4.53064 31.1147,3.85999 30.6973,3.43732C 30.2813,3.0213 29.6093,3.0213 29.1827,3.448L 27.688,4.93732C 27.2707,5.35999 27.26,6.03601 27.6827,6.45331C 28.0987,6.86932 28.7707,6.87463 29.1973,6.448 Z M 8.53067,27.7347C 7.94267,27.7347 7.46933,28.208 7.46933,28.812L 7.46933,30.9213C 7.46933,31.516 7.94267,32 8.53067,32C 9.12533,32 9.59867,31.5267 9.59867,30.9213L 9.59867,28.812C 9.59867,28.2133 9.12533,27.7347 8.53067,27.7347 Z M 22.4013,4.26532C 22.9893,4.26532 23.4693,3.79199 23.4693,3.18799L 23.4693,1.07867C 23.4693,0.484009 22.9947,0 22.4013,0C 21.812,0 21.3333,0.473328 21.3333,1.07867L 21.3333,3.18799C 21.3333,3.78668 21.8067,4.26532 22.4013,4.26532 Z M 29.8693,11.74C 29.8693,12.328 30.344,12.8027 30.948,12.8027L 33.0627,12.8027C 33.656,12.8027 34.136,12.328 34.136,11.74C 34.136,11.1453 33.6667,10.672 33.0627,10.672L 30.948,10.672C 30.3547,10.672 29.8693,11.1453 29.8693,11.74 Z"
            };

            _functionItemList.Add(m);

            //m = new MenuListItem
            //{
            //    Name = "Calendar",
            //    IsEnabled = true,
            //    MenuF = MenuFunc.Calendar,
            //    Icon =
            //        "M 31.292,3.19733L 26.6667,3.19733L 26.6667,1.06799C 26.6667,0.478638 26.188,0 25.5987,0C 25.0107,0 24.5307,0.478638 24.5307,1.06799L 24.5307,3.19733L 18.136,3.19733L 18.136,1.06799C 18.136,0.478638 17.656,0 17.068,0C 16.4787,0 16,0.478638 16,1.06799L 16,3.19733L 9.59867,3.19733L 9.59867,1.06799C 9.59867,0.478638 9.12,0 8.53067,0C 7.94267,0 7.46933,0.478638 7.46933,1.06799L 7.46933,3.19733L 2.844,3.19733C 1.276,3.19733 0,4.47333 0,6.04132L 0,31.2867C 0,32.86 1.276,34.136 2.844,34.136L 31.292,34.136C 32.86,34.136 34.136,32.86 34.136,31.2867L 34.136,6.04132C 34.136,4.47333 32.86,3.19733 31.292,3.19733 Z M 32,31.2867C 32,31.6827 31.6827,32 31.292,32L 2.844,32C 2.45333,32 2.136,31.6827 2.136,31.2867L 2.136,6.04132C 2.136,5.65063 2.45333,5.33331 2.844,5.33331L 7.46933,5.33331L 7.46933,7.4693C 7.46933,8.05731 7.94267,8.53064 8.53067,8.53064C 9.12,8.53064 9.59867,8.05731 9.59867,7.4693L 9.59867,5.33331L 16,5.33331L 16,7.4693C 16,8.05731 16.4787,8.53064 17.068,8.53064C 17.656,8.53064 18.136,8.05731 18.136,7.4693L 18.136,5.33331L 24.5307,5.33331L 24.5307,7.4693C 24.5307,8.05731 25.0107,8.53064 25.5987,8.53064C 26.188,8.53064 26.6667,8.05731 26.6667,7.4693L 26.6667,5.33331L 31.292,5.33331C 31.6827,5.33331 32,5.65063 32,6.04132M 7.46933,12.8027L 11.7347,12.8027L 11.7347,16L 7.46933,16L 7.46933,12.8027 Z M 7.46933,18.136L 11.7347,18.136L 11.7347,21.3333L 7.46933,21.3333L 7.46933,18.136 Z M 7.46933,23.4693L 11.7347,23.4693L 11.7347,26.6666L 7.46933,26.6666L 7.46933,23.4693 Z M 14.932,23.4693L 19.1973,23.4693L 19.1973,26.6666L 14.932,26.6666L 14.932,23.4693 Z M 14.932,18.136L 19.1973,18.136L 19.1973,21.3333L 14.932,21.3333L 14.932,18.136 Z M 14.932,12.8027L 19.1973,12.8027L 19.1973,16L 14.932,16L 14.932,12.8027 Z M 22.4013,23.4693L 26.6667,23.4693L 26.6667,26.6666L 22.4013,26.6666L 22.4013,23.4693 Z M 22.4013,18.136L 26.6667,18.136L 26.6667,21.3333L 22.4013,21.3333L 22.4013,18.136 Z M 22.4013,12.8027L 26.6667,12.8027L 26.6667,16L 22.4013,16L 22.4013,12.8027 Z"
            //};

            //_functionItemList.Add(m);

            m = new MenuListItem
            {
                Name = "RSS",
                IsEnabled = false,
                MenuF = MenuFunc.Rss,
                Icon =
                    "M5.2190108,23.563C3.445009,23.563 2.0000074,25.007 2.0000071,26.782001 2.0000074,28.556004 3.445009,30.000006 5.2190108,30.000006 6.9930129,30.000006 8.438015,28.556004 8.438015,26.782001 8.438015,25.007 6.9930129,23.563 5.2190108,23.563z M5.2190108,21.562996C8.0970144,21.562996 10.438017,23.903999 10.438017,26.782001 10.438017,29.660006 8.0970144,32.001007 5.2190108,32.001007 2.3410077,32.001007 5.15994E-06,29.660006 5.2453738E-06,26.782001 5.15994E-06,23.903999 2.3410077,21.562996 5.2190108,21.562996z M2.0650155,12.396109L2.0650155,15.617091C5.8930073,15.983089 9.394,17.593081 12.006994,20.207067 14.55899,22.761053 16.025986,26.037035 16.377985,29.957014L19.620979,29.957014C19.36298,25.40604 17.345983,21.095062 13.873991,17.72308 10.509997,14.456098 6.2540064,12.548108 2.0650155,12.396109z M1.506746,10.389066C6.3854761,10.404598 11.381339,12.515796 15.266988,16.288088 19.382978,20.285067 21.648973,25.495039 21.648973,30.95701L21.648973,31.957005 14.482989,31.957005 14.441989,31.00001C14.27899,27.170031 12.983993,24.015047 10.592998,21.621059 8.1190023,19.146072 4.7210097,17.70108 1.0240171,17.550081L0.065019748,17.511082 0.065019748,10.42312 1.0350176,10.394119C1.192111,10.390244,1.3493675,10.388565,1.506746,10.389066z M2,2.0670168L2,5.2860107C8.7439575,5.7109985 14.708984,8.3529663 19.302002,12.958984 23.80896,17.47998 26.295959,23.210022 26.704956,30L30,30 30,29.656006C29.948975,29.237976 29.92395,28.796997 29.899963,28.354004 29.875,27.89801 29.850952,27.440979 29.789978,27.015991 28.85498,20.620972 26.046997,15.064026 21.44397,10.500977 17.136963,6.2299805 11.86499,3.5390015 5.7749634,2.5009766 5.0979614,2.3859863 4.414978,2.3070068 3.7309573,2.2279663 3.4239502,2.190979 3.1149902,2.1549683 2.8079836,2.1159666 2.5769653,2.1129761 2.2929687,2.09198 2,2.0670168z M0,0L0.99999998,0C1.2769775,0 1.6499634,0.029968262 2.0219729,0.062988356 2.3339844,0.088989258 2.6439819,0.11700439 2.875,0.11700426L3.0019531,0.12500012C3.3209839,0.16601563 3.6430054,0.20300293 3.9629514,0.23999016 4.6809692,0.32397461 5.3989868,0.40802002 6.1119995,0.52996826 12.613953,1.6380005 18.246948,4.5159912 22.85199,9.0809937 27.769958,13.955994 30.768982,19.893982 31.768005,26.727966 31.84198,27.22699 31.868958,27.744019 31.895996,28.244019 31.919983,28.661011 31.940979,29.075989 31.992004,29.466003L32,29.593994 32,32 24.778992,32 24.748962,31.029968C24.544006,24.340027 22.233948,18.734009 17.885986,14.370972 13.440002,9.9119873 7.5910034,7.4459839 0.96899412,7.2399902L0,7.210022z"
            };

            _functionItemList.Add(m);

            m = new MenuListItem
            {
                Name = "Quote",
                IsEnabled = false,
                MenuF = MenuFunc.Quote,
                Icon =
                    "M6.6949539,2.0380347C4.1279736,2.0380345 2.03899,4.1270304 2.03899,6.6940251 2.03899,9.2620201 3.0929823,11.351016 5.6599617,11.351016 5.9769592,11.351016 7.1279497,11.205016 8.1599417,10.659017 8.3849401,15.988007 5.5119629,19.017 5.5119629,19.017 12.041911,15.607007 11.719913,8.566021 11.437916,6.6940251 11.403916,6.3840256 11.317917,6.0780263 11.258917,5.7830267 11.255917,5.772027 11.250917,5.7500272 11.248918,5.7430272L11.251917,5.7420268C10.810921,3.6300314,8.935936,2.0380345,6.6949539,2.0380347z M25.222938,2.0380209C22.655966,2.0380206 20.566958,4.1270056 20.566958,6.694017 20.566958,9.2620049 21.620969,11.35099 24.18794,11.35099 24.504955,11.35099 25.65595,11.204994 26.687926,10.658975 26.91293,15.988005 24.039961,19.016991 24.039961,19.016991 30.569923,15.606963 30.247904,8.5660229 29.965922,6.694017 29.931926,6.3840204 29.845928,6.0779906 29.786907,5.7830081 29.783916,5.7720218 29.778912,5.7499881 29.776928,5.7430301L29.779919,5.7419925C29.338912,3.6299975,27.463923,2.0380206,25.222938,2.0380209z M6.6949539,3.8623812E-05C9.9439278,3.863319E-05 12.657906,2.327034 13.261901,5.4020276 13.605899,6.9220247 14.946888,19.108 2.03899,21.005997 2.03899,21.005997 6.5839548,17.339003 6.4229555,12.932013 6.2999563,12.938012 6.1769581,12.938012 6.0549583,12.950012 2.3699875,13.318011 -0.003993988,10.680017 5.7220459E-06,6.6940251 0.0050067902,3.0020325 3.0029831,3.863319E-05 6.6949539,3.8623812E-05z M25.222938,0C28.471943,-5.1650204E-08 31.185917,2.3270216 31.789917,5.4020276 32.133909,6.9219828 33.474905,19.107994 20.566958,21.006001 20.566958,21.006001 25.111946,17.338955 24.950936,12.93198 24.827951,12.937962 24.704936,12.937962 24.582957,12.949986 20.897981,13.317966 18.523971,10.679971 18.527999,6.694017 18.532973,3.002008 21.530973,-5.1650204E-08 25.222938,0z"
            };

            _functionItemList.Add(m);

            m = new MenuListItem
            {
                Name = "Note",
                IsEnabled = false,
                MenuF = MenuFunc.Note,
                Icon =
                    "M 30.4013,16C 29.516,16 28.8027,16.7186 28.8027,17.5986L 28.8027,30.932L 3.19733,30.932L 3.19733,5.33331L 16.5307,5.33331C 17.416,5.33331 18.136,4.61462 18.136,3.73468C 18.136,2.8493 17.416,2.13599 16.5307,2.13599L 2.136,2.13599C 0.958667,2.13599 0,3.08801 0,4.26532L 0,32C 0,33.1773 0.958667,34.136 2.136,34.136L 29.864,34.136C 31.0413,34.136 32,33.1773 32,32L 32,17.5986C 32,16.7186 31.2867,16 30.4013,16 Z M 33.76,4.96399L 29.172,0.369324C 28.9213,0.119995 28.5933,0 28.2653,0C 27.9373,0 27.6093,0.119995 27.364,0.369324L 9.59867,18.136L 9.59867,24.5306L 16,24.5306L 33.76,6.77063C 34.0107,6.5213 34.136,6.1933 34.136,5.86401C 34.136,5.53601 34.0107,5.21332 33.76,4.96399 Z M 14.932,21.3333L 12.8027,21.3333L 12.8027,19.1973L 28.2653,3.73468L 30.4013,5.86401"
            };

            _functionItemList.Add(m);

            BottomFunctionItemList = new ObservableCollection<MenuListItem>();

            m = new MenuListItem
            {
                Name = "Settings",
                MenuF = MenuFunc.Settings,
                IsEnabled = false,
                Icon =
                    "M14.682,10.493 C12.398,10.493 10.546,12.344 10.546,14.628 C10.546,16.912 12.398,18.764 14.682,18.764 C16.966,18.764 18.818,16.912 18.818,14.628 C18.818,12.344 16.966,10.493 14.682,10.493 z M14.682,8.46201 C18.088,8.46201 20.849,11.223 20.849,14.628 C20.849,18.034 18.088,20.795 14.682,20.795 C11.276,20.795 8.515,18.034 8.515,14.628 C8.515,11.223 11.276,8.46201 14.682,8.46201 z M13.423,2.454 C13.135,3.083 12.67,3.702 12.227,4.922 C12.149,5.142 11.983,5.316 11.771,5.403 L10.084,6.104 C9.883,6.187 9.66,6.185 9.461,6.099 C8.243,5.57 6.8,4.978 6.15,4.758 L4.643,6.263 C4.886,6.917 5.494,8.299 6.038,9.466 C6.138,9.678 6.141,9.925 6.052,10.145 L5.352,11.837 C5.266,12.045 5.103,12.208 4.896,12.289 C3.671,12.772 3.054,13.143 2.431,13.447 L2.431,15.889 C3.06,16.179 3.669,16.623 4.888,17.066 C5.103,17.146 5.276,17.312 5.363,17.526 L6.059,19.217 C6.146,19.427 6.143,19.665 6.054,19.873 C5.532,21.077 4.951,22.499 4.723,23.159 L6.23,24.664 C6.875,24.426 8.275,23.808 9.459,23.258 C9.661,23.164 9.893,23.159 10.1,23.244 L11.793,23.945 C11.998,24.029 12.16,24.195 12.241,24.403 C12.726,25.631 13.118,26.261 13.423,26.881 L15.866,26.881 C16.152,26.254 16.618,25.633 17.061,24.411 C17.14,24.195 17.306,24.018 17.517,23.933 L19.21,23.231 C19.407,23.149 19.633,23.151 19.832,23.239 C21.049,23.766 22.487,24.359 23.139,24.582 L24.645,23.072 C24.404,22.421 23.792,21.038 23.249,19.875 C23.151,19.662 23.144,19.414 23.234,19.195 L23.931,17.502 C24.016,17.295 24.182,17.131 24.387,17.051 C25.613,16.567 26.231,16.191 26.858,15.889 L26.858,13.447 C26.224,13.159 25.617,12.718 24.399,12.276 C24.184,12.198 24.01,12.033 23.921,11.816 L23.224,10.12 C23.139,9.909 23.139,9.671 23.231,9.461 C23.752,8.26 24.333,6.839 24.562,6.176 L23.054,4.672 C22.408,4.909 21.007,5.527 19.824,6.078 C19.624,6.173 19.39,6.179 19.183,6.093 L17.491,5.394 C17.287,5.31 17.122,5.143 17.042,4.934 C16.559,3.706 16.17,3.077 15.866,2.454 z M13.062,0 L16.098,0 C16.676,0 16.844,0 18.42,3.99 L19.47,4.423 C22.708,2.924 23.098,2.924 23.267,2.924 C23.48,2.924 23.719,3.018 23.87,3.169 L26.023,5.313 C26.441,5.736 26.56,5.856 24.857,9.805 L25.28,10.835 C29.288,12.297 29.288,12.472 29.288,13.083 L29.288,16.123 C29.288,16.731 29.286,16.888 25.293,18.468 L24.871,19.495 C26.669,23.362 26.545,23.49 26.117,23.924 L23.963,26.076 C23.809,26.228 23.571,26.323 23.359,26.323 C23.205,26.323 22.802,26.323 19.507,24.899 L18.457,25.333 C16.991,29.337 16.82,29.337 16.226,29.337 L13.19,29.337 C12.611,29.337 12.444,29.337 10.868,25.35 L9.816,24.912 C6.575,26.415 6.186,26.415 6.019,26.415 C5.805,26.415 5.564,26.32 5.413,26.166 L3.266,24.017 C2.839,23.587 2.729,23.476 4.431,19.529 L4.007,18.505 C0,17.039 0,16.862 0,16.251 L0,13.21 C0,12.602 0,12.447 3.993,10.868 L4.417,9.842 C2.62,5.969 2.744,5.844 3.175,5.41 L5.326,3.262 C5.476,3.111 5.717,3.016 5.93,3.016 C6.085,3.016 6.491,3.016 9.786,4.438 L10.831,4.003 C12.297,0 12.483,0 13.062,0 z"
            };
            BottomFunctionItemList.Add(m);

            ImageList = new ObservableCollection<string>();
            LocalImageList = new ObservableCollection<LocalImage>();
            IsIconSaved = false;

            IsDesktopBackground = SettingsHelper.GetSetting<bool>(SettingKey.IsDesktopBackground.ToString());
        }

        private async void RegisterViewModels()
        {
            ViewModelLocator vmLocator = new ViewModelLocator();
            ImageSourceVm = vmLocator.ImageSourceVm;
            WeatherVm = vmLocator.WeatherVm;
            QuoteVm = vmLocator.QuoteVm;
            RssVm = vmLocator.RssVm;
            NoteVm = vmLocator.NoteVm;
            await WeatherVm.Init();
        }

        private async void RegisterBackgroundTask()
        {
            var exampleTaskName = "ChangeLockScreenTask";
            var taskRegistered = BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == exampleTaskName);

            if (!taskRegistered)
            {
                await BackgroundExecutionManager.RequestAccessAsync();
                TimeTrigger hourlyTrigger = new TimeTrigger(240, false);

                var builder = new BackgroundTaskBuilder
                {
                    Name = exampleTaskName,
                    TaskEntryPoint = "BackgroundLockChanger.BackgroundLockChanger"
                };

                builder.SetTrigger(hourlyTrigger);
                BackgroundTaskRegistration task = builder.Register();
                task.Completed += Task_Completed;
            }
        }

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            //ToastContent content = new ToastContent()
            //{
            //    Launch = "TuanTran",

            //    Visual = new ToastVisual()
            //    {
            //        TitleText = new ToastText()
            //        {
            //            Text = "Task completed"
            //        },

            //        //BodyTextLine1 = new ToastText()
            //        //{
            //        //    Text = "NotificationsExtensions is great!"
            //        //},

            //        //AppLogoOverride = new ToastAppLogo()
            //        //{
            //        //    Crop = ToastImageCrop.Circle,
            //        //    Source = new ToastImageSource("ms-appx:///Assets/Resources/Toast/new.png")
            //        //}
            //    },

            //    //Actions = new ToastActionsCustom()
            //    //{
            //    //    Inputs =
            //    //    {
            //    //        new ToastTextBox("tbReply")
            //    //        {
            //    //            PlaceholderContent = "Type a response"
            //    //        }
            //    //    },

            //    //    Buttons =
            //    //    {
            //    //        new ToastButton("reply", "reply")
            //    //        {
            //    //            ActivationType = ToastActivationType.Background,
            //    //            ImageUri = "Assets/QuickReply.png",
            //    //            TextBoxId = "tbReply"
            //    //        }
            //    //    }
            //    //},

            //    Audio = new ToastAudio()
            //    {
            //        Src = new Uri("ms-winsoundevent:Notification.IM")
            //    }
            //};

            //XmlDocument doc = content.GetXml();

            //// Generate WinRT notification
            //var toast = new ToastNotification(doc)
            //{
            //    ExpirationTime = DateTime.Now.AddDays(1),
            //    Tag = "1",
            //    Group = "database"
            //};

            //ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public async void UpdateListAsync()
        {
            await UpdateListTask();
        }

        public async Task UpdateListTask()
        {
            if (IsDrawing) return;
            IsDrawing = true;
            int imageService = SettingManager.GetImageService();

            Debug.WriteLine("Update List Task");
            //if (IsImageUpdating)
            //{
            //    _updateImageTimer.Start();
            //    return;
            //}
            IsImageUpdating = true;
            IsImageSaved = false;

            if (StaticData.IsImageServiceChanged || ImageList.Count == 0)
            {
                if (imageService != 2)
                {
                    var networkHelper = new NetworkHelper();
                    
                    //Check Internet connection with Bing & Flickr image service
                    if (!networkHelper.HasInternetAccess)
                    {
                        var dialog = new Windows.UI.Popups.MessageDialog("There is no Internet connection. Try again with your own image & offline functions :D");
                        await dialog.ShowAsync();

                        IsDrawing = false;
                        IsImageUpdating = false;

                        return;
                    }
                }

                await UpdateImageList();
            }

            var fileName = imageService == 2 ? LocalImageList[0].File.Name : ImageList[0].Split('/').Last();

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            _bitmap = await CreateResources(device, fileName);
            var generateResult = await GenerateImage(device, _bitmap);

            if (generateResult)
            {
                StorageFolder readyFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Ready");
                StorageFile readyFile = await readyFolder.GetFileAsync(fileName);

                PreviewImage = new BitmapImage(new Uri(readyFile.Path));

                IsImageSaved = true;
                IsDrawing = false;
                IsImageUpdating = false;
            }
        }

        public async Task UpdateImageList()
        {
            Debug.WriteLine("Update Image List");
            ImageList.Clear();
            int imageService = SettingManager.GetImageService();
            switch (imageService)
            {
                case 0:
                {
                    //Bing
                    PreviewImage = null;
                    var v = Application.Current.Resources["Locator"] as ViewModelLocator;
                    if (v != null)
                    {
                        await v.ImageSourceVm.BingSettingVm.GetImageRoot();

                        BingHelper b = new BingHelper();

                        foreach (BingImage bingImage in v.ImageSourceVm.BingSettingVm.BingImageRoot.images)
                        {
                            string link = b.GenerateImageLink(bingImage.urlbase);
                            ImageList.Add(link);
                        }
                    }

                    await HttpService.DownloadImage(ImageList[0]);

                    _currentImageListService = 0;

                    break;
                }
                case 1:
                {
                    //Flickr
                    PreviewImage = null;
                    var v = Application.Current.Resources["Locator"] as ViewModelLocator;
                    if (v != null)
                    {
                        await v.ImageSourceVm.FlickrSettingVm.GetImageRoot();
                        FlickrHelper f = new FlickrHelper();
                        foreach (Photo p in v.ImageSourceVm.FlickrSettingVm.FlickrImageRoot.photos.photo)
                        {
                            ImageList.Add(await f.GenerateImageLink(p));
                        }
                    }

                    await HttpService.DownloadImage(ImageList[0]);

                    _currentImageListService = 1;

                    break;
                }
                case 2:
                {
                    //My Folder
                    PreviewImage = null;
                    var v = Application.Current.Resources["Locator"] as ViewModelLocator;
                    if (v != null)
                    {
                        await v.ImageSourceVm.MyFolderSettingVm.GetFolder();
                        if (v.ImageSourceVm.MyFolderSettingVm.MyFolderImageRoot != null)
                        {
                            LocalImageList.Clear();
                            foreach (
                                StorageFile file in
                                v.ImageSourceVm.MyFolderSettingVm.MyFolderImageRoot.Where(file => file != null))
                            {
                                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                                {
                                    // Set the image source to the selected bitmap 
                                    WriteableBitmap wbm = new WriteableBitmap(1, 1);
                                    await wbm.SetSourceAsync(fileStream);

                                    LocalImage l = new LocalImage
                                    {
                                        Bitmap = wbm,
                                        File = file
                                    };

                                    LocalImageList.Add(l);
                                }
                            }
                        }
                    }

                    _currentImageListService = 2;

                    break;
                }
            }

            StaticData.IsImageServiceChanged = false;
        }

        /// <summary>
        /// Load Image Resource
        /// </summary>
        /// <param name="device"></param>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        private async Task<CanvasBitmap> CreateResources(CanvasDevice device, string fileName)
        {
            BackgroundFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Background",
                CreationCollisionOption.OpenIfExists);

            int imageService = SettingManager.GetImageService();

            if (imageService != 2 && StorageHelper.IsFileExisted(BackgroundFolder, fileName))
            {
                BackgroundFile = await BackgroundFolder.GetFileAsync(fileName);
                using (IRandomAccessStream stream = await BackgroundFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    _bitmap = await CanvasBitmap.LoadAsync(device, stream);
                }
            }
            else
            {
                MyFolderSettingViewModel vm = new MyFolderSettingViewModel();
                await vm.GetFolder();
                BackgroundFile = vm.MyFolderImageRoot[0];
                using (IRandomAccessStream stream = await BackgroundFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    _bitmap = await CanvasBitmap.LoadAsync(device, stream);
                }
            }
            return _bitmap;
        }

        public async Task<bool> ChangeCurrentBackgroundTask()
        {
            //Get image service
            string imageLink = "";

            int imageService = SettingManager.GetImageService();
            switch (imageService)
            {
                case 0:
                {
                    //Bing Image
                    ImageSourceVm.BingSettingVm.LanguageCode = SettingManager.BingGetLanguage();

                    await ImageSourceVm.BingSettingVm.GetImageRoot();
                    BingHelper b = new BingHelper();
                    imageLink = b.GenerateImageLink(ImageSourceVm.BingSettingVm.BingImageRoot.images[0].urlbase);
                    break;
                }
                case 1:
                {
                    //Flickr
                    await ImageSourceVm.FlickrSettingVm.GetImageRoot();

                    FlickrHelper f = new FlickrHelper();
                    imageLink = await f.GenerateImageLink(ImageSourceVm.FlickrSettingVm.FlickrImageRoot.photos.photo[0]);
                    break;
                }
                case 2:
                {
                    //Local folder
                    break;
                }
            }
            //Set lockscreen
            bool success = false;
            bool desktopSuccess = false;

            // download image from uri into temp storagefile
            if (imageService != 2)
            {
                await HttpService.DownloadImage(imageLink);
            }

            //Draw on Image
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            var bitmap = await CreateResources(device, imageLink.Split('/').Last());
            var generateResult = await GenerateImage(device, bitmap, true);

            if (generateResult)
            {
                //Test commit
                StorageFolder readyFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Ready");
                StorageFile readyFile = await readyFolder.GetFileAsync(BackgroundFile.Name);

                if (UserProfilePersonalizationSettings.IsSupported())
                {
                    success = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(readyFile);
                    if (IsDesktopBackground)
                    {
                        desktopSuccess =
                            await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(readyFile);
                    }
                    else
                    {
                        desktopSuccess = true;
                    }                  
                }
            }

            return success & desktopSuccess;
        }

        public async Task UpdatePreviewImageTask()
        {
            int imageService = SettingManager.GetImageService();

            //Update ImageList when ImageService is Flickr
            if (_currentImageListService == 1)
            {
                await UpdateImageList();
            }
            else
            {
                if (imageService == 0)
                {
                    if (ImageList.Count == 0) await UpdateImageList();
                }
                else
                {
                    if (LocalImageList.Count == 0) await UpdateImageList();
                }
            }

            if (ImageList.Count == 0 && imageService != 2)
            {
                return;
            }

            if (LocalImageList.Count == 0 && imageService == 2)
            {
                return;
            }

            //This parameter is unuse when ImageService == 2
            var fileName = imageService == 2 ? LocalImageList[0].File.Name : ImageList[0].Split('/').Last();

            //Draw on Image
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            var bitmap = await CreateResources(device, fileName);
            await GenerateImage(device, bitmap);

            StorageFolder readyFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Ready");
            StorageFile readyFile = await readyFolder.GetFileAsync(fileName);

            PreviewImage = null;
            PreviewImage = new BitmapImage(new Uri(readyFile.Path));
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(PreviewImage));
        }

        public async Task<bool> GenerateImage(CanvasDevice device, CanvasBitmap bitmap, bool isBackground = false)
        {
            //count++;

            var size = SettingManager.GetWindowsResolution();

            var drawPosition = SettingManager.GetDrawPosition();

            //Generate images
            RenderTarget = new CanvasRenderTarget(device, (float) size.Width, (float) size.Height, 96);

            using (var ds = RenderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);

                ////Select source rectangle
                //double newX = 0, newY = 0;
                //var newWidth = bitmap.Size.Width;
                //var newHeight = bitmap.Size.Height;
                //newWidth = newHeight < newWidth ? newHeight * (size.Width / size.Height) : newWidth;
                //newHeight = newHeight > newWidth ? newWidth * (size.Width / size.Height) : newHeight;
                //if (newWidth != bitmap.Size.Width)
                //{
                //    newX = Math.Abs(bitmap.Size.Width - newWidth) / 2;
                //}
                //if (newHeight != bitmap.Size.Height)
                //{
                //    newY = Math.Abs(bitmap.Size.Height - newHeight) / 2;
                //}

                //Draw the Image
                //ds.DrawImage(bitmap, new Rect(0, 0, size.Width, size.Height), new Rect(newX, newY, newWidth, newHeight), 100 ,CanvasImageInterpolation.Linear);
                ds.DrawImage(bitmap, new Rect(0, 0, size.Width, size.Height));

                ImageSourceVm.DrawInfo(ds, device);

                var oldPoint = new Point();
                var drawPoint = new Point();
                var area = drawPosition.Split('|');
                for (int i = 0; i < area.Length; i++)
                {
                    if (area[i] != "")
                    {
                        var tempArr = area[i].Split('-');
                        double temp;
                        if (double.TryParse(tempArr[1], out temp)) drawPoint.Y = temp;
                        else return false;
                        foreach (var ch in tempArr[0])
                        {                          
                            switch (ch)
                            {
                                case 'W':
                                    drawPoint.X = BitmapHelper.ElementX(i, size.Width);
                                    oldPoint.X = drawPoint.X;
                                    oldPoint.Y = drawPoint.Y;
                                    drawPoint =
                                        await WeatherVm.DrawWeather(ds, device, bitmap, drawPoint, isBackground);
                                    if ((drawPoint.X == -1) && (drawPoint.Y == -1))
                                    {
                                        IsDrawing = false;
                                        return false;
                                    }
                                    if (oldPoint != drawPoint) drawPoint.Y += drawPoint.Y < size.Height ? size.Height * 2 /100 : 0;
                                    break;
                                case 'R':
                                    drawPoint.X = BitmapHelper.ElementX(i, size.Width);
                                    oldPoint.X = drawPoint.X;
                                    oldPoint.Y = drawPoint.Y;
                                    drawPoint = await RssVm.DrawRss(ds, device, bitmap, drawPoint);
                                    if ((drawPoint.X == -1) && (drawPoint.Y == -1))
                                    {
                                        IsDrawing = false;
                                        return false;
                                    }
                                    if (oldPoint != drawPoint) drawPoint.Y += drawPoint.Y < size.Height ? size.Height * 1.5 / 100 : 0;
                                    break;
                                case 'Q':
                                    drawPoint.X = BitmapHelper.ElementX(i, size.Width);
                                    oldPoint.X = drawPoint.X;
                                    oldPoint.Y = drawPoint.Y;
                                    drawPoint = await QuoteVm.DrawQuote(ds, device, bitmap, drawPoint);
                                    if ((drawPoint.X == -1) && (drawPoint.Y == -1))
                                    {
                                        IsDrawing = false;
                                        return false;
                                    }
                                    if (oldPoint != drawPoint) drawPoint.Y += drawPoint.Y < size.Height ? size.Height * 1.5 / 100 : 0;
                                    break;
                                case 'N':
                                    drawPoint.X = BitmapHelper.ElementX(i, size.Width);
                                    oldPoint.X = drawPoint.X;
                                    oldPoint.Y = drawPoint.Y;
                                    drawPoint = NoteVm.DrawNote(ds, device, bitmap, drawPoint);
                                    if ((drawPoint.X == -1) && (drawPoint.Y == -1))
                                    {
                                        IsDrawing = false;
                                        return false;
                                    }
                                    if (oldPoint != drawPoint) drawPoint.Y += drawPoint.Y < size.Height ? size.Height * 1.5 / 100 : 0;
                                    break;
                            }
                        }
                    }
                }
            }


            StorageFolder readyFolder =
                await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync("Ready", CreationCollisionOption.OpenIfExists);
            var fileList = await readyFolder.GetFilesAsync();
            foreach (StorageFile storageFile in fileList)
            {
                if (StorageHelper.IsFileExisted(readyFolder, storageFile.Name))
                {
                    await storageFile.DeleteAsync(StorageDeleteOption.Default);
                }
            }

            await RenderTarget.SaveAsync(
                Path.Combine(readyFolder.Path, BackgroundFile.Name), 
                CanvasBitmapFileFormat.Auto,
                1);
            IsImageSaved = true;

            StartPage?.PreviewImageInvalidate();
            return true;
        }

        public void NavigateWebView(string link)
        {
            IsShowWebView = true;
            StartPage.NavigateWebView(link);
        }

        #region ViewModel Control Event

        public void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            IsSplitViewPaneOpened = !IsSplitViewPaneOpened;
        }

        public void ControlPanelMoreSettingButton_Click(object sender, RoutedEventArgs e)
        {
            MenuFunc m = (MenuFunc)((AppBarButton)sender).Tag;
            StartPage.NavigateToPage(m);
        }

        public async void ToggleDesktopBackground(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (IsDesktopBackground != toggleSwitch.IsOn)
            {
                await UpdateListTask();
            }
        }

        public async void ToggleWeather(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (WeatherVm.IsShowWeather != toggleSwitch.IsOn)
            {
                await UpdateListTask();
            }
        }

        public async void ToggleNote(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (NoteVm.IsDisplayNote != toggleSwitch.IsOn)
            {
                await UpdateListTask();
            }
        }

        public async void ToggleQuote(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (QuoteVm.IsDisplayQuote != toggleSwitch.IsOn)
            {
                await UpdateListTask();
            }
        }
        public async void ToggleRss(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (RssVm.IsEnabled != toggleSwitch.IsOn)
            {
                await UpdateListTask();
            }
        }

        #endregion

    }
}