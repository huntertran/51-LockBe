using System;
using Windows.System;
using Windows.UI.Xaml;

namespace LockBe.View.SettingGroup.SubSettingGroup
{
    public sealed partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void EmailThi_OnClick(object sender, RoutedEventArgs e)
        {
            await
                Launcher.LaunchUriAsync(
                    new Uri("mailto:Thi.NguyenVuHoang@studentpartner.com?subject=[LockBe]&body=Hello, "));
        }

        private async void LinkedInThi_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://vn.linkedin.com/in/hoangthi1710"));
        }

        private async void EmailTuan_OnClick(object sender, RoutedEventArgs e)
        {
            await
                Launcher.LaunchUriAsync(
                    new Uri("mailto:cuoilennaocacban@hotmail.com?subject=[LockBe]&body=Hello, "));
        }

        private async void LinkedInTuan_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://vn.linkedin.com/in/tuanmsp"));
        }
    }
}
