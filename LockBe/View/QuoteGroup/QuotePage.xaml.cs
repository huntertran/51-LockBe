using Windows.UI.Xaml;
using ShareClass.Model;
using ShareClass.ViewModel.QuoteGroup;

namespace LockBe.View.QuoteGroup
{
    public sealed partial class QuotePage
    {
        public QuoteViewModel Vm => (QuoteViewModel) DataContext;

        public QuotePage()
        {
            InitializeComponent();

            Loaded += QuotePage_Loaded;
        }

        private void QuotePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PositionComboBox.Items != null)
                foreach (ImageSourceItem item in PositionComboBox.Items)
                {
                    if (item.Number == Vm.SelectedPosition.Number)
                    {
                        PositionComboBox.SelectedItem = item;
                    }
                }
        }
    }
}