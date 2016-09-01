using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using HtmlAgilityPack;

namespace ShareClass.Utilities.Helpers.SourceDataHelper
{
    public class BingHelper
    {
        private readonly List<Size> _availableSizes = new List<Size>();

        public BingHelper()
        {
            InitSize();
        }

        private void InitSize()
        {
            Size s = new Size(150, 150);
            _availableSizes.Add(s);
            s = new Size(176, 220);
            _availableSizes.Add(s);
            s = new Size(200, 200);
            _availableSizes.Add(s);
            s = new Size(220, 176);
            _availableSizes.Add(s);
            s = new Size(234, 416);
            _availableSizes.Add(s);
            s = new Size(240, 240);
            _availableSizes.Add(s);
            s = new Size(240, 320);
            _availableSizes.Add(s);
            s = new Size(240, 400);
            _availableSizes.Add(s);
            s = new Size(288, 480);
            _availableSizes.Add(s);
            s = new Size(320, 180);
            _availableSizes.Add(s);
            s = new Size(320, 240);
            _availableSizes.Add(s);
            s = new Size(320, 320);
            _availableSizes.Add(s);
            s = new Size(360, 480);
            _availableSizes.Add(s);
            s = new Size(400, 240);
            _availableSizes.Add(s);
            s = new Size(416, 234);
            _availableSizes.Add(s);
            s = new Size(480, 288);
            _availableSizes.Add(s);
            s = new Size(480, 360);
            _availableSizes.Add(s);
            s = new Size(480, 640);
            _availableSizes.Add(s);
            s = new Size(480, 800);
            _availableSizes.Add(s);
            s = new Size(540, 900);
            _availableSizes.Add(s);
            s = new Size(640, 360);
            _availableSizes.Add(s);
            s = new Size(640, 480);
            _availableSizes.Add(s);
            s = new Size(720, 1280);
            _availableSizes.Add(s);
            s = new Size(768, 1024);
            _availableSizes.Add(s);
            s = new Size(768, 1280);
            _availableSizes.Add(s);
            s = new Size(768, 1366);
            _availableSizes.Add(s);
            s = new Size(800, 480);
            _availableSizes.Add(s);
            s = new Size(800, 600);
            _availableSizes.Add(s);
            s = new Size(900, 540);
            _availableSizes.Add(s);
            s = new Size(1024, 768);
            _availableSizes.Add(s);
            s = new Size(1080, 1920);
            _availableSizes.Add(s);
            s = new Size(1280, 720);
            _availableSizes.Add(s);        
            s = new Size(1280, 768);
            _availableSizes.Add(s);
            s = new Size(1366, 768);
            _availableSizes.Add(s);
            s = new Size(1920, 1080);
            _availableSizes.Add(s);
            s = new Size(1920, 1200);
            _availableSizes.Add(s);
        }

        public string GenerateImageLink(string urlBase)
        {
            var res = SettingManager.GetWindowsResolution();

            //string ending = await
            //    GetResolutionTask(res.Width.ToString(CultureInfo.InvariantCulture),
            //        res.Height.ToString(CultureInfo.InvariantCulture));

            Size s = GetNearestSize(res);

            Debug.WriteLine("Size: " + s.Width + " | " + s.Height);

            return $"http://www.bing.com{urlBase}_{s.Width}x{s.Height}.jpg";
        }

        public async Task<string> GetResolutionTask(string width, string height)
        {
            //http://www.bing.com/ImageResolution.aspx?w=367&h=805
            var html =
                await HttpService.SendAsync($"http://www.bing.com/ImageResolution.aspx?w={width}&h={height}", null, null,
                        null, false);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = doc.DocumentNode.Descendants("a").First().Attributes["href"].Value.Split('_').Last();

            return result;
        }

        public Size GetNearestSize(Size windowsSize)
        {
            var closest =
                _availableSizes.Aggregate(
                    (x, y) => Math.Abs(x.Width - windowsSize.Width) < Math.Abs(y.Width - windowsSize.Width) ? x : y);

            double width = closest.Width;

            var closest2 =
                _availableSizes.Aggregate(
                    (x, y) => Math.Abs(x.Height - windowsSize.Height) < Math.Abs(y.Height - windowsSize.Height) ? x : y);

            double height = closest2.Height;

            foreach (Size size in _availableSizes)
            {
                if (size.Width == width && size.Height == height)
                {
                    return size;
                }
            }

            if (closest.Width*closest.Height > closest2.Width*closest2.Height)
            {
                return closest;
            }

            return closest2;
        }
    }


}
