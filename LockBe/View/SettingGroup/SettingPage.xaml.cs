using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using LockBe.View.SettingGroup.SubSettingGroup;

namespace LockBe.View.SettingGroup
{
    public sealed partial class SettingPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;
            Debug.Assert(p != null, "p != null");
            PivotItem pI = p.SelectedItem as PivotItem;
            if (pI != null)
                switch (pI.Tag.ToString())
                {
                    case "0":
                        //General
                        //SettingFrame.Navigate(typeof(SaveLocationSettingPage));
                        break;
                    case "1":
                        //Rate and Feedback
                        SettingFrame.Navigate(typeof(RateAndFeedbackPage));
                        break;
                    case "2":
                        //About
                        SettingFrame.Navigate(typeof(AboutPage));
                        break;
                    case "3":
                        //More apps
                        SettingFrame.Navigate(typeof(MoreAppsPage));
                        break;
                }
        }
    }
}
