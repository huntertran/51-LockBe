using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Mntone.SvgForXaml;
using ShareClass.Model;
using ShareClass.Model.GoogleMap;
using ShareClass.Model.Weather;
using ShareClass.Utilities.CallApi;
using ShareClass.Utilities.Helpers;
using ShareClass.Utilities.Helpers.SourceDataHelper;
using ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup;
using ShareClass.ViewModel.StartGroup;

namespace ShareClass.ViewModel.WeatherGroup
{
    public class WeatherViewModel : ObservableObject
    {
        public StartViewModel StartVm => ((ViewModelLocator)Application.Current.Resources["Locator"]).StartVm;

        private readonly WeatherApi _api = new WeatherApi();
        private readonly GoogleMapApi _googleMapApi = new GoogleMapApi();

        public readonly Dictionary<string, string> WeatherKinds = new Dictionary<string, string>
        {
            {"clouds", "032cloud"},
            {"haze", "614haze"},
            {"snow", "439snow"},
            {"windy", "516windy"},
            {"clear,sky", "220clearsky"},
            {"heavy,rain,thunderstorm", "343heavyrain"},
            {"light,rain", "032lightrain"},
            {"moderate,rain", "032mediumrain"},
            {"light,snow", "022snowrain"},
            {"mist,fog", "213mist"},
            {"cold,windy", "439coldwind"},
            {"few,clouds,broken", "614fewcloud"}
        };

        private CitiWeather _currentWeather;
        private WeatherInfo _currentWeatherInfo;

        private Point _drawPoint;

        private Geocode _geoLocation = new Geocode
        {
            Results = new List<Result>()
        };

        private bool _isShowWeather;
        private bool _isShowProgress;
        private Geoposition _pos;
        private bool _isFahrenheit;

        public Point DrawPoint
        {
            get { return _drawPoint; }
            set
            {
                if (Equals(value, _drawPoint)) return;
                _drawPoint = value;
                OnPropertyChanged();
            }
        }
        
        public Geocode GeoLocation
        {
            get { return _geoLocation; }
            set
            {
                if (Equals(value, _geoLocation)) return;
                _geoLocation = value;
                UpdateAddress();
                OnPropertyChanged();
            }
        }

        public Geoposition Pos
        {
            get { return _pos; }
            set
            {
                if (Equals(value, _pos)) return;
                _pos = value;
                UpdateGeolocation(_pos);
                OnPropertyChanged();
            }
        }

        public bool IsShowWeather
        {
            get { return _isShowWeather; }
            set
            {
                if (value == _isShowWeather) return;
                _isShowWeather = value;
                SettingsHelper.SetSetting(SettingKey.IsDisplayWeather.ToString(), _isShowWeather);
                OnPropertyChanged();
            }
        }

        public bool IsFahrenheit
        {
            get { return _isFahrenheit; }
            set
            {
                if (value == _isFahrenheit) return;
                _isFahrenheit = value;
                SettingsHelper.SetSetting(SettingKey.IsFahrenheit.ToString(), _isFahrenheit);
                OnPropertyChanged();
            }
        }

        public bool IsShowProgress
        {
            get { return _isShowProgress; }
            set
            {
                if (value == _isShowProgress) return;
                _isShowProgress = value;
                OnPropertyChanged();
            }
        }

        public WeatherInfo CurrentWeatherInfo
        {
            get { return _currentWeatherInfo; }
            set
            {
                if (Equals(value, _currentWeatherInfo)) return;
                _currentWeatherInfo = value;
                OnPropertyChanged();
            }
        }

        public CitiWeather CurrentWeather
        {
            get { return _currentWeather; }
            set
            {
                if (Equals(value, _currentWeather)) return;
                _currentWeather = value;
                OnPropertyChanged();
            }
        }

        #region Draw Position Area

        private ObservableCollection<ImageSourceItem> _positionItemsCollection;

        public ObservableCollection<ImageSourceItem> PositionItemsCollection
        {
            get { return _positionItemsCollection; }
            set
            {
                if (Equals(value, _positionItemsCollection)) return;
                _positionItemsCollection = value;
                OnPropertyChanged();
            }
        }

        private ImageSourceItem _selectedPosition;

        public ImageSourceItem SelectedPosition
        {
            get
            {
                return _selectedPosition;
            }
            set
            {
                if (Equals(value, _selectedPosition)) return;
                _selectedPosition = value;
                OnPropertyChanged();
            }
        }

        private void Initialize()
        {
            PositionItemsCollection = new ObservableCollection<ImageSourceItem>();

            PositionHelper.GetPositionItems(ref _positionItemsCollection);

            var number = PositionHelper.GetElementPosition("W");
            SelectedPosition = PositionItemsCollection[number] ?? PositionItemsCollection[0];
        }

        #endregion

        public async void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            SelectedPosition = (ImageSourceItem)comboBox.SelectedItem;

            var number = PositionHelper.GetElementPosition("W");
            if (SelectedPosition != null)
            {
                if (SelectedPosition.Number != number)
                {
                    PositionHelper.SetElementPosition("W", SelectedPosition.Number);
                    await StartVm.UpdateListTask();
                }                 
            }
        }



        public WeatherViewModel()
        {
            //Get Weather Setting
            IsShowWeather = SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayWeather.ToString());
            //Get Temp Unit Setting
            IsFahrenheit = SettingsHelper.GetSetting<bool>(SettingKey.IsFahrenheit.ToString());

            Initialize();
        }

        public async Task Init()
        {
            _currentWeatherInfo = new WeatherInfo();

            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    IsShowProgress = true;

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    var geolocator = new Geolocator();

                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    //_geolocator.StatusChanged += OnStatusChanged;

                    // Carry out the operation.
                    Pos = await geolocator.GetGeopositionAsync();
                    IsShowProgress = false;
                    //UpdateLocationData(_pos);
                    //_rootPage.NotifyUser("Location updated.", NotifyType.StatusMessage);
                    break;

                case GeolocationAccessStatus.Denied:
                    //TODO: Handle access denied
                    //_rootPage.NotifyUser("Access to location is denied.", NotifyType.ErrorMessage);
                    //LocationDisabledMessage.Visibility = Visibility.Visible;
                    //UpdateLocationData(null);
                    break;

                case GeolocationAccessStatus.Unspecified:
                    //TODO: Handle Unspecified
                    //_rootPage.NotifyUser("Unspecified error.", NotifyType.ErrorMessage);
                    //UpdateLocationData(null);
                    break;
            }
        }

        private async void UpdateGeolocation(Geoposition pos)
        {
            var b = new BasicGeoposition
            {
                Latitude = pos.Coordinate.Point.Position.Latitude,
                Longitude = pos.Coordinate.Point.Position.Longitude
            };
            GeoLocation = await _googleMapApi.GetAddressFromGpsTask(b);
            await GetWeather(false);
        }

        private void UpdateAddress()
        {
            CurrentWeatherInfo.Address = GeoLocation.Results[0].FormattedAddress;
        }

        public async Task GetWeather(bool isUseGps = true)
        {
            if (_pos != null && isUseGps)
            {
                //pos available
                var b = new BasicGeoposition
                {
                    Latitude = Pos.Coordinate.Point.Position.Latitude,
                    Longitude = Pos.Coordinate.Point.Position.Longitude
                };
                CurrentWeather = await _api.GetCityWeather(b);
            }
            else
            {
                if (GeoLocation != null && GeoLocation.Results.Any())
                {
                    var b = new BasicGeoposition
                    {
                        Latitude = GeoLocation.Results[0].Geometry.Location.Lat,
                        Longitude = GeoLocation.Results[0].Geometry.Location.Lng
                    };
                    CurrentWeather = await _api.GetCityWeather(b);
                }
            }
        }

        public async Task ShowWeather()
        {
            var vm = new BingSettingViewModel
            {
                LanguageCode = SettingManager.BingGetLanguage()
            };
            await vm.GetImageRoot();

            //TODO: Thi: Explain this line of code
            vm.BingImageRoot.images[0] = vm.BingImageRoot.images[1];
        }

        public async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            GeoLocation = await _googleMapApi.GetGpsFromAddressTask(sender.Text);
        }

        public string PickWeatherIcon(string weatherCondition)
        {
            var maxCorrect = 0;
            var resultKeyword = "";
            //var keyValues = new List<KeyValuePair<string, string>>();
            foreach (var objct in WeatherKinds)
            {
                var keywords = objct.Key.Split(',');
                var count =
                    keywords.Count(keyword => weatherCondition.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1);
                if (count > maxCorrect)
                {
                    maxCorrect = count;
                    resultKeyword = objct.Key;
                }
            }
            if (resultKeyword != "") return WeatherKinds[resultKeyword];
            return "614fewcloud";
        }

        public async Task<Point> DrawWeather(CanvasDrawingSession ds, CanvasDevice device, CanvasBitmap canvasBitmap,Point drawPoint, bool isBackground  = false)
        {

            #region Init & Get Weather Informations

            Size screenSize = SettingManager.GetWindowsResolution();
            var width = screenSize.Width;
            var height = screenSize.Height;
            float space = (float) width - (float) drawPoint.X;
            Rect textSize;
            var newWidth = width - space;
            double tempWidth = 0;
            var oldDrawPoint = drawPoint.Y;

            if (drawPoint.Y >= height)
            {
                drawPoint.Y = drawPoint.Y > height ? drawPoint.Y - height - height*22/100 : height - height*25/100;
            }

            //if (!SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayWeather.ToString()))
            //{
            //    return new Point(0,0);
            //}

            if (!SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayWeather.ToString()))
            {
                return drawPoint;
            }

            if (CurrentWeatherInfo.Temp == null)
            {
                await GetWeather();
            }
            var address = CurrentWeatherInfo.Address;

            if (CurrentWeather?.Main == null) return new Point(-1, -1);

            var isFahrenheit = SettingsHelper.GetSetting<bool>(SettingKey.IsFahrenheit.ToString());
            if (isFahrenheit)
            {
                CurrentWeatherInfo.Temp = Math.Round(CurrentWeather.Main.Temp*1.8 + 32) + "°F";
            }
            else
            {
                CurrentWeatherInfo.Temp = Math.Round(CurrentWeather.Main.Temp) + "°C";
            }
            CurrentWeatherInfo.MainCondition = CurrentWeather.Weather[0].Main;
            CurrentWeatherInfo.Condition = CurrentWeather.Weather[0].Description;

            #endregion

            #region Get Address & Caculate area size, position

            if (address != null)
            {
                var strArr = address.Split(',');
                CurrentWeatherInfo.Address = string.Format("{0},{1}", strArr[strArr.Length - 2],
                    strArr[strArr.Length - 1]);

                var textFormat = new CanvasTextFormat
                {
                    FontFamily = "Segoe UI Light",
                    FontSize = (float) (height*4/100),
                    WordWrapping = CanvasWordWrapping.NoWrap
                };

                textSize = BitmapHelper.TextRect(CurrentWeatherInfo.Address, textFormat, ds);           

                //Caculate WeatherCondition text length for drawing AntiBright & Weather
                var conditionSize = BitmapHelper.TextRect(CurrentWeatherInfo.MainCondition, new CanvasTextFormat()
                {
                    FontSize = (float)(height * 4.5 / 100),
                    FontFamily = "Segoe UI Light",
                    FontWeight = FontWeights.Thin,
                    WordWrapping = CanvasWordWrapping.NoWrap
                }, ds);

                var tempSize = BitmapHelper.TextRect(CurrentWeatherInfo.Temp, new CanvasTextFormat()
                {
                    FontSize = (float)(height * 7.5 / 100),
                    WordWrapping = CanvasWordWrapping.NoWrap,
                    FontFamily = "Segoe UI Light",
                    FontWeight = FontWeights.Thin
                }, ds);

                //Check if CenterX + WeatherCondition(or Temperature if Temp>WeatherCon) > Address to update CenterX(use for draw WeatherCondition)
                tempWidth = tempSize.Width > conditionSize.Width ? tempSize.Width : conditionSize.Width;

                DrawPoint = new Point(newWidth - textSize.Width, drawPoint.Y);

                if (DrawPoint.X + textSize.Width + height/100 >= newWidth)
                {
                    newWidth -= height/100;
                    DrawPoint = new Point(newWidth - textSize.Width, drawPoint.Y);
                }

                if (drawPoint.X == 0)
                {
                    newWidth = textSize.Width > tempWidth + height * 15 / 100 + width / 100 ? textSize.Width + height * 3 / 100 : tempWidth + height * 15 / 100 + width / 100 + height * 3/ 100;

                    DrawPoint = new Point(newWidth - textSize.Width, drawPoint.Y);
                }


                //Check if WeatherIcon + TempWidth > Address to update DrawPoint
                if (DrawPoint.X + tempWidth + height * 15 / 100 + width / 100 + height/100 >= newWidth)
                {
                    var x = DrawPoint.X - (tempWidth + DrawPoint.X + width / 100 + height * 15 / 100 + height / 100 - newWidth);
                    DrawPoint = new Point(x, drawPoint.Y);
                }

                if (Math.Abs(drawPoint.X - width / 2) < 0.5)
                {
                    var leftToCenter = width/2 - DrawPoint.X + height*2/100;
                    var haftWidth = (newWidth - DrawPoint.X + height * 2 / 100) / 2;
                    var tempSpace = leftToCenter - haftWidth;
                    DrawPoint = new Point(DrawPoint.X + tempSpace, DrawPoint.Y);
                    newWidth += tempSpace;
                }


            }

            #endregion

            #region Draw Methods

            if ((DrawPoint.X == 0) && (DrawPoint.Y == 0)) return new Point(-1, -1);
            {
                //Check and draw transparent black rectangle if necessary
                //TODO:Change draw rectangle method
                if (BitmapHelper.IsBrightArea(canvasBitmap, 
                    (int) (DrawPoint.X - height*2/100),
                    (int) (DrawPoint.Y - width/100),
                    (int) (newWidth - DrawPoint.X + height * 2 / 100), 
                    (int) height*22/100))
                {
                    ds.FillRoundedRectangle(
                        new Rect(DrawPoint.X - height*2/100, 
                                 (int) DrawPoint.Y - width/100,
                                 newWidth - DrawPoint.X + height*2/100,
                                 height*22/100), 20, 20,
                        new CanvasSolidColorBrush(device, Colors.Black) {Opacity = 0.4F});
                }

                if ((CurrentWeatherInfo.Address == null) && (CurrentWeatherInfo.Address == ""))
                {
                    return new Point(-1, -1);
                }

                var strArr = CurrentWeatherInfo.Address?.Split(',');
                if (strArr?.Length > 2)
                {
                    CurrentWeatherInfo.Address = strArr[strArr.Length - 2] + "," + strArr[strArr.Length - 1];
                }

                if (CurrentWeatherInfo.Address != null)
                {
                    ds.DrawText(CurrentWeatherInfo.Address, (float) (newWidth - height*1.5/100), (float) DrawPoint.Y,
                        Colors.White,
                        new CanvasTextFormat
                        {
                            FontSize = (float) (height*4/100),
                            HorizontalAlignment = CanvasHorizontalAlignment.Right,
                            FontFamily = "Segoe UI Light",
                            FontWeight = FontWeights.Thin
                        });
                }

                var centerX = (float)(DrawPoint.X + (newWidth - DrawPoint.X) / 2);

                //Config Area position for better Draw design
                if (tempWidth + centerX + width*2 / 100 >= newWidth)
                {
                    centerX -= (float)tempWidth + centerX + (float) (width*2 / 100) - (float)newWidth;
                }
                else
                {
                    if ((tempWidth + height*15/100 >= textSize.Width) &&
                        (tempWidth + height*15/100 + (newWidth - (float) width*2/100) - textSize.Width) <= newWidth)
                    {
                        centerX = (float) ((newWidth - (float) width*2/100) - textSize.Width);
                    }
                }

                if (CurrentWeatherInfo.Temp != null)
                {
                    ds.DrawText(CurrentWeatherInfo.Temp, centerX, (float) (DrawPoint.Y + height*4.5/100), Colors.White,
                        new CanvasTextFormat
                        {
                            FontSize = (float) (height*7.5/100),
                            HorizontalAlignment = CanvasHorizontalAlignment.Left,
                            FontFamily = "Segoe UI Light",
                            FontWeight = FontWeights.Thin
                        });
                }

                if (CurrentWeatherInfo.MainCondition != null)
                {
                    ds.DrawText(CurrentWeatherInfo.MainCondition, centerX, (float)(DrawPoint.Y + height * 13 / 100),
                        Colors.White,
                        new CanvasTextFormat
                        {
                            FontSize = (float)(height * 4.5 / 100),
                            HorizontalAlignment = CanvasHorizontalAlignment.Left,
                            FontFamily = "Segoe UI Light",
                            FontWeight = FontWeights.Thin
                        });
                }

                if (CurrentWeatherInfo.Condition != null)
                {
                    if (isBackground)
                    {
                        var weatherIcon = PickWeatherIcon(CurrentWeatherInfo.Condition);
                        var iconSizeStr = weatherIcon.Substring(0, 3);
                        var iconSize = int.Parse(iconSizeStr);
                        var iconBitmap = new CanvasRenderTarget(device, iconSize, iconSize, 500);
                        using (var ds1 = iconBitmap.CreateDrawingSession())
                        {
                            var file =
                                await
                                    StorageFile.GetFileFromApplicationUriAsync(
                                        new Uri("ms-appx:///ShareClass/Assets/WeatherIcon/" + weatherIcon + ".svg"));
                            using (var stream = await file.OpenStreamForReadAsync())
                            using (var reader = new StreamReader(stream))
                            {
                                var xml = new XmlDocument();
                                xml.LoadXml(reader.ReadToEnd(), new XmlLoadSettings {ProhibitDtd = false});

                                var svgDocument = SvgDocument.Parse(xml);


                                using (var renderer = new Win2dRenderer(iconBitmap, svgDocument))
                                    renderer.Render(iconSize, iconSize, ds1);

                                ds.DrawImage(iconBitmap, new Rect(centerX - height*15/100,
                                    DrawPoint.Y + height*5.5/100, height*13/100, height*13/100));
                            }
                        }
                    }
                    else
                    {
                        if (!StartVm.IsIconSaved)
                        {
                            var weatherIcon = PickWeatherIcon(CurrentWeatherInfo.Condition);
                            var iconSizeStr = weatherIcon.Substring(0, 3);
                            var iconSize = int.Parse(iconSizeStr);
                            var iconBitmap = new CanvasRenderTarget(device, iconSize, iconSize, 500);
                            using (var ds1 = iconBitmap.CreateDrawingSession())
                            {
                                var file =
                                    await
                                        StorageFile.GetFileFromApplicationUriAsync(
                                            new Uri("ms-appx:///ShareClass/Assets/WeatherIcon/" + weatherIcon + ".svg"));
                                using (var stream = await file.OpenStreamForReadAsync())
                                using (var reader = new StreamReader(stream))
                                {
                                    var xml = new XmlDocument();
                                    xml.LoadXml(reader.ReadToEnd(), new XmlLoadSettings { ProhibitDtd = false });

                                    var svgDocument = SvgDocument.Parse(xml);


                                    using (var renderer = new Win2dRenderer(iconBitmap, svgDocument))
                                        renderer.Render(iconSize, iconSize, ds1);

                                    StartVm.IconBitmap = new CanvasRenderTarget(device, iconSize, iconSize, 500);
                                    StartVm.IconBitmap = iconBitmap;
                                    StartVm.IsIconSaved = true;

                                    ds.DrawImage(iconBitmap, new Rect(centerX - height * 15 / 100,
                                        DrawPoint.Y + height * 5.5 / 100, height * 13 / 100, height * 13 / 100));
                                }
                            }

                        }
                        else
                        {
                            ds.DrawImage(StartVm.IconBitmap,
                            new Rect(centerX - height * 15 / 100, DrawPoint.Y + height * 5.5 / 100, height * 13 / 100, height * 13 / 100));
                        }
                    }                  
                    
                }

                //ds.DrawText(count.ToString(), 0, 0,
                //       Colors.Red,
                //       new CanvasTextFormat
                //       {
                //           FontSize = (float)(height * 4.5 / 100),
                //           HorizontalAlignment = CanvasHorizontalAlignment.Left,
                //       });

                if (oldDrawPoint >= screenSize.Height)
                {
                    var temp = DrawPoint.Y - width / 100;
                    if (temp + screenSize.Height <= screenSize.Height * 2) return new Point(drawPoint.X, temp + screenSize.Height);
                    return new Point(-1, -1);
                }
                else
                {
                    var temp = DrawPoint.Y - width / 100 + height * 22 / 100;
                    if (temp <= screenSize.Height) return new Point(drawPoint.X, temp);
                    return new Point(-1, -1);
                }
            }

            #endregion
        }

        public async Task<CanvasRenderTarget> GenerateWeatherIcon(CanvasDrawingSession ds, double height)
        {           
            CanvasDevice device1 = CanvasDevice.GetSharedDevice();
            var weatherIcon = PickWeatherIcon(CurrentWeatherInfo.Condition);
            var iconSizeStr = weatherIcon.Substring(0, 3);
            var iconSize = int.Parse(iconSizeStr);
            var iconBitmap = new CanvasRenderTarget(device1, iconSize, iconSize, 500);
            using (var ds1 = iconBitmap.CreateDrawingSession())
            {
                var file =
                    await
                        StorageFile.GetFileFromApplicationUriAsync(
                            new Uri("ms-appx:///ShareClass/Assets/WeatherIcon/" + weatherIcon + ".svg"));
                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(reader.ReadToEnd(), new XmlLoadSettings { ProhibitDtd = false });

                    var svgDocument = SvgDocument.Parse(xml);


                    using (var renderer = new Win2dRenderer(iconBitmap, svgDocument))
                        renderer.Render(iconSize, iconSize, ds1);

                    ds.DrawText(svgDocument.RootElement.FirstChild.TagName, 40, 0,
                      Colors.Red,
                      new CanvasTextFormat
                      {
                          FontSize = (float)(height * 2.5 / 100),
                          HorizontalAlignment = CanvasHorizontalAlignment.Left,
                          FontFamily = "Segoe UI",
                      });

                    return iconBitmap;
                }
            }
        }

        public async void ToggleWeather(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (IsShowWeather != toggleSwitch.IsOn)
            {
                await StartVm.UpdateListTask();
            }
        }

        public async void ToggleTemperature(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (IsFahrenheit != toggleSwitch.IsOn)
            {
                await StartVm.UpdateListTask();
            }
        }
    }


    public enum UnitFormat
    {
        //&units=metric
        //default
        Kevin,
        //imperal
        Fahrenheit,
        //metric
        Celcius
    }
}