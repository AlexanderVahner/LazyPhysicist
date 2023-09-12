using System;
using System.Windows.Media.Imaging;

namespace LazyContouring.Images
{
    public static class ImageLoader
    {
        public static BitmapImage GetImage(string imageName)
        {
            return new BitmapImage(new Uri(@"pack://application:,,,/LazyContouring.esapi;component/Images/" + imageName, UriKind.Absolute));
        }
    }
}
