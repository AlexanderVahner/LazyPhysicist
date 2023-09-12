using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Graphics
{
    public sealed class ImageStorage
    {
        private readonly Image image;
        private readonly WriteableBitmap bitmap;
        private readonly VoxelsToPixels converter = new VoxelsToPixels();
        private readonly int xSize;
        private readonly int ySize;
        private readonly int zSize;

        private readonly ImageSlice[] imageSlices;

        public ImageStorage(Image image)
        {
            this.image = image;
            xSize = image.XSize;
            ySize = image.YSize;
            zSize = image.ZSize;

            imageSlices = new ImageSlice[zSize];
            bitmap = new WriteableBitmap(
                xSize,
                ySize,
                96,
                96,
                PixelFormats.Bgra32,
                null);
        }

        public void RepaintBitmap(int planeIndex)
        {
            var buffer = GetVoxels(planeIndex);

            try
            {
                // Reserve the back buffer for updates.
                CTBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = CTBitmap.BackBuffer;

                    for (int y = 0; y < xSize; y++)
                    {
                        for (int x = 0; x < ySize; x++)
                        {
                            *((int*)pBackBuffer) = converter.GetBrga32(buffer[x, y]);

                            pBackBuffer += 4;

                            // Specify the area of the bitmap that changed.
                            CTBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                        }
                    }
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                CTBitmap.Unlock();
            }
        }

        public int[,] GetVoxels(int planeIndex)
        {
            if (imageSlices[planeIndex] == null)
            {
                imageSlices[planeIndex] = new ImageSlice(new int[xSize, ySize]);
                Image.GetVoxels(planeIndex, imageSlices[planeIndex].Voxels);
            }

            return imageSlices[planeIndex].Voxels;
        }

        public int XSize { get => xSize; }
        public int YSize { get => ySize; }
        public int ZSize { get => zSize; }

        public Image Image => image;

        public WriteableBitmap CTBitmap => bitmap;
    }
}

