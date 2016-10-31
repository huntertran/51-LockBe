using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Newtonsoft.Json.Linq;
using ShareClass.Model;
using ShareClass.Utilities;
using ShareClass.Utilities.Helpers;
using ShareClass.Utilities.Helpers.SourceDataHelper;

namespace ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup
{
    public class BingSettingViewModel : ObservableObject
    {
        //http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US

        private int numberOfImage = 5;
        
        private ObservableCollection<BingLanguage> _bingLanguageCollection;
        private string _languageCode;
        private BingImageRoot _bingImageRoot;
        private bool _isShowImageInfo;


        public ObservableCollection<BingLanguage> BingLanguageCollection
        {
            get { return _bingLanguageCollection; }
            set
            {
                if (Equals(value, _bingLanguageCollection)) return;
                _bingLanguageCollection = value;
                OnPropertyChanged();
            }
        }

        public string LanguageCode
        {
            get { return _languageCode; }
            set
            {
                if (value == _languageCode) return;
                _languageCode = value;
                OnPropertyChanged();
            }
        }

        public BingImageRoot BingImageRoot
        {
            get { return _bingImageRoot; }
            set
            {
                if (Equals(value, _bingImageRoot)) return;
                _bingImageRoot = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowImageInfo
        {
            get { return _isShowImageInfo; }
            set
            {
                if (Equals(value, _isShowImageInfo)) return;
                _isShowImageInfo = value;
                SettingManager.BingSetShowInfo(_isShowImageInfo);
                OnPropertyChanged();
            }
        }

        public BingSettingViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            BingLanguageCollection = new ObservableCollection<BingLanguage>();

            BingLanguage b = new BingLanguage
            {
                Name = "عربي",
                Code = "ar"
            };
            BingLanguageCollection.Add(b);

            //b = new BingLanguage
            //{
            //    Name = "عربي",
            //    Code = "ar"
            //};
            //BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "bg-BG",
                Name = "Български"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "ca-ES",
                Name = "català"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "cs-CZ",
                Name = "Čeština"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "da-DK",
                Name = "dansk"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "de-DE",
                Name = "Deutsch"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "el-GR",
                Name = "Ελληνικά"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "es-ES",
                Name = "Español"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "et-EE",
                Name = "Eesti"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "eu-ES",
                Name = "Euskara"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "fi-FI",
                Name = "suomi"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "fr-FR",
                Name = "Français"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "gl-ES",
                Name = "Galego"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "he",
                Name = "עברית"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "hi-IN",
                Name = "हिन्दी"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "hr-HR",
                Name = "hrvatski"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "hu-HU",
                Name = "magyar"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "is-IS",
                Name = "Íslenska"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "it-IT",
                Name = "Italiano"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "ja-JP",
                Name = "日本語"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "ko-KR",
                Name = "한국어"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "lt-LT",
                Name = "Lietuvių"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "lv-LV",
                Name = "Latviešu"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "ms-MY",
                Name = "Melayu"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "nb-NO",
                Name = "Norsk"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "nl-NL",
                Name = "Nederlands"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "en-US",
                Name = "English"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "pl-PL",
                Name = "polski"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "pt-BR",
                Name = "Português (Brasil)"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "pt-PT",
                Name = "Português (Portugal)"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "ro-RO",
                Name = "Română"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "ru-RU",
                Name = "русский"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "sk-SK",
                Name = "slovenčina"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "sl-SI",
                Name = "slovenščina"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "sr-ba",
                Name = "српски (ћирилица)"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "sv-SE",
                Name = "Svenska"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "th-TH",
                Name = "ไทย"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "tr-TR",
                Name = "Türkçe"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "uk-UA",
                Name = "українська"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "vi-VN",
                Name = "Tiếng Việt"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "zh-CN",
                Name = "简体中文"

            };
            BingLanguageCollection.Add(b);

            b = new BingLanguage
            {
                Code = "zh-HK",
                Name = "繁體中文"

            };
            BingLanguageCollection.Add(b);

            BingLanguageCollection = new ObservableCollection<BingLanguage>(BingLanguageCollection.OrderBy(x => x.Name));

            //Show Image info or not
            IsShowImageInfo = SettingManager.BingGetShowInfo();
            LanguageCode = SettingManager.BingGetLanguage();
        }
        
        private string GenerateLink(string market)
        {
            return "http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=" + numberOfImage + "&mkt=" + market;
        }

        private async Task<BingImageRoot> GetImagesTask(string market = null)
        {
            if (string.IsNullOrEmpty(market))
            {
                market = "en-us";
            }
            string link = GenerateLink(market);
            string result = await HttpService.SendAsync(link);

            if (string.IsNullOrEmpty(result)) return null;

            JObject j = JObject.Parse(result);
            return j.ToObject<BingImageRoot>();
        }

        public async Task GetImageRoot()
        {
            BingImageRoot = await GetImagesTask(LanguageCode);
            BingHelper b = new BingHelper();
            foreach (BingImage bingImage in BingImageRoot.images)
            {
                var appropriateLink = b.GenerateImageLink(bingImage.urlbase);
                var isLinkValid = await HttpService.GetHeadTask(appropriateLink, false);
                if (isLinkValid)
                {
                    bingImage.AppropriateLink = appropriateLink;
                }
            }
            for (int i = BingImageRoot.images.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(BingImageRoot.images[i].AppropriateLink))
                {
                    BingImageRoot.images.RemoveAt(i);
                }
            }
        }

        public void DrawInfo(CanvasDrawingSession ds, CanvasDevice device)
        {
            Size screenSize = SettingManager.GetWindowsResolution();

            Rect drawRect = new Rect()
            {
                Y = screenSize.Height*95/100,
                X = screenSize.Width * 0.3 / 100,
                Height = screenSize.Height * 5 / 100,
            };

            CanvasTextFormat format = new CanvasTextFormat
            {
                FontFamily = "Segoe UI",
                FontSize = 14,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            var drawSize = BitmapHelper.TextRect(BingImageRoot.images[0].copyright, format, ds);

            drawRect.Width = drawSize.Width;

            ds.FillRectangle(new Rect(drawRect.X - screenSize.Width * 0.4  / 100,
                       screenSize.Height - drawSize.Height - screenSize.Width * 0.2 / 100,
                       drawRect.Width + screenSize.Width * 1 / 100,
                       drawRect.Height + screenSize.Width * 1 / 100), Color.FromArgb(150, 0, 0, 0));
            ds.DrawText(BingImageRoot.images[0].copyright, drawRect, Colors.White, format);
        }

        public async void UpdatePreviewImage()
        {
            await ((ViewModelLocator) Application.Current.Resources["Locator"]).StartVm.UpdatePreviewImageTask();
        }
    }
}