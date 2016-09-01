using Windows.UI.Xaml;
using ShareClass.Model;
using ShareClass.ViewModel.NoteGroup;

namespace LockBe.View.NoteGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage
    {
        public NoteViewModel Vm => (NoteViewModel) DataContext;

        public NotePage()
        {
            InitializeComponent();

            Loaded += NotePage_Loaded;
        }

        private void NotePage_Loaded(object sender, RoutedEventArgs e)
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