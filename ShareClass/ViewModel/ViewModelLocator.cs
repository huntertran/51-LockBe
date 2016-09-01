/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:BrandedApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ShareClass.ViewModel.ImageSourceGroup;
using ShareClass.ViewModel.NoteGroup;
using ShareClass.ViewModel.QuoteGroup;
using ShareClass.ViewModel.RssGroup;
using ShareClass.ViewModel.SettingGroup;
using ShareClass.ViewModel.StartGroup;
using ShareClass.ViewModel.WeatherGroup;

namespace ShareClass.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<StartViewModel>();
            SimpleIoc.Default.Register<ImageSourceViewModel>();
            SimpleIoc.Default.Register<QuoteViewModel>();
            SimpleIoc.Default.Register<NoteViewModel>();
            SimpleIoc.Default.Register<WeatherViewModel>();
            SimpleIoc.Default.Register<RssViewModel>();

            SimpleIoc.Default.Register<MoreAppViewModel>();
        }

        public StartViewModel StartVm => ServiceLocator.Current.GetInstance<StartViewModel>();
        public ImageSourceViewModel ImageSourceVm => ServiceLocator.Current.GetInstance<ImageSourceViewModel>();
        public WeatherViewModel WeatherVm => ServiceLocator.Current.GetInstance<WeatherViewModel>();
        public QuoteViewModel QuoteVm => ServiceLocator.Current.GetInstance<QuoteViewModel>();
        public NoteViewModel NoteVm => ServiceLocator.Current.GetInstance<NoteViewModel>();
        public RssViewModel RssVm => ServiceLocator.Current.GetInstance<RssViewModel>();

        public MoreAppViewModel MoreAppVm => ServiceLocator.Current.GetInstance<MoreAppViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}