using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HtmlAgilityPack;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using ShareClass.Model;
using ShareClass.Model.Rss;
using ShareClass.Utilities;
using ShareClass.Utilities.Helpers;
using ShareClass.Utilities.Helpers.SourceDataHelper;
using ShareClass.ViewModel.StartGroup;

namespace ShareClass.ViewModel.RssGroup
{
    public class RssViewModel : ObservableObject
    {
        private RssChannel _rssChanel;

        private bool _isEnabled;

        private string _link;
        private bool _isDisplayRssDescription;
        private string _validationText;
        private int _rssItemNumber;

        #region Property

        public RssChannel RssChanel
        {
            get { return _rssChanel; }
            set
            {
                if (Equals(value, _rssChanel)) return;
                _rssChanel = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsDisplayRssDescription
        {
            get { return _isDisplayRssDescription; }
            set
            {
                if (value == _isDisplayRssDescription) return;
                _isDisplayRssDescription = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _link; }
            set
            {
                if (value == _link) return;
                _link = value;
                SettingManager.SetRssLink(_link);
                OnPropertyChanged();
            }
        }

        public int RssItemNumber
        {
            get { return _rssItemNumber; }
            set
            {
                if (value == _rssItemNumber) return;
                _rssItemNumber = value;
                SettingManager.SetRssItemNum(_rssItemNumber);
                OnPropertyChanged();
            }
        }

        public string ValidationText
        {
            get { return _validationText; }
            set
            {
                if (value == _validationText) return;
                _validationText = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Draw Position

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

        public StartViewModel StartVm => ((ViewModelLocator)Application.Current.Resources["Locator"]).StartVm;

        private void Initialize()
        {
            PositionItemsCollection = new ObservableCollection<ImageSourceItem>();

            PositionHelper.GetPositionItems(ref _positionItemsCollection);

            var number = PositionHelper.GetElementPosition("R");
            SelectedPosition = PositionItemsCollection[number] ?? PositionItemsCollection[0];
        }

        public async void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            SelectedPosition = (ImageSourceItem)comboBox.SelectedItem;

            var number = PositionHelper.GetElementPosition("R");
            if (SelectedPosition != null)
            {
                if (SelectedPosition.Number != number)
                    PositionHelper.SetElementPosition("R", SelectedPosition.Number);
                await StartVm.UpdateListTask();
            }
        }

        #endregion

        public RssViewModel()
        {
            ValidationText = "must be between 1 and 5";
            Link = SettingManager.GetRssLink();
            IsEnabled = SettingManager.GetIsDisplayRss().Item1;
            IsDisplayRssDescription = SettingManager.GetIsDisplayRss().Item2;
            RssItemNumber = SettingManager.GetRssItemNum();

            Initialize();
        }

        public async Task GetRssTask(string link)
        {
            RssChanel = new RssChannel();

            bool isValid = await HttpService.GetHeadTask(link);
            if (!isValid)
            {
                //Link is not available
                ValidationText = "Link is not available";
                return;
            }

            ValidationText = "";

            string result = await HttpService.SendAsync(link);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);

            var channelNode = doc.DocumentNode.Descendants("channel").First();

            bool t = false;
            bool l = false;
            bool d = false;
            bool la = false;

            foreach (HtmlNode htmlNode in channelNode.ChildNodes)
            {
                switch (htmlNode.Name)
                {
                    case "title":
                        t = true;
                        break;
                    case "link":
                        l = true;
                        break;
                    case "description":
                        d = true;
                        break;
                    case "language":
                        la = true;
                        break;
                }
            }

            if (t) RssChanel.Title = channelNode.ChildNodes["title"].InnerText;
            if (l) RssChanel.Link = channelNode.ChildNodes["link"].InnerText;
            if (d) RssChanel.Description = channelNode.ChildNodes["description"].InnerText;
            if (la) RssChanel.Language = channelNode.ChildNodes["language"].InnerText;

            RssChanel.ItemList = new ObservableCollection<RssItem>();
            var itemNodes = channelNode.Descendants("item");
            foreach (HtmlNode itemNode in itemNodes)
            {

                bool it = false;
                bool il = false;
                bool id = false;
                bool ip = false;

                foreach (HtmlNode childNode in itemNode.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "title":
                            it = true;
                            break;
                        case "link":
                            il = true;
                            break;
                        case "description":
                            id = true;
                            break;
                        case "pubDate":
                            ip = true;
                            break;
                    }
                }

                RssItem i = new RssItem();
                if (id) i.Description = ExtractDescription(itemNode.ChildNodes["description"].InnerText);
                if (il) i.Link = itemNode.ChildNodes["link"].NextSibling.InnerText;
                if (string.IsNullOrEmpty(i.Link)) i.Link = itemNode.ChildNodes["link"].InnerText;
                if (ip) i.Time = itemNode.ChildNodes["pubDate"].InnerText;
                if (it) i.Title = "» " + itemNode.ChildNodes["title"].InnerText;

                RssChanel.ItemList.Add(i);
            }
        }

        private string ExtractDescription(string data)
        {
            string result;

            data = WebUtility.HtmlDecode(data);

            if (data.Contains("CDATA"))
            {
                result = data.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(result);

                result = doc.DocumentNode.InnerText.Trim();
            }
            else
            {
                result = data;
            }

            return result;
        }

        public async Task<Point> DrawRss(CanvasDrawingSession ds, CanvasDevice device, CanvasBitmap canvasBitmap, Point drawPoint)
        {
            if (SettingManager.GetIsDisplayRss().Item1)
            {
                var screenSize = SettingManager.GetWindowsResolution();

                string rssLink = SettingManager.GetRssLink();
                if (string.IsNullOrEmpty(rssLink))
                {
                    return drawPoint;
                }

                string text = await BuildRssText(rssLink);

                Rect drawRect = new Rect(0, 0, 0, 0)
                {
                    X = (screenSize.Width / 10) * 6.5,
                    Width = (screenSize.Width / 10) * 3.35,
                    Y = drawPoint.Y
                };

                CanvasTextFormat format = new CanvasTextFormat
                {
                    FontFamily = "Segoe UI",
                    FontSize = (float) (screenSize.Height * 2 / 100),
                    VerticalAlignment = CanvasVerticalAlignment.Top,
                    HorizontalAlignment = CanvasHorizontalAlignment.Left,
                    WordWrapping = CanvasWordWrapping.WholeWord
                };

                var drawSize = BitmapHelper.TextRect(text, format, ds, drawRect.Width);

                //Margin
                //drawRect.Width = drawRect.Width + 12;
                //drawRect.Height = drawRect.Height + 8;
                //drawRect.X += 30;

                if (drawPoint.X == 0)
                {
                    drawRect.X = screenSize.Height * 1.7 / 100 + screenSize.Width - (screenSize.Width / 10 * 6.5 - screenSize.Height * 1.3 / 100)
                                                  - (screenSize.Width / 10 * 3.35 + screenSize.Height * 2.5 / 100);
                }

                if (Math.Abs(drawPoint.X - screenSize.Width / 2) < 0.5)
                {
                    var haftWidth = (drawRect.Width + screenSize.Height * 2.5 / 100) / 2;
                    drawRect.X = screenSize.Width / 2 - haftWidth - (screenSize.Height * 1.3 / 100) / 2;
                }

                if (drawPoint.Y >= screenSize.Height)
                {
                    var tempHeight = drawSize.Height + screenSize.Height * 4 / 100;
                    drawRect.Y = drawPoint.Y > screenSize.Height ? drawPoint.Y - screenSize.Height - tempHeight : screenSize.Height - tempHeight;
                }

                //if (BitmapHelper.IsBrightArea(canvasBitmap,
                //  (int)(drawRect.X - screenSize.Height * 1.3 / 100),
                //  (int)(drawRect.Y - screenSize.Height / 100),
                //  (int)(drawRect.Width + screenSize.Height * 2.5 / 100),
                //  (int)(drawSize.Height + screenSize.Height * 4 / 100)))
                //{
                //    ds.FillRoundedRectangle(new Rect((int)(drawRect.X - screenSize.Height * 1.3 / 100),
                //                                     (int)(drawRect.Y - screenSize.Height / 100),
                //                                     (int)(drawRect.Width + screenSize.Height * 2.5 / 100),
                //                                     (int)(drawSize.Height + screenSize.Height * 4 / 100)), 20, 20,
                //                            new CanvasSolidColorBrush(device, Colors.Black) { Opacity = 0.4F });
                //}
                ds.FillRoundedRectangle(new Rect((int)(drawRect.X - screenSize.Height * 1.3 / 100),
                                                    (int)(drawRect.Y - screenSize.Height / 100),
                                                    (int)(drawRect.Width + screenSize.Height * 2.5 / 100),
                                                    (int)(drawSize.Height + screenSize.Height * 4 / 100)), 20, 20,
                                           new CanvasSolidColorBrush(device, Colors.Black) { Opacity = 0.4F });
                //ds.FillRectangle(drawRect, Color.FromArgb(150, 0, 0, 0));
                ds.DrawText(text, drawRect, Colors.White, format);

                if (drawPoint.Y >= screenSize.Height)
                {
                    var tempY = drawRect.Y - screenSize.Height / 100;
                    if (tempY + screenSize.Height <= screenSize.Height * 2) return new Point(drawPoint.X, tempY + screenSize.Height);
                    return new Point(-1, -1);
                }
                else
                {
                    var tempY = drawRect.Y - screenSize.Height / 100 + drawSize.Height + screenSize.Height * 4 / 100;
                    if (tempY <= screenSize.Height) return new Point(drawPoint.X, tempY);
                    return new Point(-1, -1);
                }
            }
            //return new Point(0,0);
            return drawPoint;
        }

        public async Task<string> BuildRssText(string rssLink)
        {
            await GetRssTask(rssLink);

            int num = SettingManager.GetRssItemNum();

            bool isShowDesc = SettingManager.GetIsDisplayRss().Item2;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < num; i++)
            {
                sb.Append(RssChanel.ItemList[i].Title);
                
                sb.AppendLine();

                if (isShowDesc)
                {
                    sb.Append("   ");
                    sb.Append(RssChanel.ItemList[i].Description);
                    sb.AppendLine();
                }
            }

            var result = sb.ToString().Trim();

            return result;
        }
    }
}