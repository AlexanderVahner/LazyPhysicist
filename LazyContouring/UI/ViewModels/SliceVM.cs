using LazyContouring.Graphics;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace LazyContouring.UI.ViewModels
{
    public sealed class SliceVM : Notifier
    {
        private readonly SliceCanvas sliceCanvas;

        public SliceVM(SliceCanvas sliceCanvas)
        {
            this.sliceCanvas = sliceCanvas;
        }

        public double Width => SliceCanvas.Width;
        public double Height => SliceCanvas.Height;
        public WriteableBitmap CTBitmap => SliceCanvas.CTBitmap;

        public SliceCanvas SliceCanvas => sliceCanvas;
    }
}

