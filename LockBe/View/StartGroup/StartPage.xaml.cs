using System;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using LockBe.View.ImageSourceGroup;
using LockBe.View.NoteGroup;
using LockBe.View.QuoteGroup;
using LockBe.View.RssGroup;
using LockBe.View.SettingGroup;
using LockBe.View.WeatherGroup;
using Microsoft.Graphics.Canvas.UI.Xaml;
using ShareClass.Model;
using ShareClass.Utilities.Helpers;
using ShareClass.ViewModel.StartGroup;

namespace LockBe.View.StartGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : IStartPage
    {
        public StartViewModel Vm => (StartViewModel) DataContext;

        public StartPage()
        {
            InitializeComponent();

            var startViewModel = DataContext as StartViewModel;
            if (startViewModel != null) startViewModel.StartPage = this;

            //Set title bar
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(TitleGrid);

            Loaded += StartPage_Loaded;
        }

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Set width and heigh of CanvasControl
            Size targetSize = SettingManager.GetWindowsResolution();

            PreviewCanvasControl.Width = targetSize.Width;
            PreviewCanvasControl.Height = targetSize.Height;

            NavigateToPage(MenuFunc.Start);
        }
        
        private void MenuListItem_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            MenuListItem m = ((Grid) sender).DataContext as MenuListItem;
            Debug.Assert(m != null, "m != null");
            NavigateToPage(m.MenuF);
        }

        public void NavigateToFunction(Frame frame, MenuFunc func)
        {
            switch (func)
            {
                case MenuFunc.Start:
                {
                    if (!(frame.Content is StartControlPage))
                    {
                        frame.Navigate(typeof(StartControlPage));
                    }
                    break;
                }
                case MenuFunc.ImageSource:
                {
                    if (!(frame.Content is ImageSourcePage))
                    {
                        frame.Navigate(typeof(ImageSourcePage));
                    }
                    break;
                }
                case MenuFunc.WeatherForecast:
                {
                    if (!(frame.Content is WeatherPage))
                    {
                        frame.Navigate(typeof(WeatherPage));
                    }
                    break;
                }
                case MenuFunc.Rss:
                {
                    if (!(frame.Content is RssPage))
                    {
                        frame.Navigate(typeof(RssPage));
                    }
                    break;
                }
                case MenuFunc.Settings:
                {
                    if (!(frame.Content is SettingPage))
                    {
                        frame.Navigate(typeof(SettingPage));
                    }
                    break;
                }
                case MenuFunc.Quote:
                {
                    if (!(frame.Content is QuotePage))
                    {
                        frame.Navigate(typeof(QuotePage));
                    }
                    break;
                }
                case MenuFunc.Note:
                {
                    if (!(frame.Content is NotePage))
                    {
                        frame.Navigate(typeof (NotePage));
                    }
                    break;
                }
                default:
                {
                    if (!(frame.Content is ImageSourcePage))
                    {
                        frame.Navigate(typeof(ImageSourcePage));
                    }
                    break;
                }
            }
        }

        private void PreviewCanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Vm.RenderTarget != null)
            {
                args.DrawingSession.DrawImage(Vm.RenderTarget);
            }
        }

        public void PreviewImageInvalidate()
        {
            PreviewCanvasControl.Invalidate();
        }

        private void Viewbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("Size Changed: " + e.NewSize.Width + " - " + e.NewSize.Height);
        }

        public void NavigateWebView(string link)
        {
            MainWebView.Navigate(new Uri(link, UriKind.Absolute));
        }

        public void NavigateToPage(MenuFunc m)
        {
            NavigateToFunction(MainFrame, m);
        }

        private void CloseWebViewButton_OnClick(object sender, RoutedEventArgs e)
        {
            Vm.IsShowWebView = false;
        }
    }
}