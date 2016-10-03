using System.Linq;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace ShareClass.Utilities.Helpers
{
    public class BitmapHelper
    {
        public static bool IsBrightArea(CanvasBitmap canvasBitmap, int left, int top, int width, int height)
        {
            if ((left + width > canvasBitmap.Size.Width) || ( top + height > canvasBitmap.Size.Height))
            return false;
            var arrColors = canvasBitmap.GetPixelColors(left, top, width, height);
            long acceptedNumber = arrColors.Length*5/100;
            long brightColor =
                arrColors.LongCount(arrColor => (arrColor.B > 200) && (arrColor.G > 200) && (arrColor.R > 200));

            if (brightColor > acceptedNumber) return true;
            return false;
        }

        public static Rect TextRect(string text, CanvasTextFormat textFormat, CanvasDrawingSession ds, double width = 0,
            double height = 0)
        {
            Rect theRectYouAreLookingFor;

            CanvasTextLayout textLayout = new CanvasTextLayout(ds, text, textFormat, (float) width, (float) height);
            theRectYouAreLookingFor = new Rect(0, 0, textLayout.DrawBounds.Width, textLayout.DrawBounds.Height);

            return theRectYouAreLookingFor;
        }

        //1- Top Left, 2- Top Mid, 3- Top Right, 4- Bottom Mid, 5- Bottom Right
        public static double ElementX(int number, double width)
        {
            if (number == 0) return 0;
            if (number%2 == 0) return width;
            return width/2;
        }
    }
}