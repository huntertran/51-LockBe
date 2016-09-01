using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml.Input;

namespace LockBe.View.SettingGroup.SubSettingGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RateAndFeedbackPage
    {
        public RateAndFeedbackPage()
        {
            InitializeComponent();
        }

        private async void RateGrid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await
                Launcher.LaunchUriAsync(
                    new Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName));
        }

        private async void FeedbackGrid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await
                Launcher.LaunchUriAsync(
                    new Uri("mailto:cuoilennaocacban@hotmail.com?subject=[LockBe] Feedback&body=Hello, "));
        }
    }
}
