using ShareClass.ViewModel.StartGroup;
namespace LockBe.View.StartGroup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartControlPage
    {
        public StartViewModel Vm => (StartViewModel)DataContext;

        public StartControlPage()
        {
            InitializeComponent();
            if (!Vm.startControlPageLoaded)
            {
                Vm.UpdateListAsync();
                Vm.startControlPageLoaded = true;
            }
           
        }
    }
}
