using System;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using ShareClass.Model;
using ShareClass.Utilities.Helpers;
using ShareClass.Utilities.Helpers.SourceDataHelper;
using ShareClass.ViewModel.StartGroup;

namespace ShareClass.ViewModel.NoteGroup
{
    public class NoteViewModel : ObservableObject
    {
        private bool firstLoad = false;

        private ObservableCollection<ImageSourceItem> _positionItemsCollection;

        public ObservableCollection<ImageSourceItem> PositionItemsCollection
        {
            get { return _positionItemsCollection; }
            set
            {
                if (Equals(value, _positionItemsCollection)) return;
                _positionItemsCollection = value;
                OnPropertyChanged();
            }
        }

        private ImageSourceItem _selectedPosition;

        public ImageSourceItem SelectedPosition
        {
            get
            {
                return _selectedPosition;
            }
            set
            {
                if (Equals(value, _selectedPosition)) return;
                _selectedPosition = value;
                OnPropertyChanged();
            }
        }

        private string _note;
        private bool _isDisplayNote;

        public StartViewModel StartVm => ((ViewModelLocator)Application.Current.Resources["Locator"]).StartVm;

        public string Note
        {
            get { return _note; }
            set
            {
                if (Equals(value, _note)) return;
                _note = value;
                OnPropertyChanged();
            }
        }

        public bool IsDisplayNote
        {
            get { return _isDisplayNote; }
            set
            {
                if (value == _isDisplayNote) return;
                _isDisplayNote = value;
                SettingsHelper.SetSetting(SettingKey.IsDisplayNote.ToString(), _isDisplayNote);
                OnPropertyChanged();
            }
        }


        public NoteViewModel()
        {
            firstLoad = true;

            // Get Note Settings
            IsDisplayNote = SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayNote.ToString());
            if (SettingManager.GetNote() != null)
            {
                Note = SettingManager.GetNote();
            }

            Initialize();
        }

        private void Initialize()
        {
            PositionItemsCollection = new ObservableCollection<ImageSourceItem>();

            PositionHelper.GetPositionItems(ref _positionItemsCollection);

            var number = PositionHelper.GetElementPosition("N");
            SelectedPosition = PositionItemsCollection[number] ?? PositionItemsCollection[0];
        }

        public async void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            SelectedPosition = (ImageSourceItem)comboBox.SelectedItem;

            var number = PositionHelper.GetElementPosition("N");
            if (SelectedPosition != null)
            {
                if (SelectedPosition.Number != number)
                {
                    PositionHelper.SetElementPosition("N", SelectedPosition.Number);
                    await StartVm.UpdateListTask();
                }               
            }
        }

        public Point DrawNote(CanvasDrawingSession ds, CanvasDevice device, CanvasBitmap canvasBitmap, Point drawPoint)
        {

            if (!SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayNote.ToString()))
            {
                return drawPoint;
            }

            if (string.IsNullOrEmpty(Note))
            {
                return drawPoint;
            }

            Size screenSize = SettingManager.GetWindowsResolution();

            //Qoute rect coordinate
            //X: 7.5 of 10
            //Y: DrawPoint.Y
            //width: 2.35
            //heigh: depend


            Rect noteRect = new Rect(0, 0, 0, 0)
            {
                X = (screenSize.Width / 10) * 7.5,
                Width = (screenSize.Width / 10) * 2.35,
                Y = drawPoint.Y
            };


            CanvasTextFormat textFormat = new CanvasTextFormat
            {
                WordWrapping = CanvasWordWrapping.Wrap,
                FontFamily = "Segoe UI Light",
                FontSize = (float)(screenSize.Height * 3.5 / 100),
                FontWeight = FontWeights.Thin,
                HorizontalAlignment = CanvasHorizontalAlignment.Left
            };

            Rect noteSize = BitmapHelper.TextRect(Note, textFormat, ds, noteRect.Width);

            if (drawPoint.X == 0)
            {
                noteRect.X = screenSize.Height * 1.7 / 100 + screenSize.Width - (screenSize.Width / 10 * 7.5 - screenSize.Height * 1.3 / 100)
                                              - (screenSize.Width / 10 * 2.35 + screenSize.Height * 2.5 / 100);
            }

            if (Math.Abs(drawPoint.X - screenSize.Width / 2) < 0.5)
            {
                var haftWidth = (noteRect.Width + screenSize.Height * 2.5 / 100) / 2;
                noteRect.X = screenSize.Width / 2 - haftWidth - (screenSize.Height * 1.3 / 100) / 2;
            }

            if (drawPoint.Y >= screenSize.Height)
            {
                var tempHeight = noteSize.Height + screenSize.Height * 4 / 100;
                noteRect.Y = drawPoint.Y > screenSize.Height ? drawPoint.Y - screenSize.Height - tempHeight : screenSize.Height - tempHeight - screenSize.Height * 3 / 100;
            }

            //Draw the background
            if (BitmapHelper.IsBrightArea(canvasBitmap,
                   (int)(noteRect.X - screenSize.Height * 1.3 / 100),
                   (int)(noteRect.Y - screenSize.Height / 100),
                   (int)(noteRect.Width + screenSize.Height * 2.5 / 100),
                   (int)(noteSize.Height + screenSize.Height * 4 / 100)))
            {
                ds.FillRoundedRectangle(new Rect((int)(noteRect.X - screenSize.Height * 1.3 / 100),
                                                 (int)(noteRect.Y - screenSize.Height / 100),
                                                 (int)(noteRect.Width + screenSize.Height * 2.5 / 100),
                                                 (int)(noteSize.Height + screenSize.Height * 4 / 100)), 20, 20,
                                        new CanvasSolidColorBrush(device, Colors.Black) { Opacity = 0.4F });
            }

            ds.DrawText(Note, noteRect, Colors.White, new CanvasTextFormat
            {
                FontSize = (float)(screenSize.Height * 3.5 / 100),
                FontFamily = "Segoe UI Light",
                FontWeight = FontWeights.Thin,
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                WordWrapping = CanvasWordWrapping.Wrap,
            });

            if (drawPoint.Y >= screenSize.Height)
            {
                var tempY = noteRect.Y - screenSize.Height / 100;
                if (tempY + screenSize.Height <= screenSize.Height * 2) return new Point(drawPoint.X, tempY + screenSize.Height);
                return new Point(-1, -1);
            }
            else
            {
                var tempY = noteRect.Y - screenSize.Height / 100 + noteSize.Height + screenSize.Height * 4 / 100;
                if (tempY <= screenSize.Height) return new Point(drawPoint.X, tempY);
                return new Point(-1, -1);
            }
        }


        public async void ToggleNote(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch) sender;
            if (SettingManager.GetNote() != null && IsDisplayNote != toggleSwitch.IsOn )
            {
                await StartVm.UpdateListTask();
            }
        }

        public async void SaveNote()
        {
            if (string.IsNullOrEmpty(Note))
            {
                var dialog = new MessageDialog("You need to write your note");
                await dialog.ShowAsync();
            }
            else
            {
                SettingManager.SetNote(Note);

                await StartVm.UpdateListTask();
            }
        }
    }
}
