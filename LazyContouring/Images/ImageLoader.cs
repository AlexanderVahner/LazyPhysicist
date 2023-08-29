using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
