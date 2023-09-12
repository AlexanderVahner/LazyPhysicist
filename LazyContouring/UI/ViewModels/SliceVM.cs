using LazyContouring.Graphics;
using LazyPhysicist.Common;
using System.Windows.Media.Imaging;

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

