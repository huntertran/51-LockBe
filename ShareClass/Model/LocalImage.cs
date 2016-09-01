using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using ShareClass.ViewModel;

namespace ShareClass.Model
{
    public class LocalImage : ObservableObject
    {
        private StorageFile _file;
        private WriteableBitmap _bitmap;

        public StorageFile File
        {
            get { return _file; }
            set
            {
                if (Equals(value, _file)) return;
                _file = value;
                OnPropertyChanged();
            }
        }

        public WriteableBitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                if (Equals(value, _bitmap)) return;
                _bitmap = value;
                OnPropertyChanged();
            }
        }
    }
}
