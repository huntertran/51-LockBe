using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LockBe.UserControls
{
    public sealed partial class TitleTextBlockUserControl
    {
        public TitleTextBlockUserControl()
        {
            InitializeComponent();
        }

        public SolidColorBrush BackgroundColor
        {
            get { return (SolidColorBrush) GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(SolidColorBrush), typeof(TitleTextBlockUserControl),
                new PropertyMetadata(true));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TitleTextBlockUserControl),
                new PropertyMetadata(""));

        public double TitleFontSize
        {
            get { return (double) GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(TitleTextBlockUserControl),
                new PropertyMetadata(24));

    }
}