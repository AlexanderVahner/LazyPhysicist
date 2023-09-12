using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LazyContouring.Graphics
{
    public sealed class SliceCanvas : Canvas
    {
        private int currentSlice;
        private readonly StructureSetStorage structureSetStorage;
        private readonly ImageStorage imageStorage;



        public SliceCanvas(StructureSetStorage structureSetStorage, ImageStorage imageStorage)
        {
            this.structureSetStorage = structureSetStorage;
            this.imageStorage = imageStorage;

            Width = imageStorage.XSize;
            Height = imageStorage.YSize;

            Children.Add(new Image()
            {
                Source = CTBitmap,
                Width = Width,
                Height = Height
            });

            foreach (var structure in structureSetStorage.Structures)
            {
                Children.Add(structure.Path);
            }

            MouseWheel += SliceCanvas_MouseWheel;
        }

        private void SliceCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                CurrentSlice++;
            }
            else if (e.Delta < 0)
            {
                CurrentSlice--;
            }
        }

        private void PaintCurrentSlice()
        {
            imageStorage.RepaintBitmap(currentSlice);
            structureSetStorage.RepaintSlice(currentSlice);
        }

        private void SetCurrentSlice(int value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value >= imageStorage.ZSize)
            {
                value = imageStorage.ZSize - 1;
            }

            if (currentSlice != value)
            {
                currentSlice = value;
                PaintCurrentSlice();
            }
        }
        public int CurrentSlice { get => currentSlice; set => SetCurrentSlice(value); }
        public WriteableBitmap CTBitmap => imageStorage.CTBitmap;
    }
}
