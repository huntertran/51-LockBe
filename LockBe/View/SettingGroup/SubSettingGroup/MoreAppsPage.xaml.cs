using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ShareClass.Model;
using ShareClass.ViewModel.SettingGroup;

namespace LockBe.View.SettingGroup.SubSettingGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoreAppsPage
    {
        public MoreAppViewModel Vm => (MoreAppViewModel)DataContext;

        public MoreAppsPage()
        {
            InitializeComponent();
        }

        public async void MoreAppItem_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            AppItem a = ((Grid)sender).DataContext as AppItem;
            if (a != null)
                await
                    Launcher.LaunchUriAsync(
                        new Uri("ms-windows-store://pdp/?ProductId=" + a.link.Trim('/').Split('/').Last()));
        }
    }
}
