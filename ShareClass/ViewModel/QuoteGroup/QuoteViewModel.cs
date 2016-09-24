using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using ShareClass.Model;
using ShareClass.Model.Qoute;
using ShareClass.Utilities.CallApi;
using ShareClass.Utilities.Helpers;
using ShareClass.Utilities.Helpers.SourceDataHelper;
using ShareClass.ViewModel.StartGroup;

namespace ShareClass.ViewModel.QuoteGroup
{
    public class QuoteViewModel : ObservableObject
    {
        private Quote _quote;
        private bool _isDisplayQuote;
        private bool _isOfflineQuote;
        private string _offlineQuote;
        private string _offlineAuthor;

        public StartViewModel StartVm => ((ViewModelLocator) Application.Current.Resources["Locator"]).StartVm;

        public Quote Quote
        {
            get { return _quote; }
            set
            {
                if (Equals(value, _quote)) return;
                _quote = value;
                OnPropertyChanged();
            }
        }

        public bool IsDisplayQuote
        {
            get { return _isDisplayQuote; }
            set
            {
                if (value == _isDisplayQuote) return;
                _isDisplayQuote = value;
                SettingsHelper.SetSetting(SettingKey.IsDisplayQuote.ToString(), _isDisplayQuote);
                OnPropertyChanged();
            }
        }

        public bool IsOfflineQuote
        {
            get { return _isOfflineQuote; }
            set
            {
                if (value == _isOfflineQuote) return;
                _isOfflineQuote = value;
                SettingsHelper.SetSetting(SettingKey.IsOfflineQuote.ToString(), _isOfflineQuote);
                OnPropertyChanged();
            }
        }

        public string OfflineQuote
        {
            get { return _offlineQuote; }
            set
            {
                if (value == _offlineQuote) return;
                _offlineQuote = value;
                OnPropertyChanged();
            }
        }

        public string OfflineAuthor
        {
            get { return _offlineAuthor; }
            set
            {
                if (value == _offlineAuthor) return;
                _offlineAuthor = value;
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

            var number = PositionHelper.GetElementPosition("Q");
            SelectedPosition = PositionItemsCollection[number] ?? PositionItemsCollection[0];
        }

        public async void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            SelectedPosition = (ImageSourceItem)comboBox.SelectedItem;

            var number = PositionHelper.GetElementPosition("Q");
            if (SelectedPosition != null)
            {
                if (SelectedPosition.Number != number)
                {
                    PositionHelper.SetElementPosition("Q", SelectedPosition.Number);
                    await StartVm.UpdateListTask();
                }              
            }
        }

        #endregion

        public QuoteViewModel()
        {
            _quote = new Quote();

            // Get Quote Settings
            IsDisplayQuote = SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayQuote.ToString());
            IsOfflineQuote = SettingsHelper.GetSetting<bool>(SettingKey.IsOfflineQuote.ToString());
            Initialize();
            if (SettingManager.GetOfflineQuote() != null)
            {
                var strArr = SettingManager.GetOfflineQuote().Split('|');
                if (!strArr.Any()) return;
                OfflineQuote = strArr[0];
                OfflineAuthor = strArr[1];
            }
        }

        public async Task GetRandomQuote()
        {
            var quoteApi = new QuoteApi();
            var quoteOfDay = await quoteApi.GetQuoteOfDay();
            Quote.quote = "\"" + quoteOfDay.quote + "\"";
            //_drawInfo.QouteOfDay =
            //   "\"You know what it’s like to wake up in the middle of the night with a vivid dream? And you know that if you don’t have a pencil and pad by the bed, it will be completely gone by the next morning. Sometimes it’s important to wake up and stop dreaming. When a really great dream shows up, grab it.\"";
            Quote.author = "- " + quoteOfDay.author;
        }

        public async Task<Point> DrawQuote(CanvasDrawingSession ds, CanvasDevice device, CanvasBitmap canvasBitmap, Point drawPoint)
        {
            //if (!SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayQuote.ToString()))
            //{
            //    return new Point(0,0);
            //}

            if (!SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayQuote.ToString()))
            {
                return drawPoint;
            }

            if (SettingsHelper.GetSetting<bool>(SettingKey.IsOfflineQuote.ToString()))
            {
                if (OfflineQuote == null) return new Point(0,0);
                Quote.quote = "\"" + OfflineQuote + "\"";
                Quote.author = "- " + OfflineAuthor;
            }
            else
            {
                await GetRandomQuote();
            }           

            Size screenSize = SettingManager.GetWindowsResolution();

            //Qoute rect coordinate
            //X: 5.5 of 10
            //Y: 8 of 10
            //width: 4.35 of 10
            //heigh: depend

            Rect quoteRect = new Rect(0, 0, 0, 0)
            {
                X = (screenSize.Width / 10) * 5.5,
                Width = (screenSize.Width / 10) * 4.35,
                Y = (screenSize.Height / 10) * 8
            };

            Rect creditRect = new Rect(0, 0, 0, 0);

            CanvasTextFormat textFormat = new CanvasTextFormat
            {
                WordWrapping = CanvasWordWrapping.WholeWord,
                FontFamily = "Segoe UI Light",
                FontSize = (float) (screenSize.Height*4.5/100)
            };

            Rect quoteSize = BitmapHelper.TextRect(Quote.quote, textFormat, ds, quoteRect.Width, 200);


            Rect creditSize = BitmapHelper.TextRect(Quote.author, textFormat, ds, quoteRect.Width);

            creditRect.X = quoteRect.X;
            var distanceToEnd = screenSize.Width - creditRect.X;
            if (creditSize.Width + quoteSize.Width <= distanceToEnd)
            {
                creditRect.X = quoteRect.X + quoteRect.Width * 1 / 2;
            }
            else
            {
                creditRect.X = screenSize.Width - creditSize.Width - screenSize.Height * 2.2 / 100;
            }

            creditRect.Y = quoteRect.Y;
            creditRect.Width = creditSize.Width;
            creditRect.Height = creditSize.Height;
            var tempSpace = screenSize.Width - (screenSize.Width/10*5.5 - screenSize.Height*1.3/100)
                            - (creditRect.X + creditRect.Width - quoteRect.X + screenSize.Height*2.5/100);
            double tempX = quoteRect.X, temp;

            if (drawPoint.X == 0)
            {
                tempX = screenSize.Height * 1.3 / 100 + tempSpace;
            }

            if (Math.Abs(drawPoint.X - screenSize.Width / 2) < 0.5)
            {
                var haftWidth = (creditRect.X + creditRect.Width - quoteRect.X + screenSize.Height * 2.5 / 100) / 2;
                tempX = screenSize.Width / 2 - haftWidth - (screenSize.Height * 1.3 / 100) / 2;
            }

            if (drawPoint.Y >= screenSize.Height)
            {
                var tempHeight = creditSize.Height + screenSize.Height*4/100;
                var tempSpace1 = drawPoint.Y > screenSize.Height ? drawPoint.Y - screenSize.Height - tempHeight : screenSize.Height - tempHeight;
                var temp1 = quoteRect.Y - tempSpace1;
                quoteRect.Y -= temp1;
                creditRect.Y -= temp1;
            }
            else
            {
                var tempHeight = quoteSize.Height + screenSize.Height / 100;
                var newY = drawPoint.Y + tempHeight;
                quoteRect.Y = newY;
                creditRect.Y = newY;
            }

            if (BitmapHelper.IsBrightArea(canvasBitmap,
                             (int)(tempX - screenSize.Height * 1.3 / 100),
                             (int)(quoteRect.Y - quoteSize.Height - screenSize.Height / 100),
                             (int)(creditRect.X + creditRect.Width - quoteRect.X + screenSize.Height * 2.5 / 100),
                             (int)(quoteSize.Height + creditSize.Height + screenSize.Height * 4 / 100)))
            {
                ds.FillRoundedRectangle(
                    new Rect((int) (tempX - screenSize.Height * 1.3 / 100),
                             (int) (quoteRect.Y - quoteSize.Height - screenSize.Height / 100),
                             (int) (creditRect.X + creditRect.Width - quoteRect.X + screenSize.Height * 2.5 / 100),
                             (int) (quoteSize.Height + creditSize.Height + screenSize.Height * 4 / 100)), 20, 20,
                    new CanvasSolidColorBrush(device, Colors.Black) {Opacity = 0.4F});
            }

            //Draw the credit

            if (quoteSize.Height > creditSize.Height)
            {
                temp = quoteRect.X - tempX;
                quoteRect.X = tempX;
                ds.DrawText(Quote.quote, quoteRect, Colors.White, new CanvasTextFormat
                {
                    FontSize = (float) (screenSize.Height * 4.5 / 100),
                    FontFamily = "Segoe UI Light",
                    FontWeight = FontWeights.Thin,
                    VerticalAlignment = CanvasVerticalAlignment.Bottom,
                    HorizontalAlignment = CanvasHorizontalAlignment.Center
                });
            }
            else
            {
                temp = quoteRect.X - tempX;
                quoteRect.X = tempX;
                ds.DrawText(Quote.quote, quoteRect, Colors.White, new CanvasTextFormat
                {
                    FontSize = (float) (screenSize.Height * 4.5 / 100),
                    FontFamily = "Segoe UI Light",
                    FontWeight = FontWeights.Thin,
                    VerticalAlignment = CanvasVerticalAlignment.Top,
                    HorizontalAlignment = CanvasHorizontalAlignment.Left
                });
            }
            creditRect.X -= temp;
            ds.DrawText(Quote.author, creditRect, Colors.White, new CanvasTextFormat
            {
                FontSize = (float) (screenSize.Height * 4.5 / 100),
                FontFamily = "Segoe UI Light",
                FontWeight = FontWeights.Thin,
                FontStyle = FontStyle.Oblique,
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
            });

            if (drawPoint.Y >= screenSize.Height)
            {
                var tempY = quoteRect.Y - quoteSize.Height - screenSize.Height / 100;
                if (tempY + screenSize.Height <= screenSize.Height * 2) return new Point(drawPoint.X, tempY + screenSize.Height);
                return new Point(-1, -1);
            }
            else
            {
                var tempY = quoteRect.Y - quoteSize.Height - screenSize.Height / 100 + quoteSize.Height + creditSize.Height + screenSize.Height * 4 / 100;
                if (tempY <= screenSize.Height) return new Point(drawPoint.X, tempY);
                return new Point(-1, -1);
            }
        }

        public async void ToggleOfflineQuote(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (SettingManager.GetOfflineQuote() != null && IsOfflineQuote != toggleSwitch.IsOn)
            {
                await StartVm.UpdateListTask();
            }
        }

        public async void ToggleQuote(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            if (IsDisplayQuote != toggleSwitch.IsOn)
            {
                await StartVm.UpdateListTask();
            }
        }

        public async void SaveOfflineQuote()
        {
            if (string.IsNullOrEmpty(OfflineQuote))
            {
                var dialog = new MessageDialog("You need to fill all informations");
                await dialog.ShowAsync();
            }
            else
            {
                SettingManager.SetOfflineQuote(OfflineQuote + "|" + OfflineAuthor);

                await StartVm.UpdateListTask();
            }
        }
    }
}