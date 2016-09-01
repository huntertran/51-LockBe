using ShareClass.Model;

namespace ShareClass.ViewModel.StartGroup
{
    public interface IStartPage
    {
        void PreviewImageInvalidate();

        void NavigateWebView(string link);

        void NavigateToPage(MenuFunc m);
    }
}
